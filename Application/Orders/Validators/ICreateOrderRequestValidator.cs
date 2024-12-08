using Domain;

namespace Application.Orders.Validators;

public interface ICreateOrderRequestValidator
{
    bool TryValidate(Customer customer,
                     Address billingAddress,
                     Address shippingAddress,
                     out IDictionary<string, string> errors);
}