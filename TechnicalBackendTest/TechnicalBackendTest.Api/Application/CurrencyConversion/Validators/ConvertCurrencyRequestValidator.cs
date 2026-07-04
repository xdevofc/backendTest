using FluentValidation;
using TechnicalBackendTest.Api.Contracts.Request;

namespace TechnicalBackendTest.Api.Application.CurrencyConversion.Validators;

public class ConvertCurrencyRequestValidator : AbstractValidator<ConvertCurrencyRequest>
{
    public ConvertCurrencyRequestValidator()
    {
        RuleFor(request => request.FromCurrencyCode)
            .NotEmpty();

        RuleFor(request => request.ToCurrencyCode)
            .NotEmpty();

        RuleFor(request => request.Amount)
            .GreaterThan(0);
    }
}
