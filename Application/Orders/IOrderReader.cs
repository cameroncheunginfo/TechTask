using Client.Dtos;

namespace Application.Orders;

public interface IOrderReader
{
    OrderDto ReadOrder(string orderNumber);
}