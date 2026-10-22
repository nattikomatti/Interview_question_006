using Example.InterviewQuestion006.Application;
using Example.InterviewQuestion006.Application.Barcodes;
using Example.InterviewQuestion006.Application.Products;
using Example.InterviewQuestion006.Domain;
using Example.InterviewQuestion006.Domain.Products;

namespace Example.InterviewQuestion006.Tests.Application;

public class ProductServiceTests
{
    private readonly InMemoryProductRepository _repository = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_repository, new FakeBarcodeGenerator());
    }

    [Fact]
    public async Task CreateAsync_stores_product_and_returns_formatted_code()
    {
        var dto = await _service.CreateAsync(new CreateProductRequest("ABCD1234EFGH5678"));

        Assert.Equal("ABCD-1234-EFGH-5678", dto.ProductCode);
        Assert.Single(await _service.GetAllAsync());
    }

    [Fact]
    public async Task CreateAsync_rejects_duplicate_code()
    {
        await _service.CreateAsync(new CreateProductRequest("ABCD-1234-EFGH-5678"));

        await Assert.ThrowsAsync<ConflictException>(() =>
            _service.CreateAsync(new CreateProductRequest("ABCD1234EFGH5678")));
    }

    [Fact]
    public async Task CreateAsync_rejects_invalid_code()
    {
        await Assert.ThrowsAsync<DomainValidationException>(() =>
            _service.CreateAsync(new CreateProductRequest("abcd-1234")));
    }

    [Fact]
    public async Task DeleteAsync_removes_product()
    {
        var dto = await _service.CreateAsync(new CreateProductRequest("ABCD-1234-EFGH-5678"));

        await _service.DeleteAsync(dto.Id);

        Assert.Empty(await _service.GetAllAsync());
    }

    [Fact]
    public async Task DeleteAsync_throws_when_product_missing()
    {
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(42));
    }

    [Fact]
    public async Task GetBarcodeSvgAsync_encodes_formatted_code()
    {
        var dto = await _service.CreateAsync(new CreateProductRequest("ABCD1234EFGH5678"));

        var svg = await _service.GetBarcodeSvgAsync(dto.Id);

        Assert.Equal("svg:ABCD-1234-EFGH-5678", svg);
    }

    private sealed class FakeBarcodeGenerator : IBarcodeGenerator
    {
        public string GenerateSvg(string data) => $"svg:{data}";
    }

    private sealed class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products = [];
        private int _nextId = 1;

        public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Product>>(_products.ToList());

        public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
            Task.FromResult(_products.FirstOrDefault(p => p.Id == id));

        public Task<bool> ExistsAsync(ProductCode code, CancellationToken cancellationToken = default) =>
            Task.FromResult(_products.Any(p => p.Code == code));

        public Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
             
            typeof(Product).GetProperty(nameof(Product.Id))!.SetValue(product, _nextId++);
            _products.Add(product);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
        {
            _products.Remove(product);
            return Task.CompletedTask;
        }
    }
}
