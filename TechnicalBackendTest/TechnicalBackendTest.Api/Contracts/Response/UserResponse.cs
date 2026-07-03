namespace TechnicalBackendTest.Api.Contracts.Responses;

public record UserResponse(
    int Id,
    string Name,
    string Email,
    bool IsActive
);