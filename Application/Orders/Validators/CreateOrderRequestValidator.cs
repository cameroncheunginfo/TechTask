using System.Text.RegularExpressions;
using Domain;

namespace Application.Orders.Validators;

// Fluent Validation could be a good alternative to this - which is more readable (https://docs.fluentvalidation.net/en/latest/)
public class CreateOrderRequestValidator : ICreateOrderRequestValidator
{
    public bool TryValidate(Customer customer,
                            Address billingAddress,
                            Address shippingAddress,
                            out IDictionary<string, string> errors)
    {
        errors = new Dictionary<string, string>();

        // Does validation of a customer really belong here, should it be on when the customer is created?
        if (string.IsNullOrWhiteSpace(customer.Name))
            errors.Add(nameof(customer.Name), "Name is required");

        /* Regex does not validate to RFC 5322
         * Erronously valid: cameron...cheung@gmail.com
         * 
         * Also, does the user need to create an account (and therefore validate their email) to place an order? If so, then this validation is not a concern of this service
        */
        if (!Regex.IsMatch(customer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            errors.Add(nameof(customer.Email), "Email is not valid");

        if (customer.Created > DateTime.Now) // Mismatched DateTime kind, should be DateTime.UtcNow
            errors.Add(nameof(customer.Created), "Customer cannot be from the future");

        if (string.IsNullOrWhiteSpace(billingAddress.LineOne))
            errors.Add(nameof(billingAddress.LineOne), "First address line is required");

        if (string.IsNullOrWhiteSpace(billingAddress.PostCode))
            errors.Add(nameof(billingAddress.PostCode), "PostCode is required");

        if (!Regex.IsMatch(billingAddress.PostCode,
                           @"^(GIR 0AA|((([A-Z]{1,2}[0-9][0-9A-Z]?)|(([A-Z]{1,2}[0-9][0-9A-Z]?)))(\s?[0-9][A-Z]{2})))$",
                           RegexOptions.IgnoreCase)) // Ignoring case but not converting to upper/lower case - will break hashing
            errors.Add(nameof(billingAddress.PostCode), "Postcode is not valid");

        if (string.IsNullOrWhiteSpace(shippingAddress.LineOne))
            errors.Add(nameof(shippingAddress.LineOne), "First address line is required"); // Update error messages to be able to distinguish between shippingAddress.LineOne and billingAddress.LineOne

        if (string.IsNullOrWhiteSpace(shippingAddress.PostCode))
            errors.Add(nameof(shippingAddress.PostCode), "PostCode is required");

        if (!Regex.IsMatch(shippingAddress.PostCode,
                           @"^(GIR 0AA|((([A-Z]{1,2}[0-9][0-9A-Z]?)|(([A-Z]{1,2}[0-9][0-9A-Z]?)))(\s?[0-9][A-Z]{2})))$",
                           RegexOptions.IgnoreCase))
            errors.Add(nameof(shippingAddress.PostCode), "Postcode is not valid");

        return !errors.Any();
    }
}