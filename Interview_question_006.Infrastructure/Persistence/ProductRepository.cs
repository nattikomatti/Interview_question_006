using Example.InterviewQuestion006.Application;
using Example.InterviewQuestion006.Application.Products;
using Example.InterviewQuestion006.Domain.Products;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Example.InterviewQuestion006.Infrastructure.Persistence;

public class ProductRepository(AppDbContext db) : IProductRepository
{
     
    private static readonly int[] UniqueViolationErrors = [2601, 2627];

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Products.AsNoTracking().OrderBy(p => p.Id).ToListAsync(cancellationToken);

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExistsAsync(ProductCode code, CancellationToken cancellationToken = default) =>
        db.Products.AnyAsync(p => p.Code == code, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        db.Products.Add(product);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && UniqueViolationErrors.Contains(sql.Number))
        {
             
            db.Entry(product).State = EntityState.Detached;
            throw new ConflictException($"Product code {product.Code.Formatted} already exists.");
        }
    }

    public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
    {
        db.Products.Remove(product);
        await db.SaveChangesAsync(cancellationToken);
    }
}
