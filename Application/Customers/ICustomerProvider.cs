using Client.Dtos;
using Domain;

namespace Application.Customers;

public interface ICustomerProvider
{
    Customer GetCustomer(CustomerDto request);
}