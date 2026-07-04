using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Application.Currencies.Mappings;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Currencies.Queries;

internal static class GetCurrenciesQuery
{
    public static async Task<List<CurrencyResponse>> ExecuteAsync(AppDbContext db)
    {
        return await db.Currencies
            .Select(CurrencyMappings.ProjectToResponse)
            .ToListAsync();
    }
}
