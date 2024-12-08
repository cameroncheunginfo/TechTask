using Application.Orders.Mappers;
using Application.Outbox;
using Client.Dtos;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class OrderUpdater(
    IRepository<Order> orderRepo,
    IRepository<Variant> variantRepo,
    IRepository<OutboxMessage> outboxRepo,
    IOutboxMessageCreator outboxMessageCreator,
    IOutboxMessageSender outboxMessageSender,
    IUnitOfWork unitOfWork)
    : IOrderUpdater
{
    private readonly IRepository<Order> _orderRepo = orderRepo;
    private readonly IRepository<Variant> _variantRepo = variantRepo;
    private readonly IRepository<OutboxMessage> _outboxRepo = outboxRepo;
    private readonly IOutboxMessageCreator _outboxMessageCreator = outboxMessageCreator;
    private readonly IOutboxMessageSender _outboxMessageSender = outboxMessageSender;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public OrderDto UpdateOrder(UpdateOrderRequestDto request)
    {
        // Validate request

        var order = _orderRepo.Get(x => x.OrderNumber == request.OrderNumber)
                              // Potentially look at using AsSplitQuery() for better performance
                              .Include(x => x.BillingAddress)
                              .Include(x => x.OrderItems)
                                .ThenInclude(x => x.Variant)
                                .ThenInclude(x => x.Product)
                              .Include(x => x.Customer)
                              .Include(x => x.ShippingAddress)
                              .SingleOrDefault()
            ;

        // Throw error if order is not found

        var skus = request.OrderItems.Select(x => x.Sku).ToList();
        var variants = _variantRepo.Get(x => skus.Contains(x.Sku))
                                   .Include(i => i.Product)
                                   .ToList();

        // Validate all SKUs in request are found

        var orderItems = variants.Join(request.OrderItems,
                                       var => var.Sku,
                                       item => item.Sku,
                                       (variant, item) => new
                                       {
                                           variant,
                                           item
                                       })
                                 .ToDictionary(x => x.variant, x => x.item.Quantity);

        order!.UpdateItems(orderItems);
        order.UpdateShippingAddress(request.ShippingAddress.AddressLineOne,
                                    request.ShippingAddress.AddressLineTwo!,
                                    request.ShippingAddress.AddressLineThree!,
                                    request.ShippingAddress.PostCode);

        /* If we're suppose to be using the transactional outbox pattern then 
         * this line needs to be remove so that the outbox message is created in the same transaction
         * */
        _unitOfWork.Save();

        var outboxMessage = outboxMessageCreator.Create<Order>(order);

        _outboxRepo.Insert(outboxMessage);

        _unitOfWork.Save();

        _outboxMessageSender.Send(outboxMessage);

        return new OrderDtoMapper().Map(order);
    }
}

public class OrderReader(
    IRepository<Order> orderRepo,
    IUnitOfWork unitOfWork)
    : IOrderReader
{
    private readonly IRepository<Order> _orderRepo = orderRepo;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public OrderDto ReadOrder(string orderNumber)
    {
        var order = _orderRepo.Get(x => x.OrderNumber == orderNumber)
                              .Include(x => x.OrderItems)
                                .ThenInclude(x => x.Variant)
                                .ThenInclude(x => x.Product) // Split query - requires profiling first
                              .Include(x => x.Customer)
                              .Include(x => x.ShippingAddress)
                              .Include(x => x.BillingAddress)
                              .SingleOrDefault()
            ;

        // This does not need to be saved, query can also be AsNoTracking
        _unitOfWork.Save();

        return new OrderDtoMapper().Map(order);
    }
}