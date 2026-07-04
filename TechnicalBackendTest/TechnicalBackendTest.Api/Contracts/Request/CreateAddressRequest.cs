namespace TechnicalBackendTest.Api.Contracts.Request;

public record CreateAddressRequest(
    string Street,
    string City,
    string Country,
    string? ZipCode
);
