namespace Domain;

public class Customer
{
    private Customer() {}

    public Customer(string  emailAddress,
                    string  name,
                    string? phoneNumber)
    {
        ExternalId = Guid.NewGuid();
        Email = emailAddress;
        Name = name;
        PhoneNumber = phoneNumber;
        Created = DateTime.UtcNow;
    }

    public int CustomerId { get; private init; }
    public Guid ExternalId { get; private init; }
    public string Email { get; private set; }
    public string Name { get; private set; }
    public string? PhoneNumber { get; private set; }
    public DateTime Created { get; private set; }

    public ICollection<Order> Orders { get; private set; }
}