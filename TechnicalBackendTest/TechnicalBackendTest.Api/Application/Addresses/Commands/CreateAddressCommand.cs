using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Application.Addresses.Mappings;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Domain.Entities;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Addresses.Commands;

internal static class CreateAddressCommand
{
    public static async Task<CreateAddressCommandResult> ExecuteAsync(
        int userId,
        CreateAddressRequest request,
        AppDbContext db,
        IValidator<CreateAddressRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return CreateAddressCommandResult.ValidationFailed(validation);
        }

        var userExists = await db.Users.AnyAsync(user => user.Id == userId);

        if (!userExists)
        {
            return CreateAddressCommandResult.UserNotFound();
        }

        var address = new Address
        {
            UserId = userId,
            Street = request.Street,
            City = request.City,
            Country = request.Country,
            ZipCode = request.ZipCode
        };

        db.Addresses.Add(address);
        await db.SaveChangesAsync();

        return CreateAddressCommandResult.Created(address.ToResponse());
    }
}

internal sealed record CreateAddressCommandResult(
    CreateAddressCommandStatus Status,
    AddressResponse? Address = null,
    ValidationResult? Validation = null)
{
    public static CreateAddressCommandResult Created(AddressResponse address)
    {
        return new CreateAddressCommandResult(CreateAddressCommandStatus.Created, address);
    }

    public static CreateAddressCommandResult ValidationFailed(ValidationResult validation)
    {
        return new CreateAddressCommandResult(CreateAddressCommandStatus.ValidationFailed, Validation: validation);
    }

    public static CreateAddressCommandResult UserNotFound()
    {
        return new CreateAddressCommandResult(CreateAddressCommandStatus.UserNotFound);
    }
}

internal enum CreateAddressCommandStatus
{
    Created,
    ValidationFailed,
    UserNotFound
}
