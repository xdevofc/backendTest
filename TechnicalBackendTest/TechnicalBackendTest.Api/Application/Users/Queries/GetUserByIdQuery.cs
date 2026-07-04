using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Application.Users.Mappings;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Users.Queries;

internal static class GetUserByIdQuery
{
    public static async Task<UserResponse?> ExecuteAsync(
        int id,
        AppDbContext db)
    {
        return await db.Users
            .Where(user => user.Id == id)
            .Select(UserMappings.ProjectToResponse)
            .FirstOrDefaultAsync();
    }
}
