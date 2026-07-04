using FluentValidation;
using TechnicalBackendTest.Api.Application.Addresses.Commands;
using TechnicalBackendTest.Api.Application.Addresses.Queries;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Endpoints;

public static class AddressesEndpoints
{
    public static void MapAddressesEndpoints(this IEndpointRouteBuilder app)
    {
        var usersGroup = app.MapGroup("/users")
            .WithTags("Addresses");

        usersGroup.MapPost("/{userId:int}/addresses", CreateAddressAsync);
        usersGroup.MapGet("/{userId:int}/addresses", GetAddressesByUserAsync);

        var addressesGroup = app.MapGroup("/addresses")
            .WithTags("Addresses");

        addressesGroup.MapPut("/{id:int}", UpdateAddressAsync);
        addressesGroup.MapDelete("/{id:int}", DeleteAddressAsync);
    }

    private static async Task<IResult> CreateAddressAsync(
        int userId,
        CreateAddressRequest request,
        AppDbContext db,
        IValidator<CreateAddressRequest> validator)
    {
        var result = await CreateAddressCommand.ExecuteAsync(userId, request, db, validator);

        return result.Status switch
        {
            CreateAddressCommandStatus.ValidationFailed => Results.ValidationProblem(ToErrors(result.Validation!)),
            CreateAddressCommandStatus.UserNotFound => Results.NotFound(),
            CreateAddressCommandStatus.Created => Results.Created($"/addresses/{result.Address!.Id}", result.Address),
            _ => Results.Problem()
        };
    }

    private static async Task<IResult> GetAddressesByUserAsync(
        int userId,
        AppDbContext db)
    {
        var result = await GetAddressesByUserQuery.ExecuteAsync(userId, db);

        return result.Status switch
        {
            GetAddressesByUserQueryStatus.UserNotFound => Results.NotFound(),
            GetAddressesByUserQueryStatus.Found => Results.Ok(result.Addresses),
            _ => Results.Problem()
        };
    }

    private static async Task<IResult> UpdateAddressAsync(
        int id,
        UpdateAddressRequest request,
        AppDbContext db,
        IValidator<UpdateAddressRequest> validator)
    {
        var result = await UpdateAddressCommand.ExecuteAsync(id, request, db, validator);

        return result.Status switch
        {
            UpdateAddressCommandStatus.ValidationFailed => Results.ValidationProblem(ToErrors(result.Validation!)),
            UpdateAddressCommandStatus.NotFound => Results.NotFound(),
            UpdateAddressCommandStatus.Updated => Results.NoContent(),
            _ => Results.Problem()
        };
    }

    private static async Task<IResult> DeleteAddressAsync(
        int id,
        AppDbContext db)
    {
        var result = await DeleteAddressCommand.ExecuteAsync(id, db);

        return result.Status switch
        {
            DeleteAddressCommandStatus.NotFound => Results.NotFound(),
            DeleteAddressCommandStatus.Deleted => Results.NoContent(),
            _ => Results.Problem()
        };
    }

    private static Dictionary<string, string[]> ToErrors(FluentValidation.Results.ValidationResult validation)
    {
        return validation.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());
    }
}
