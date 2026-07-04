using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Application.Users.Mappings;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Users.Queries;

internal static class GetUsersQuery
{
    public static async Task<List<UserResponse>> ExecuteAsync(
        AppDbContext db,
        bool? isActive)
    {
        var query = db.Users.AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(user => user.IsActive == isActive.Value);
        }

        return await query
            .Select(UserMappings.ProjectToResponse)
            .ToListAsync();
    }
}
