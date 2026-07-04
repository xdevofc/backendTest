namespace TechnicalBackendTest.Api.Contracts.Responses;

public record CurrencyResponse(
    int Id,
    string Code,
    string Name,
    decimal RateToBase
);
