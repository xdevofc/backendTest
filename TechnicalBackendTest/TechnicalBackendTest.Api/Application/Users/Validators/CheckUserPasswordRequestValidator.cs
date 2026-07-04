using FluentValidation;
using TechnicalBackendTest.Api.Contracts.Request;

namespace TechnicalBackendTest.Api.Application.Users.Validators;

public class CheckUserPasswordRequestValidator : AbstractValidator<CheckUserPasswordRequest>
{
    public CheckUserPasswordRequestValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0);

        RuleFor(request => request.Password)
            .NotEmpty();
    }
}
