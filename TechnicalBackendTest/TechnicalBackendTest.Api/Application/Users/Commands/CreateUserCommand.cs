using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Application.Users.Mappings;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Domain.Entities;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Users.Commands;

internal static class CreateUserCommand
{
    public static async Task<CreateUserCommandResult> ExecuteAsync(
        CreateUserRequest request,
        AppDbContext db,
        IValidator<CreateUserRequest> validator,
        IPasswordHasher<User> passwordHasher)
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return CreateUserCommandResult.ValidationFailed(validation);
        }

        var emailExists = await db.Users.AnyAsync(user => user.Email == request.Email);

        if (emailExists)
        {
            return CreateUserCommandResult.EmailAlreadyExists();
        }

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            IsActive = true
        };

        user.PasswordHash = passwordHasher.HashPassword(user, request.Name);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return CreateUserCommandResult.Created(user.ToResponse());
    }
}

internal sealed record CreateUserCommandResult(
    CreateUserCommandStatus Status,
    UserResponse? User = null,
    ValidationResult? Validation = null)
{
    public static CreateUserCommandResult Created(UserResponse user)
    {
        return new CreateUserCommandResult(CreateUserCommandStatus.Created, user);
    }

    public static CreateUserCommandResult ValidationFailed(ValidationResult validation)
    {
        return new CreateUserCommandResult(CreateUserCommandStatus.ValidationFailed, Validation: validation);
    }

    public static CreateUserCommandResult EmailAlreadyExists()
    {
        return new CreateUserCommandResult(CreateUserCommandStatus.EmailAlreadyExists);
    }
}

internal enum CreateUserCommandStatus
{
    Created,
    ValidationFailed,
    EmailAlreadyExists
}
