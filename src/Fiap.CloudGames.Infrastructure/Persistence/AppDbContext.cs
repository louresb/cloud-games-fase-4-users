using Fiap.CloudGames.Domain.Users.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fiap.CloudGames.Infrastructure.Persistence;

public class AppDbContext : DbContext, IDataProtectionKeyContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

	public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

	// getter-only DbSets: safer (no reassignment) and avoids nullability warnings
	public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
