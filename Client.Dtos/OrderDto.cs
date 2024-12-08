namespace Client.Dtos;

public class OrderDto
{
    public string OrderNumber { get; set; }
    public string CustomerName { get; set; }
    public string? PhoneNumber { get; set; }
    public AddressDto BillingAddress { get; set; }
    public AddressDto ShippingAddress { get; set; }
    public decimal TotalCost { get; set; }

    public ICollection<OrderItemDto> OrderItems { get; set; }
}