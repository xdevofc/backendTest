namespace TechnicalBackendTest.Api.Contracts.Request;

public record CreateUserRequest(
    string Name,
    string Email
);