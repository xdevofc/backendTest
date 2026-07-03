namespace TechnicalBackendTest.Api.Contracts.Request;

public record UpdateUserRequest(
    string Name,
    string Email,
    bool IsActive
);