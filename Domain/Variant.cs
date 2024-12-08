namespace Domain;

public class Variant
{
    private Variant(){}

    public Variant(Product product,
                   string name,
                   string sku,
                   decimal price)
    {
        ExternalId = Guid.NewGuid();
        
        ProductId = product.ProductId;
        
        Name = name;
        Sku = sku;
        Price = price;
    }

    public int VariantId { get; set; /* trade off for seeding */}
    public Guid ExternalId { get; private set; }
    public int ProductId { get; private set; }
    public string Name { get; private set; }
    public string Sku { get; private set; }
    public decimal Price { get; private set; }

    public Product Product { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; }
}