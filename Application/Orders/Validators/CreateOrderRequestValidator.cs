using System.Text.RegularExpressions;
using Domain;

namespace Application.Orders.Validators;

public class CreateOrderRequestValidator : ICreateOrderRequestValidator
{
    public bool TryValidate(Customer customer,
                            Address billingAddress,
                            Address shippingAddress,
                            out IDictionary<string, string> errors)
    {
        errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(customer.Name))
            errors.Add(nameof(customer.Name), "Name is required");

        if (!Regex.IsMatch(customer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            errors.Add(nameof(customer.Email), "Email is not valid");

        if (customer.Created > DateTime.Now)
            errors.Add(nameof(customer.Created), "Customer cannot be from the future");

        if (string.IsNullOrWhiteSpace(billingAddress.LineOne))
            errors.Add(nameof(billingAddress.LineOne), "First address line is required");

        if (string.IsNullOrWhiteSpace(billingAddress.PostCode))
            errors.Add(nameof(billingAddress.PostCode), "PostCode is required");

        if (!Regex.IsMatch(billingAddress.PostCode,
                           @"^(GIR 0AA|((([A-Z]{1,2}[0-9][0-9A-Z]?)|(([A-Z]{1,2}[0-9][0-9A-Z]?)))(\s?[0-9][A-Z]{2})))$",
                           RegexOptions.IgnoreCase))
            errors.Add(nameof(billingAddress.PostCode), "Postcode is not valid");

        if (string.IsNullOrWhiteSpace(shippingAddress.LineOne))
            errors.Add(nameof(shippingAddress.LineOne), "First address line is required");

        if (string.IsNullOrWhiteSpace(shippingAddress.PostCode))
            errors.Add(nameof(shippingAddress.PostCode), "PostCode is required");

        if (!Regex.IsMatch(shippingAddress.PostCode,
                           @"^(GIR 0AA|((([A-Z]{1,2}[0-9][0-9A-Z]?)|(([A-Z]{1,2}[0-9][0-9A-Z]?)))(\s?[0-9][A-Z]{2})))$",
                           RegexOptions.IgnoreCase))
            errors.Add(nameof(shippingAddress.PostCode), "Postcode is not valid");

        return !errors.Any();
    }
}