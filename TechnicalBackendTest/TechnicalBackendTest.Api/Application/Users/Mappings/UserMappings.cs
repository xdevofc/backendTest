using System.Linq.Expressions;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Domain.Entities;

namespace TechnicalBackendTest.Api.Application.Users.Mappings;

internal static class UserMappings
{
    public static Expression<Func<User, UserResponse>> ProjectToResponse
    {
        get
        {
            return user => new UserResponse(
                user.Id,
                user.Name,
                user.Email,
                user.IsActive);
        }
    }

    public static UserResponse ToResponse(this User user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.IsActive);
    }
}
