using Client.Dtos;

namespace Application.Orders;

public interface IOrderCreator
{
    OrderDto CreateOrder(CreateOrderRequestDto request);
}