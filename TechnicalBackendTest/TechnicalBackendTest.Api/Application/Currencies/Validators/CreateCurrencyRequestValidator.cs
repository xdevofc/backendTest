using FluentValidation;
using TechnicalBackendTest.Api.Contracts.Request;

namespace TechnicalBackendTest.Api.Application.Currencies.Validators;

public class CreateCurrencyRequestValidator : AbstractValidator<CreateCurrencyRequest>
{
    public CreateCurrencyRequestValidator()
    {
        RuleFor(request => request.Code)
            .NotEmpty();

        RuleFor(request => request.Name)
            .NotEmpty();

        RuleFor(request => request.RateToBase)
            .GreaterThan(0);
    }
}
