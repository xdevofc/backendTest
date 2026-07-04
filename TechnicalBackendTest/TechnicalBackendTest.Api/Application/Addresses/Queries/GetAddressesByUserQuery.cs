using Microsoft.EntityFrameworkCore;
using TechnicalBackendTest.Api.Application.Addresses.Mappings;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Infrastructure.Persistence;

namespace TechnicalBackendTest.Api.Application.Addresses.Queries;

internal static class GetAddressesByUserQuery
{
    public static async Task<GetAddressesByUserQueryResult> ExecuteAsync(
        int userId,
        AppDbContext db)
    {
        var userExists = await db.Users.AnyAsync(user => user.Id == userId);

        if (!userExists)
        {
            return GetAddressesByUserQueryResult.UserNotFound();
        }

        var addresses = await db.Addresses
            .Where(address => address.UserId == userId)
            .Select(AddressMappings.ProjectToResponse)
            .ToListAsync();

        return GetAddressesByUserQueryResult.Found(addresses);
    }
}

internal sealed record GetAddressesByUserQueryResult(
    GetAddressesByUserQueryStatus Status,
    List<AddressResponse>? Addresses = null)
{
    public static GetAddressesByUserQueryResult Found(List<AddressResponse> addresses)
    {
        return new GetAddressesByUserQueryResult(GetAddressesByUserQueryStatus.Found, addresses);
    }

    public static GetAddressesByUserQueryResult UserNotFound()
    {
        return new GetAddressesByUserQueryResult(GetAddressesByUserQueryStatus.UserNotFound);
    }
}

internal enum GetAddressesByUserQueryStatus
{
    Found,
    UserNotFound
}
