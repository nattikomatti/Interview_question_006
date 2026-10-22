namespace Example.InterviewQuestion006.Application.Products;

public record ProductDto(int Id, string ProductCode, DateTime CreatedAt);

public record CreateProductRequest(string? ProductCode);
