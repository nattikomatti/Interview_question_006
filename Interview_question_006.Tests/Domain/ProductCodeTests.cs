using Example.InterviewQuestion006.Domain;
using Example.InterviewQuestion006.Domain.Products;

namespace Example.InterviewQuestion006.Tests.Domain;

public class ProductCodeTests
{
    [Theory]
    [InlineData("ABCD-1234-EFGH-5678", "ABCD1234EFGH5678")]
    [InlineData("ABCD1234EFGH5678", "ABCD1234EFGH5678")]
    [InlineData("  0000-0000-0000-0000  ", "0000000000000000")]
    [InlineData("ZZZZZZZZZZZZZZZZ", "ZZZZZZZZZZZZZZZZ")]
    public void Create_accepts_valid_codes(string input, string expected)
    {
        var code = ProductCode.Create(input);

        Assert.Equal(expected, code.Value);
    }

    [Fact]
    public void Formatted_inserts_separator_every_four_characters()
    {
        var code = ProductCode.Create("ABCD1234EFGH5678");

        Assert.Equal("ABCD-1234-EFGH-5678", code.Formatted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abcd-1234-efgh-5678")]       // lowercase
    [InlineData("ABCD-1234-EFGH-567")]        // too short
    [InlineData("ABCD-1234-EFGH-56789")]      // too long
    [InlineData("ABCD12345EFGH5678")]         // 17 raw chars
    [InlineData("ABC-D1234-EFGH-5678")]       // separator in wrong place
    [InlineData("ABCD_1234_EFGH_5678")]       // wrong separator
    [InlineData("ABCD-1234-EFGH-567@")]       // symbol
    [InlineData("ABCD-1234-ÉFGH-5678")]       // non-English letter
    [InlineData("ABCD-12 4-EFGH-5678")]       // inner space
    public void Create_rejects_invalid_codes(string? input)
    {
        Assert.Throws<DomainValidationException>(() => ProductCode.Create(input));
    }

    [Fact]
    public void Codes_with_same_value_are_equal()
    {
        Assert.Equal(ProductCode.Create("ABCD-1234-EFGH-5678"), ProductCode.Create("ABCD1234EFGH5678"));
    }
}
