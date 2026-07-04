using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Domain.Entities;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Users.Queries;

internal static class CheckUserPasswordQuery
{
    public static async Task<CheckUserPasswordQueryResult> ExecuteAsync(
        CheckUserPasswordRequest request,
        AppDbContext db,
        IValidator<CheckUserPasswordRequest> validator,
        IPasswordHasher<User> passwordHasher)
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return CheckUserPasswordQueryResult.ValidationFailed(validation);
        }

        var user = await db.Users.FindAsync(request.Id);

        if (user is null)
        {
            return CheckUserPasswordQueryResult.NotFound();
        }

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return CheckUserPasswordQueryResult.Checked(new CheckUserPasswordResponse(false));
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        var isCorrect = verificationResult is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;

        return CheckUserPasswordQueryResult.Checked(new CheckUserPasswordResponse(isCorrect));
    }
}

internal sealed record CheckUserPasswordQueryResult(
    CheckUserPasswordQueryStatus Status,
    CheckUserPasswordResponse? PasswordCheck = null,
    ValidationResult? Validation = null)
{
    public static CheckUserPasswordQueryResult Checked(CheckUserPasswordResponse passwordCheck)
    {
        return new CheckUserPasswordQueryResult(CheckUserPasswordQueryStatus.Checked, passwordCheck);
    }

    public static CheckUserPasswordQueryResult ValidationFailed(ValidationResult validation)
    {
        return new CheckUserPasswordQueryResult(CheckUserPasswordQueryStatus.ValidationFailed, Validation: validation);
    }

    public static CheckUserPasswordQueryResult NotFound()
    {
        return new CheckUserPasswordQueryResult(CheckUserPasswordQueryStatus.NotFound);
    }
}

internal enum CheckUserPasswordQueryStatus
{
    Checked,
    ValidationFailed,
    NotFound
}
