namespace Client.Dtos;

public class UpdateOrderRequestDto
{
    public string OrderNumber { get; set; }
    public AddressDto ShippingAddress { get; set; }
    public ICollection<CreateOrderItemDto> OrderItems { get; set; }
}