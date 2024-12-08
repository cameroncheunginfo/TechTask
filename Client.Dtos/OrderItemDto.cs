namespace Client.Dtos;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int VariantId { get; set; }
    public string VariantName { get; set; }
    public string Sku { get; set; }
    public string ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}