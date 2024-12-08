using Client.Dtos;
using Domain;

namespace Application.Orders.Mappers;

public class OrderDtoMapper : IOrderDtoMapper
{
    public OrderDto Map(Order order)
    {
        return new OrderDto
        {
            OrderNumber = order.OrderNumber,
            CustomerName = order.Customer.Name,
            PhoneNumber = order.Customer.PhoneNumber,
            BillingAddress = new AddressDto
            {
                AddressLineOne = order.BillingAddress.LineOne,
                AddressLineTwo = order.BillingAddress.LineTwo,
                AddressLineThree = order.BillingAddress.LineThree,
                PostCode = order.BillingAddress.PostCode,
            },
            ShippingAddress = new AddressDto
            {
                AddressLineOne = order.ShippingAddress.LineOne,
                AddressLineTwo = order.ShippingAddress.LineTwo,
                AddressLineThree = order.ShippingAddress.LineThree,
                PostCode = order.ShippingAddress.PostCode,
            },
            TotalCost = order.OrderItems.Sum(x => x.Quantity * x.Variant.Price),
            OrderItems = order.OrderItems.Select(x => new OrderItemDto
            {
                ProductId = x.Variant.Product.ProductId,
                ProductName = x.Variant.Product.Name,
                VariantId = x.Variant.VariantId,
                VariantName = x.Variant.Name,
                Sku = x.Variant.Sku,
                ImageUrl = x.Variant.Product.ImageUrl,
                Quantity = x.Quantity,
                UnitPrice = x.Variant.Price,
                TotalPrice = x.Variant.Price * x.Quantity

            }).ToList(),
        };
    }
}