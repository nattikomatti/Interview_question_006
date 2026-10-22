using System.Text;

namespace Example.InterviewQuestion006.Domain.Products;

 
public sealed record ProductCode
{
    public const int Length = 16;
    public const int GroupSize = 4;
    public const char Separator = '-';

    public string Value { get; }

    private ProductCode(string value) => Value = value;

   
    public static ProductCode Create(string? input)
    {
        if (!TryCreate(input, out var code, out var error))
        {
            throw new DomainValidationException(error!);
        }

        return code!;
    }

    public static bool TryCreate(string? input, out ProductCode? code, out string? error)
    {
        code = null;
        error = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Product code is required.";
            return false;
        }

        var trimmed = input.Trim();
        var isDashedFormat = trimmed.Length == Length + Length / GroupSize - 1;

        if (isDashedFormat && !HasSeparatorsInPlace(trimmed))
        {
            error = "Product code must be in the format XXXX-XXXX-XXXX-XXXX.";
            return false;
        }

        var raw = isDashedFormat ? trimmed.Replace(Separator.ToString(), string.Empty) : trimmed;

        if (raw.Length != Length)
        {
            error = $"Product code must be exactly {Length} characters.";
            return false;
        }

        if (!raw.All(IsAllowedChar))
        {
            error = "Product code may contain only digits (0-9) and uppercase letters (A-Z).";
            return false;
        }

        code = new ProductCode(raw);
        return true;
    }

   
    public static ProductCode FromPersisted(string value) => new(value);

    public string Formatted
    {
        get
        {
            var builder = new StringBuilder(Length + Length / GroupSize - 1);
            for (var i = 0; i < Value.Length; i++)
            {
                if (i > 0 && i % GroupSize == 0)
                {
                    builder.Append(Separator);
                }
                builder.Append(Value[i]);
            }
            return builder.ToString();
        }
    }

    public override string ToString() => Formatted;

    private static bool IsAllowedChar(char c) => c is >= '0' and <= '9' or >= 'A' and <= 'Z';

    private static bool HasSeparatorsInPlace(string dashed)
    {
        for (var i = 0; i < dashed.Length; i++)
        {
            var shouldBeSeparator = (i + 1) % (GroupSize + 1) == 0;
            if (shouldBeSeparator != (dashed[i] == Separator))
            {
                return false;
            }
        }
        return true;
    }
}
