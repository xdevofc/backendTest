using FluentValidation;
using Microsoft.AspNetCore.Identity;
using TechnicalBackendTest.Api.Application.Users.Commands;
using TechnicalBackendTest.Api.Application.Users.Queries;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Domain.Entities;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users");

        group.MapPost("/", CreateUserAsync);
        group.MapPost("/check-password", CheckUserPasswordAsync);
        group.MapGet("/", GetUsersAsync);
        group.MapGet("/{id:int}", GetUserByIdAsync);
        group.MapPut("/{id:int}", UpdateUserAsync);
        group.MapDelete("/{id:int}", DeleteUserAsync);
    }

    private static async Task<IResult> CreateUserAsync(
        CreateUserRequest request,
        AppDbContext db,
        IValidator<CreateUserRequest> validator,
        IPasswordHasher<User> passwordHasher)
    {
        var result = await CreateUserCommand.ExecuteAsync(request, db, validator, passwordHasher);

        return result.Status switch
        {
            CreateUserCommandStatus.ValidationFailed => Results.ValidationProblem(ToErrors(result.Validation!)),
            CreateUserCommandStatus.EmailAlreadyExists => Results.Conflict("Email already exists."),
            CreateUserCommandStatus.Created => Results.Created($"/users/{result.User!.Id}", result.User),
            _ => Results.Problem()
        };
    }

    private static async Task<IResult> CheckUserPasswordAsync(
        CheckUserPasswordRequest request,
        AppDbContext db,
        IValidator<CheckUserPasswordRequest> validator,
        IPasswordHasher<User> passwordHasher)
    {
        var result = await CheckUserPasswordQuery.ExecuteAsync(request, db, validator, passwordHasher);

        return result.Status switch
        {
            CheckUserPasswordQueryStatus.ValidationFailed => Results.ValidationProblem(ToErrors(result.Validation!)),
            CheckUserPasswordQueryStatus.NotFound => Results.NotFound(),
            CheckUserPasswordQueryStatus.Checked => Results.Ok(result.PasswordCheck),
            _ => Results.Problem()
        };
    }

    private static async Task<IResult> GetUsersAsync(
        AppDbContext db,
        bool? isActive)
    {
        var users = await GetUsersQuery.ExecuteAsync(db, isActive);

        return Results.Ok(users);
    }

    private static async Task<IResult> GetUserByIdAsync(
        int id,
        AppDbContext db)
    {
        var user = await GetUserByIdQuery.ExecuteAsync(id, db);

        return user is null
            ? Results.NotFound()
            : Results.Ok(user);
    }

    private static async Task<IResult> UpdateUserAsync(
        int id,
        UpdateUserRequest request,
        AppDbContext db,
        IValidator<UpdateUserRequest> validator)
    {
        var result = await UpdateUserCommand.ExecuteAsync(id, request, db, validator);

        return result.Status switch
        {
            UpdateUserCommandStatus.ValidationFailed => Results.ValidationProblem(ToErrors(result.Validation!)),
            UpdateUserCommandStatus.NotFound => Results.NotFound(),
            UpdateUserCommandStatus.EmailAlreadyExists => Results.Conflict("Email already exists."),
            UpdateUserCommandStatus.Updated => Results.NoContent(),
            _ => Results.Problem()
        };
    }

    private static async Task<IResult> DeleteUserAsync(
        int id,
        AppDbContext db)
    {
        var result = await DeleteUserCommand.ExecuteAsync(id, db);

        return result.Status switch
        {
            DeleteUserCommandStatus.NotFound => Results.NotFound(),
            DeleteUserCommandStatus.Deleted => Results.NoContent(),
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
