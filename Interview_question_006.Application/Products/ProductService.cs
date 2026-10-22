using Example.InterviewQuestion006.Application.Barcodes;
using Example.InterviewQuestion006.Domain.Products;

namespace Example.InterviewQuestion006.Application.Products;

public class ProductService(IProductRepository repository, IBarcodeGenerator barcodeGenerator)
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await repository.GetAllAsync(cancellationToken);
        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var code = ProductCode.Create(request.ProductCode);

        if (await repository.ExistsAsync(code, cancellationToken))
        {
            throw new ConflictException($"Product code {code.Formatted} already exists.");
        }

        var product = new Product(code);
        await repository.AddAsync(product, cancellationToken);
        return ToDto(product);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetRequiredAsync(id, cancellationToken);
        await repository.DeleteAsync(product, cancellationToken);
    }

    public async Task<string> GetBarcodeSvgAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await GetRequiredAsync(id, cancellationToken);
        return barcodeGenerator.GenerateSvg(product.Code.Formatted);
    }

    private async Task<Product> GetRequiredAsync(int id, CancellationToken cancellationToken) =>
        await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product {id} was not found.");

    private static ProductDto ToDto(Product product) =>
        new(product.Id, product.Code.Formatted, product.CreatedAt);
}
