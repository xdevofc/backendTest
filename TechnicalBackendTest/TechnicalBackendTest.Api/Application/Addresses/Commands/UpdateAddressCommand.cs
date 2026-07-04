using FluentValidation;
using FluentValidation.Results;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Addresses.Commands;

internal static class UpdateAddressCommand
{
    public static async Task<UpdateAddressCommandResult> ExecuteAsync(
        int id,
        UpdateAddressRequest request,
        AppDbContext db,
        IValidator<UpdateAddressRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return UpdateAddressCommandResult.ValidationFailed(validation);
        }

        var address = await db.Addresses.FindAsync(id);

        if (address is null)
        {
            return UpdateAddressCommandResult.NotFound();
        }

        address.Street = request.Street;
        address.City = request.City;
        address.Country = request.Country;
        address.ZipCode = request.ZipCode;

        await db.SaveChangesAsync();

        return UpdateAddressCommandResult.Updated();
    }
}

internal sealed record UpdateAddressCommandResult(
    UpdateAddressCommandStatus Status,
    ValidationResult? Validation = null)
{
    public static UpdateAddressCommandResult Updated()
    {
        return new UpdateAddressCommandResult(UpdateAddressCommandStatus.Updated);
    }

    public static UpdateAddressCommandResult ValidationFailed(ValidationResult validation)
    {
        return new UpdateAddressCommandResult(UpdateAddressCommandStatus.ValidationFailed, validation);
    }

    public static UpdateAddressCommandResult NotFound()
    {
        return new UpdateAddressCommandResult(UpdateAddressCommandStatus.NotFound);
    }
}

internal enum UpdateAddressCommandStatus
{
    Updated,
    ValidationFailed,
    NotFound
}
