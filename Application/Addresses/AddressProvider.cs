using Client.Dtos;
using DataAccess;
using Domain;

namespace Application.Addresses;

// This might be my personal preference, but I don't recommend using primary constructors as the inputs are not readonly
public class AddressProvider(IRepository<Address> addressRepo,
                             IUnitOfWork unitOfWork) : IAddressProvider
{
    private readonly IRepository<Address> _addressRepo = addressRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    // Rename to GetOrCreate to better describe what this method does
    // Could use a named tuple so the return value is more descriptive
    public Tuple<Address, Address> GetAddresses(AddressDto billingAddressDto,
                                                AddressDto shippingAddressDto)
    {
        var billingAddressHash = Address.GenerateAddressHash(billingAddressDto.AddressLineOne,
                                                             billingAddressDto.AddressLineTwo!,
                                                             billingAddressDto.AddressLineThree!,
                                                             billingAddressDto.PostCode);

        var shippingAddressHash = Address.GenerateAddressHash(shippingAddressDto.AddressLineOne,
                                                              shippingAddressDto.AddressLineTwo!,
                                                              shippingAddressDto.AddressLineThree!,
                                                              shippingAddressDto.PostCode);

        var lookupHashes = new Guid[] { billingAddressHash, shippingAddressHash };

        // Need to materialise this on this line with a ToList() or similar, otherwise it is 2 seperate DB requests
        // This should be _addressRepo
        var addresses = addressRepo.Get(x => lookupHashes.Contains(x.Hash));

        var billingAddress = addresses.SingleOrDefault(x => x.Hash == billingAddressHash)
                             ?? CreateAddress(billingAddressDto.AddressLineOne,
                                              billingAddressDto.AddressLineTwo!,
                                              billingAddressDto.AddressLineThree!,
                                              billingAddressDto.PostCode);

        var shippingAddress = addresses.SingleOrDefault(x => x.Hash == shippingAddressHash)
                              ?? CreateAddress(shippingAddressDto.AddressLineOne,
                                               shippingAddressDto.AddressLineTwo!,
                                               shippingAddressDto.AddressLineThree!,
                                               shippingAddressDto.PostCode);

        return new Tuple<Address, Address>(billingAddress, shippingAddress);
    }

    private Address CreateAddress(string lineOne,
                                  string lineTwo,
                                  string lineThree,
                                  string postCode)
    {
        var address = new Address(lineOne,
                                  lineTwo,
                                  lineThree,
                                  postCode);

        _addressRepo.Insert(address);

        // There's a question to be asked here about if the order creation fails after this point, do we care that the addresses are still in the DB?
        _unitOfWork.Save();

        return address;
    }
}