using System.Xml.Linq;
using Example.InterviewQuestion006.Infrastructure.Barcodes;

namespace Example.InterviewQuestion006.Tests.Infrastructure;

public class Code39BarcodeGeneratorTests
{
    private const string AllowedProductChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-";

    [Fact]
    public void Every_pattern_has_nine_elements_with_three_wide_and_exactly_one_wide_space()
    {
        foreach (var (symbol, pattern) in Code39BarcodeGenerator.Patterns)
        {
            Assert.True(pattern.Length == 9, $"'{symbol}' length");
            Assert.True(pattern.Count(c => c == 'w') == 3, $"'{symbol}' wide count");

            var wideSpaces = pattern.Where((c, i) => i % 2 == 1 && c == 'w').Count();
            Assert.True(wideSpaces == 1, $"'{symbol}' wide spaces");
        }
    }

    [Fact]
    public void Patterns_are_unique()
    {
        var patterns = Code39BarcodeGenerator.Patterns.Values.ToList();

        Assert.Equal(patterns.Count, patterns.Distinct().Count());
    }

    [Theory] 
    [InlineData("1AKU")]
    [InlineData("2BLV")]
    [InlineData("3CMW")]
    [InlineData("4DNX")]
    [InlineData("5EOY")]
    [InlineData("6FPZ")]
    [InlineData("7GQ-")]
    [InlineData("8HR.")]
    [InlineData("9IS ")]
    [InlineData("0JT*")]
    public void Symbols_in_same_row_share_bar_widths_and_shift_wide_space(string row)
    {
        int[] expectedWideSpaceIndex = [3, 5, 7, 1];

        var barsOfFirst = Bars(Code39BarcodeGenerator.Patterns[row[0]]);
        for (var i = 0; i < row.Length; i++)
        {
            var pattern = Code39BarcodeGenerator.Patterns[row[i]];
            Assert.Equal(barsOfFirst, Bars(pattern));
            Assert.Equal('w', pattern[expectedWideSpaceIndex[i]]);
        }

        static string Bars(string pattern) => string.Concat(pattern.Where((_, i) => i % 2 == 0));
    }

    [Fact]
    public void Every_product_code_character_is_encodable()
    {
        Assert.All(AllowedProductChars, c => Assert.True(Code39BarcodeGenerator.Patterns.ContainsKey(c)));
    }

    [Theory]
    [InlineData('*', "100101101101")]
    [InlineData('0', "101001101101")]
    [InlineData('1', "110100101011")]
    [InlineData('A', "110101001011")]
    [InlineData('Z', "100110110101")]
    [InlineData('-', "100101011011")]
    public void Pattern_matches_reference_modules(char symbol, string expectedModules)
    {
        var pattern = Code39BarcodeGenerator.Patterns[symbol];

        var modules = string.Concat(pattern.Select((c, i) =>
            new string(i % 2 == 0 ? '1' : '0', c == 'w' ? 2 : 1)));

        Assert.Equal(expectedModules, modules);
    }

    [Fact]
    public void Encode_wraps_data_in_start_and_stop_symbols()
    {
        var elements = Code39BarcodeGenerator.Encode("ABCD-1234-EFGH-5678");

        
        Assert.Equal(21 * 9 + 20, elements.Count);
        Assert.True(elements[0].IsBar);
        Assert.True(elements[^1].IsBar);

        var start = elements.Take(9).Select(e => e.Width == 3 ? 'w' : 'n');
        var stop = elements.TakeLast(9).Select(e => e.Width == 3 ? 'w' : 'n');
        Assert.Equal(Code39BarcodeGenerator.Patterns['*'], string.Concat(start));
        Assert.Equal(Code39BarcodeGenerator.Patterns['*'], string.Concat(stop));
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("A*B")]
    [InlineData("A@B")]
    public void Encode_rejects_characters_outside_code39(string data)
    {
        Assert.Throws<ArgumentException>(() => Code39BarcodeGenerator.Encode(data));
    }

    [Fact]
    public void GenerateSvg_returns_well_formed_svg_with_one_rect_per_bar()
    {
        var svg = new Code39BarcodeGenerator().GenerateSvg("ABCD-1234-EFGH-5678");

        var document = XDocument.Parse(svg);
        XNamespace ns = "http://www.w3.org/2000/svg";
        Assert.Equal(ns + "svg", document.Root!.Name);

        var barRects = document.Root.Elements(ns + "rect").Count(r => (string?)r.Attribute("fill") == "#000");
        Assert.Equal(21 * 5, barRects);
    }
}
