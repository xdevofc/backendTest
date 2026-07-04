namespace TechnicalBackendTest.Api.Contracts.Responses;

public record AddressResponse(
    int Id,
    int UserId,
    string Street,
    string City,
    string Country,
    string? ZipCode
);
