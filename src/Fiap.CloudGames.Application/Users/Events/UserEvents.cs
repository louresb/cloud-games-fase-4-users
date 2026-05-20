namespace Fiap.CloudGames.Application.Users.Events;

public record UserSignedUpEvent(Guid Id, string Name, string Email, string ConfirmationToken, string TenantId = "FIAP");
public record UserEmailConfirmedEvent(Guid Id, string Name, string Email, string TenantId = "FIAP");
public record UserInvitedEvent(Guid Id, string Name, string Email, string FirstAccessToken, string TenantId = "FIAP");
public record UserFirstAccessedEvent(Guid Id, string Name, string Email, string TenantId = "FIAP");
public record UserForgotPasswordEvent(Guid Id, string Name, string Email, string ResetToken, string TenantId = "FIAP");
public record UserPasswordResetedEvent(Guid Id, string Name, string Email, string TenantId = "FIAP");