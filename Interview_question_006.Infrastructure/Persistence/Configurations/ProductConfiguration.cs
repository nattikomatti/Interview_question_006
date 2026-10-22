using Example.InterviewQuestion006.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Example.InterviewQuestion006.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products", t => t.HasCheckConstraint(
            "CK_Products_ProductCode",
             
            "LEN([ProductCode]) = 16 AND [ProductCode] COLLATE Latin1_General_BIN NOT LIKE '%[^A-Z0-9]%'"));

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseIdentityColumn();

        builder.Property(p => p.Code)
            .HasColumnName("ProductCode")
            .HasConversion(code => code.Value, value => ProductCode.FromPersisted(value))
            .HasMaxLength(ProductCode.Length)
            .IsFixedLength()
            .IsUnicode(false)
            .IsRequired();

        builder.HasIndex(p => p.Code).IsUnique();

        builder.Property(p => p.CreatedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("SYSUTCDATETIME()");
    }
}
