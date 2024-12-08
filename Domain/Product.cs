namespace Domain;

public class Product
{
    private Product() {}

    public Product(string name,
                   string imageUrl)
    {
        ExternalId = Guid.NewGuid();
        Name = name;
        ImageUrl = imageUrl;
    }

    public int ProductId { get; set; /* trade off for seeding */ }
    public Guid ExternalId { get; private set; }
    public string Name { get; private set; }
    public string ImageUrl { get; private set; }

    public ICollection<Variant> Variants { get; private set; }
}