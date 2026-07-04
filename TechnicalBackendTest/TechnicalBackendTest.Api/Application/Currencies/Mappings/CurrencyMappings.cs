using System.Linq.Expressions;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Domain.Entities;

namespace TechnicalBackendTest.Api.Application.Currencies.Mappings;

internal static class CurrencyMappings
{
    public static Expression<Func<Currency, CurrencyResponse>> ProjectToResponse
    {
        get
        {
            return currency => new CurrencyResponse(
                currency.Id,
                currency.Code,
                currency.Name,
                currency.RateToBase);
        }
    }

    public static CurrencyResponse ToResponse(this Currency currency)
    {
        return new CurrencyResponse(
            currency.Id,
            currency.Code,
            currency.Name,
            currency.RateToBase);
    }
}
