using FluentValidation;
using TechnicalBackendTest.Api.Contracts.Request;

namespace TechnicalBackendTest.Api.Application.Users.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty();

        RuleFor(request => request.Email)
            .NotEmpty()
            .EmailAddress();
    }
}