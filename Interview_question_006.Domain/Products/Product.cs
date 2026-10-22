namespace Example.InterviewQuestion006.Domain.Products;

public class Product
{
    public int Id { get; private set; }
    public ProductCode Code { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    
    private Product() { }

    public Product(ProductCode code)
    {
        Code = code;
        CreatedAt = DateTime.UtcNow;
    }
}
