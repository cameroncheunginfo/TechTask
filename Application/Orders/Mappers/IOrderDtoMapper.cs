using Client.Dtos;
using Domain;

namespace Application.Orders.Mappers;

public interface IOrderDtoMapper
{
    public OrderDto Map(Order order);
}