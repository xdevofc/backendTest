using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Users.Commands;

internal static class UpdateUserCommand
{
    public static async Task<UpdateUserCommandResult> ExecuteAsync(
        int id,
        UpdateUserRequest request,
        AppDbContext db,
        IValidator<UpdateUserRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return UpdateUserCommandResult.ValidationFailed(validation);
        }

        var user = await db.Users.FindAsync(id);

        if (user is null)
        {
            return UpdateUserCommandResult.NotFound();
        }

        var emailExists = await db.Users.AnyAsync(existingUser =>
            existingUser.Email == request.Email && existingUser.Id != id);

        if (emailExists)
        {
            return UpdateUserCommandResult.EmailAlreadyExists();
        }

        user.Name = request.Name;
        user.Email = request.Email;
        user.IsActive = request.IsActive;

        await db.SaveChangesAsync();

        return UpdateUserCommandResult.Updated();
    }
}

internal sealed record UpdateUserCommandResult(
    UpdateUserCommandStatus Status,
    ValidationResult? Validation = null)
{
    public static UpdateUserCommandResult Updated()
    {
        return new UpdateUserCommandResult(UpdateUserCommandStatus.Updated);
    }

    public static UpdateUserCommandResult ValidationFailed(ValidationResult validation)
    {
        return new UpdateUserCommandResult(UpdateUserCommandStatus.ValidationFailed, validation);
    }

    public static UpdateUserCommandResult NotFound()
    {
        return new UpdateUserCommandResult(UpdateUserCommandStatus.NotFound);
    }

    public static UpdateUserCommandResult EmailAlreadyExists()
    {
        return new UpdateUserCommandResult(UpdateUserCommandStatus.EmailAlreadyExists);
    }
}

internal enum UpdateUserCommandStatus
{
    Updated,
    ValidationFailed,
    NotFound,
    EmailAlreadyExists
}
