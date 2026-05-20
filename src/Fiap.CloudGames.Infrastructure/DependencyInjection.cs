using System.Reflection;
using Fiap.CloudGames.Domain.Users.Interfaces;
using Fiap.CloudGames.Domain.Users.Repositories;
using Fiap.CloudGames.Infrastructure.Auth;
using Fiap.CloudGames.Infrastructure.Persistence;
using Fiap.CloudGames.Infrastructure.Users.Repositories;
using Fiap.CloudGames.Infrastructure.Users.Seeders;
using MassTransit;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.CloudGames.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration,
        Assembly? consumersAssembly = null,
        Type[]? consumerCommandTypes = null,
        Type[]? consumerEventTypes = null)
    {
        services.AddSingleton<IJwtService, JwtService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSeeder, UserSeeder>();

        var usersCommandsQueue = configuration["Queues:Users:Commands"] ?? throw new InvalidOperationException("Users commands queue not configured.");
        var usersEventsQueue = configuration["Queues:Users:Events"] ?? throw new InvalidOperationException("Users events queue not configured.");

        consumerCommandTypes = consumerCommandTypes?.Where(t => typeof(IConsumer).IsAssignableFrom(t)).ToArray() ?? [];
        consumerEventTypes = consumerEventTypes?.Where(t => typeof(IConsumer).IsAssignableFrom(t)).ToArray() ?? [];

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                // Pega a configuração injetada
                var configuration = context.GetRequiredService<IConfiguration>();

                // Lê as variáveis obrigatórias de RabbitMQ
                var rabbitHost = configuration["RabbitMq:HostName"] ?? throw new InvalidOperationException("RabbitMq host name not configured.");
                var rabbitUser = configuration["RabbitMq:UserName"] ?? throw new InvalidOperationException("RabbitMq user name not configured.");
                var rabbitPass = configuration["RabbitMq:Password"] ?? throw new InvalidOperationException("RabbitMq password not configured.");

                cfg.Host(rabbitHost, "/", h =>
                {
                    h.ConnectionName("Fiap.CloudGames.Users.API");
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                });

                cfg.ReceiveEndpoint(usersCommandsQueue, e =>
                {
                    foreach (var consumerType in consumerCommandTypes)
                    {
                        if (!typeof(IConsumer).IsAssignableFrom(consumerType))
                            throw new InvalidOperationException($"Type {consumerType.FullName} is not a MassTransit consumer.");

                        e.ConfigureConsumer(context, consumerType);
                    }
                });
                
                cfg.ReceiveEndpoint(usersEventsQueue, e =>
                {
                    foreach (var consumerType in consumerEventTypes)
                    {
                        if (!typeof(IConsumer).IsAssignableFrom(consumerType))
                            throw new InvalidOperationException($"Type {consumerType.FullName} is not a MassTransit consumer.");

                        e.ConfigureConsumer(context, consumerType);
                    }
                });

                cfg.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(2)));
            });
        });

        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);

                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });
        });

        services.AddDataProtection()
            .SetApplicationName("Fiap.CloudGames")
            .PersistKeysToDbContext<AppDbContext>();

        services.AddHealthChecks()
            .AddSqlServer(
                connectionString: connectionString, 
                name: "sqlserver",
                tags: new[] { "db", "data" });
        
        return services;
    }
}
