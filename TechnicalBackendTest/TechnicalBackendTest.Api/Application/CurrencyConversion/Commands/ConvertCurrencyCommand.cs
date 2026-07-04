using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.CurrencyConversion.Commands;

internal static class ConvertCurrencyCommand
{
    public static async Task<ConvertCurrencyCommandResult> ExecuteAsync(
        ConvertCurrencyRequest request,
        AppDbContext db,
        IValidator<ConvertCurrencyRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return ConvertCurrencyCommandResult.ValidationFailed(validation);
        }

        var fromCode = NormalizeCode(request.FromCurrencyCode);
        var toCode = NormalizeCode(request.ToCurrencyCode);

        var fromCurrency = await db.Currencies.FirstOrDefaultAsync(currency => currency.Code == fromCode);
        var toCurrency = await db.Currencies.FirstOrDefaultAsync(currency => currency.Code == toCode);

        if (fromCurrency is null || toCurrency is null)
        {
            return ConvertCurrencyCommandResult.CurrencyNotFound();
        }

        var baseAmount = request.Amount * fromCurrency.RateToBase;
        var convertedAmount = baseAmount / toCurrency.RateToBase;

        var conversion = new CurrencyConversionResponse(
            fromCurrency.Code,
            toCurrency.Code,
            request.Amount,
            convertedAmount);

        return ConvertCurrencyCommandResult.Converted(conversion);
    }

    private static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }
}

internal sealed record ConvertCurrencyCommandResult(
    ConvertCurrencyCommandStatus Status,
    CurrencyConversionResponse? Conversion = null,
    ValidationResult? Validation = null)
{
    public static ConvertCurrencyCommandResult Converted(CurrencyConversionResponse conversion)
    {
        return new ConvertCurrencyCommandResult(ConvertCurrencyCommandStatus.Converted, conversion);
    }

    public static ConvertCurrencyCommandResult ValidationFailed(ValidationResult validation)
    {
        return new ConvertCurrencyCommandResult(ConvertCurrencyCommandStatus.ValidationFailed, Validation: validation);
    }

    public static ConvertCurrencyCommandResult CurrencyNotFound()
    {
        return new ConvertCurrencyCommandResult(ConvertCurrencyCommandStatus.CurrencyNotFound);
    }
}

internal enum ConvertCurrencyCommandStatus
{
    Converted,
    ValidationFailed,
    CurrencyNotFound
}
