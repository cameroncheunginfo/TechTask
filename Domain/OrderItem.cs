namespace Domain;

public class OrderItem
{
    private OrderItem() {}

    public OrderItem(Order order,
                     Variant variant,
                     int quantity)
    {
        OrderId = order.OrderId;
        Order = order;

        VariantId = variant.VariantId;
        Variant = variant;

        Quantity = quantity;
    }

    public int OrderItemId { get; private set; }
    public int OrderId { get; private set; }
    public int VariantId { get; private set; }
    public int Quantity { get; set; }

    public Order Order { get; set; }
    public Variant Variant { get; set; }
}