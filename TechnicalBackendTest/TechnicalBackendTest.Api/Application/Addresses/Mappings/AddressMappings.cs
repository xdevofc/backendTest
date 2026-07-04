using System.Linq.Expressions;
using TechnicalBackendTest.Api.Contracts.Responses;
using TechnicalBackendTest.Api.Domain.Entities;

namespace TechnicalBackendTest.Api.Application.Addresses.Mappings;

internal static class AddressMappings
{
    public static Expression<Func<Address, AddressResponse>> ProjectToResponse
    {
        get
        {
            return address => new AddressResponse(
                address.Id,
                address.UserId,
                address.Street,
                address.City,
                address.Country,
                address.ZipCode);
        }
    }

    public static AddressResponse ToResponse(this Address address)
    {
        return new AddressResponse(
            address.Id,
            address.UserId,
            address.Street,
            address.City,
            address.Country,
            address.ZipCode);
    }
}
