namespace TechnicalBackendTest.Api.Contracts.Request;

public record UpdateAddressRequest(
    string Street,
    string City,
    string Country,
    string? ZipCode
);
