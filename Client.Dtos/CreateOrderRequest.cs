namespace Client.Dtos;

public class CreateOrderRequestDto
{
    public CustomerDto Customer { get; set; }

    public ICollection<CreateOrderItemDto> OrderItems { get; set; }
}