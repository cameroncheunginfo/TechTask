using Application.Addresses;
using Application.Customers;
using Application.Exceptions;
using Application.Orders.Mappers;
using Application.Orders.Validators;
using Application.Outbox;
using Client.Dtos;
using DataAccess;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Orders;

public class OrderCreator(
    ICustomerProvider customerProvider,
    IAddressProvider addressProvider,
    ICreateOrderRequestValidator requestValidator,
    IRepository<Order> orderRepo,
    IRepository<Variant> variantRepo,
    IRepository<OutboxMessage> outboxRepo,
    IOutboxMessageCreator outboxMessageCreator,
    IOutboxMessageSender outboxMessageSender,
    IUnitOfWork unitOfWork)
    : IOrderCreator
{
    private readonly ICustomerProvider customerProvider = customerProvider;
    private readonly IAddressProvider addressProvider = addressProvider;
    private readonly IRepository<Order> _orderRepo = orderRepo;
    private readonly IRepository<OutboxMessage> _outboxRepo = outboxRepo;
    private readonly IOutboxMessageCreator _outboxMessageCreator = outboxMessageCreator;
    private readonly IOutboxMessageSender _outboxMessageSender = outboxMessageSender;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public OrderDto CreateOrder(CreateOrderRequestDto request)
    {
        // Potentially use a DB transaction through the UnitOfWork

        var customer = customerProvider.GetCustomer(request.Customer);
        var addresses = addressProvider.GetAddresses(request.Customer.BillingAddress,
                                                     request.Customer.ShippingAddress);

        // Missing validation for <= 0 quantity and empty SKU on CreateOrderItemDto.
        // Not sure if you need to have validation that all SKUs are unique.
        // I would just pass the entire CreateOrderRequestDto object into the validator.

        /* This validation for the customer and address happens AFTER trying to get/create them from the DB. Better to move this to the very top of this method
         * and validate the request instead of what we get from the database.
         */
        if (!requestValidator.TryValidate(customer, addresses.Item1, addresses.Item2, out var errors))
            throw new ValidationException("Validation failed", errors);

        var order = new Order(customer,
                              addresses.Item1,
                              addresses.Item2);

        _orderRepo.Insert(order);

        var products = GetProducts(request.OrderItems);

        order.UpdateItems(products);

        /* If we're suppose to be using the transactional outbox pattern then 
         * this line needs to be remove so that the outbox message is created in the same transaction
         */
        _unitOfWork.Save();

        var outboxMessage = outboxMessageCreator.Create<Order>(order);

        _outboxRepo.Insert(outboxMessage);

        _unitOfWork.Save();

        _outboxMessageSender.Send(outboxMessage);

        return new OrderDtoMapper().Map(order);
    }

    private IDictionary<Variant, int> GetProducts(ICollection<CreateOrderItemDto> items)
    {
        var variants = new HashSet<Variant>();
        foreach (var item in items)
        {
            // This is doing a DB request for each item in the collection, would be better moving outside of the for loop and using a dictionary
            var variant = variantRepo.Get(x => x.Sku == item.Sku)
                                     .Include(i => i.Product)
                                     .SingleOrDefault();

            if (variant is not null)
            {
                variants.Add(variant);
            }
        }

        var requestedSkus = items.Select(x => x.Sku);

        var missingSkus = requestedSkus.ExceptBy(variants.Select(x => x.Sku), sku => sku).ToList();

        if (missingSkus.Any())
            throw new ValidationException("Request failed validation",
                                          new Dictionary<string, string>()
                                          {
                                              [nameof(missingSkus)] = string.Join(',', missingSkus)
                                          });

        return variants.Join(items,
                             x => x.Sku.ToUpperInvariant(),
                             x => x.Sku.ToUpperInvariant(),
                             (variant,
                              requestItem) => new
                              {
                                  variant,
                                  requestItem
                              })
                       .ToDictionary(x => x.variant, x => x.requestItem.Quantity);
    }
}