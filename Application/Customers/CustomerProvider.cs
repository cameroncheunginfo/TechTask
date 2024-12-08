using Client.Dtos;
using DataAccess;
using Domain;

namespace Application.Customers;

public class CustomerProvider(IRepository<Customer> customerRepo,
                              IUnitOfWork unitOfWork) : ICustomerProvider
{
    private readonly IRepository<Customer> _customerRepo = customerRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Customer GetCustomer(CustomerDto request)
    {
        var customer = _customerRepo.Get(x => x.Email == request.EmailAddress.ToLowerInvariant())
                                    .SingleOrDefault();

        if (customer is not null)
            return customer;

        customer = new Customer(request.EmailAddress,
                                request.CustomerName,
                                request.PhoneNumber);

        customerRepo.Insert(customer);

        _unitOfWork.Save();

        return customer;
    }
}