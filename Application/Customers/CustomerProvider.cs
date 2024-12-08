using Client.Dtos;
using DataAccess;
using Domain;

namespace Application.Customers;

public class CustomerProvider(IRepository<Customer> customerRepo,
                              IUnitOfWork unitOfWork) : ICustomerProvider
{
    private readonly IRepository<Customer> _customerRepo = customerRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    // Rename to GetOrCreateCustomer to better describe functionality
    public Customer GetCustomer(CustomerDto request)
    {
        var customer = _customerRepo.Get(x => x.Email == request.EmailAddress.ToLowerInvariant())
                                    .SingleOrDefault();

        if (customer is not null)
            return customer;

        // You're comparing the email address as lowercase on line 16, so somewhere you need to convert the request.EmailAddress to lowercase as well. Inside the Domain entity is best.
        customer = new Customer(request.EmailAddress,
                                request.CustomerName,
                                request.PhoneNumber);

        customerRepo.Insert(customer);

        _unitOfWork.Save();

        return customer;
    }
}