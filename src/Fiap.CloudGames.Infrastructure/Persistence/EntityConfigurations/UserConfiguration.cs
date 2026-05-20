using Fiap.CloudGames.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fiap.CloudGames.Infrastructure.Persistence.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("Users");

		builder.HasKey(u => u.Id);

		builder.Property(u => u.TenantId)
			.IsRequired()
			.HasMaxLength(50);

		builder.HasIndex(u => u.TenantId);

		builder.Property(u => u.Name)
			.IsRequired()
			.HasMaxLength(100);

		builder.OwnsOne(u => u.Email, email => 
		{
			email.Property(e => e.Address)
				.IsRequired()
				.HasMaxLength(255)
				.HasColumnName("Email");

			email.HasIndex(e => e.Address).IsUnique();
		});

		builder.OwnsOne(u => u.Password, password =>
		{
			password.Property(p => p.Hash)
				.IsRequired()
				.HasMaxLength(255)
				.HasColumnName("PasswordHash");
		});

		builder.Property(u => u.CreatedAt)
			.IsRequired();

		builder.Property(u => u.Role)
			.HasConversion<string>()
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(u => u.Status)
			.HasConversion<string>()
			.HasMaxLength(50)
			.IsRequired();

		builder.Property(u => u.EmailConfirmed)
			.IsRequired();

		builder.Property(u => u.ConfirmationToken)
			.HasMaxLength(255)
			.IsRequired(false);

		builder.Property(u => u.PasswordResetToken)
			.HasMaxLength(255)
			.IsRequired(false);

		builder.Property(u => u.PasswordResetExpiresAt)
			.IsRequired(false);

		builder.Property(u => u.FirstAccessToken)
			.HasMaxLength(255)
			.IsRequired(false);

		builder.Property(u => u.FirstAccessExpiresAt)
			.IsRequired(false);
	}
}
