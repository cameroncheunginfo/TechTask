namespace Client.Dtos;

public class CustomerDto
{
    public string CustomerName { get; set; }
    public string EmailAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public AddressDto BillingAddress { get; set; }
    public AddressDto ShippingAddress { get; set; }
}