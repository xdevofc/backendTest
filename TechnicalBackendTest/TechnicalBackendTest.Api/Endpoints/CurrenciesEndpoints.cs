using FluentValidation;
using TechnicalBackendTest.Api.Application.CurrencyConversion.Commands;
using TechnicalBackendTest.Api.Application.Currencies.Commands;
using TechnicalBackendTest.Api.Application.Currencies.Queries;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Endpoints;

public static class CurrenciesEndpoints
{
    public static void MapCurrenciesEndpoints(this IEndpointRouteBuilder app)
    {
        var currenciesGroup = app.MapGroup("/currencies")
            .WithTags("Currencies");

        currenciesGroup.MapGet("", GetCurrenciesAsync);
        currenciesGroup.MapPost("", CreateCurrencyAsync);

        var currencyGroup = app.MapGroup("/currency")
            .WithTags("Currencies");

        currencyGroup.MapPost("/convert", ConvertCurrencyAsync);
    }

    private static async Task<IResult> GetCurrenciesAsync(AppDbContext db)
    {
        var currencies = await GetCurrenciesQuery.ExecuteAsync(db);

        return Results.Ok(currencies);
    }

    private static async Task<IResult> CreateCurrencyAsync(
        CreateCurrencyRequest request,
        AppDbContext db,
        IValidator<CreateCurrencyRequest> validator)
    {
        var result = await CreateCurrencyCommand.ExecuteAsync(request, db, validator);

        return result.Status switch
        {
            CreateCurrencyCommandStatus.ValidationFailed => Results.ValidationProblem(ToErrors(result.Validation!)),
            CreateCurrencyCommandStatus.CodeAlreadyExists => Results.Conflict("Currency code already exists."),
            CreateCurrencyCommandStatus.Created => Results.Created($"/currencies/{result.Currency!.Id}", result.Currency),
            _ => Results.Problem()
        };
    }

    private static async Task<IResult> ConvertCurrencyAsync(
        ConvertCurrencyRequest request,
        AppDbContext db,
        IValidator<ConvertCurrencyRequest> validator)
    {
        var result = await ConvertCurrencyCommand.ExecuteAsync(request, db, validator);

        return result.Status switch
        {
            ConvertCurrencyCommandStatus.ValidationFailed => Results.ValidationProblem(ToErrors(result.Validation!)),
            ConvertCurrencyCommandStatus.CurrencyNotFound => Results.NotFound(),
            ConvertCurrencyCommandStatus.Converted => Results.Ok(result.Conversion),
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
