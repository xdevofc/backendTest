namespace TechnicalBackendTest.Api.Contracts.Request;

public record CheckUserPasswordRequest(
    int Id,
    string Password
);
