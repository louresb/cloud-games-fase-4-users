using Fiap.CloudGames.Domain.Users.Enums;

namespace Fiap.CloudGames.Application.Users.Dtos;

/// <summary>
/// DTO usado pelo administrador para criar um usuário.
/// </summary>
/// <param name="Name">Nome do usuário.</param>
/// <param name="Email">Email do usuário.</param>
/// <param name="Role">Papel do usuário.</param>
/// <param name="TenantId">Tenant do usuário (FIAP, Alura, PM3). Opcional — padrão FIAP.</param>
public record AdminUserCreateDto(string Name, string Email, UserRole Role, string? TenantId = null);
