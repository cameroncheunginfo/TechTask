using Client.Dtos;

namespace Application.Orders;

public interface IOrderUpdater
{
    OrderDto UpdateOrder(UpdateOrderRequestDto request);
}