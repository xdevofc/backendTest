using FluentValidation;
using TechnicalBackendTest.Api.Contracts.Request;

namespace TechnicalBackendTest.Api.Application.Addresses.Validators;

public class UpdateAddressRequestValidator : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressRequestValidator()
    {
        RuleFor(request => request.Street)
            .NotEmpty();

        RuleFor(request => request.City)
            .NotEmpty();

        RuleFor(request => request.Country)
            .NotEmpty();
    }
}
