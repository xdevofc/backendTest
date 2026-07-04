namespace TechnicalBackendTest.Api.Contracts.Responses;

public record CurrencyConversionResponse(
    string FromCurrency,
    string ToCurrency,
    decimal OriginalAmount,
    decimal ConvertedAmount
);
