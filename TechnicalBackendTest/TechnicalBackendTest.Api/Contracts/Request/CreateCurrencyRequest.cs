namespace TechnicalBackendTest.Api.Contracts.Request;

public record CreateCurrencyRequest(
    string Code,
    string Name,
    decimal RateToBase
);
