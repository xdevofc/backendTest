using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Application.Currencies.Mappings;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Domain.Entities;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Currencies.Commands;

internal static class CreateCurrencyCommand
{
    public static async Task<CreateCurrencyCommandResult> ExecuteAsync(
        CreateCurrencyRequest request,
        AppDbContext db,
        IValidator<CreateCurrencyRequest> validator)
    {
        var validation = await validator.ValidateAsync(request);

        if (!validation.IsValid)
        {
            return CreateCurrencyCommandResult.ValidationFailed(validation);
        }

        var code = NormalizeCode(request.Code);
        var codeExists = await db.Currencies.AnyAsync(currency => currency.Code == code);

        if (codeExists)
        {
            return CreateCurrencyCommandResult.CodeAlreadyExists();
        }

        var currency = new Currency
        {
            Code = code,
            Name = request.Name,
            RateToBase = request.RateToBase
        };

        db.Currencies.Add(currency);
        await db.SaveChangesAsync();

        return CreateCurrencyCommandResult.Created(currency.ToResponse());
    }

    private static string NormalizeCode(string code)
    {
        return code.Trim().ToUpperInvariant();
    }
}

internal sealed record CreateCurrencyCommandResult(
    CreateCurrencyCommandStatus Status,
    CurrencyResponse? Currency = null,
    ValidationResult? Validation = null)
{
    public static CreateCurrencyCommandResult Created(CurrencyResponse currency)
    {
        return new CreateCurrencyCommandResult(CreateCurrencyCommandStatus.Created, currency);
    }

    public static CreateCurrencyCommandResult ValidationFailed(ValidationResult validation)
    {
        return new CreateCurrencyCommandResult(CreateCurrencyCommandStatus.ValidationFailed, Validation: validation);
    }

    public static CreateCurrencyCommandResult CodeAlreadyExists()
    {
        return new CreateCurrencyCommandResult(CreateCurrencyCommandStatus.CodeAlreadyExists);
    }
}

internal enum CreateCurrencyCommandStatus
{
    Created,
    ValidationFailed,
    CodeAlreadyExists
}
