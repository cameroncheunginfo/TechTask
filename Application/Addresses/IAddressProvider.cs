using Client.Dtos;
using Domain;

namespace Application.Addresses;

public interface IAddressProvider
{
    Tuple<Address, Address> GetAddresses(AddressDto billingAddressDto,
                                         AddressDto shippingAddressDto);
}