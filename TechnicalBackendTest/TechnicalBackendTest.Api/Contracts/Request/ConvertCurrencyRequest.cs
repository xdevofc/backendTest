namespace TechnicalBackendTest.Api.Contracts.Request;

public record ConvertCurrencyRequest(
    string FromCurrencyCode,
    string ToCurrencyCode,
    decimal Amount
);
