namespace Fiap.CloudGames.Application.Users.Dtos;

/// <summary>
/// DTO usado para cadastro de usuário pelo próprio usuário (self-signup).
/// </summary>
/// <param name="Name">Nome completo do usuário.</param>
/// <param name="Email">Endereço de email.</param>
/// <param name="Password">Senha em texto plano (será validada/hasheada pelo domínio).</param>
/// <param name="TenantId">Tenant (FIAP, Alura, PM3). Opcional — usa o header X-Tenant-Id ou FIAP por padrão.</param>
public record UserRegisterDto(string Name, string Email, string Password, string? TenantId = null);
