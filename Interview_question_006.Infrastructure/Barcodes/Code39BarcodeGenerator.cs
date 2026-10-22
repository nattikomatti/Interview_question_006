using System.Globalization;
using System.Text;
using Example.InterviewQuestion006.Application.Barcodes;

namespace Example.InterviewQuestion006.Infrastructure.Barcodes;

 
public class Code39BarcodeGenerator : IBarcodeGenerator
{
    public const char StartStop = '*';

    private const int NarrowWidth = 1;
    private const int WideWidth = 3;
    private const int QuietZone = 10 * NarrowWidth;
    private const int BarHeight = 40;

    public static readonly IReadOnlyDictionary<char, string> Patterns = new Dictionary<char, string>
    {
        ['0'] = "nnnwwnwnn", ['1'] = "wnnwnnnnw", ['2'] = "nnwwnnnnw", ['3'] = "wnwwnnnnn",
        ['4'] = "nnnwwnnnw", ['5'] = "wnnwwnnnn", ['6'] = "nnwwwnnnn", ['7'] = "nnnwnnwnw",
        ['8'] = "wnnwnnwnn", ['9'] = "nnwwnnwnn",
        ['A'] = "wnnnnwnnw", ['B'] = "nnwnnwnnw", ['C'] = "wnwnnwnnn", ['D'] = "nnnnwwnnw",
        ['E'] = "wnnnwwnnn", ['F'] = "nnwnwwnnn", ['G'] = "nnnnnwwnw", ['H'] = "wnnnnwwnn",
        ['I'] = "nnwnnwwnn", ['J'] = "nnnnwwwnn", ['K'] = "wnnnnnnww", ['L'] = "nnwnnnnww",
        ['M'] = "wnwnnnnwn", ['N'] = "nnnnwnnww", ['O'] = "wnnnwnnwn", ['P'] = "nnwnwnnwn",
        ['Q'] = "nnnnnnwww", ['R'] = "wnnnnnwwn", ['S'] = "nnwnnnwwn", ['T'] = "nnnnwnwwn",
        ['U'] = "wwnnnnnnw", ['V'] = "nwwnnnnnw", ['W'] = "wwwnnnnnn", ['X'] = "nwnnwnnnw",
        ['Y'] = "wwnnwnnnn", ['Z'] = "nwwnwnnnn",
        ['-'] = "nwnnnnwnw", ['.'] = "wwnnnnwnn", [' '] = "nwwnnnwnn",
        [StartStop] = "nwnnwnwnn",
    };

    public string GenerateSvg(string data)
    {
        var bars = Encode(data);
        var totalWidth = QuietZone * 2 + bars.Sum(b => b.Width);

        var svg = new StringBuilder();
        svg.Append(CultureInfo.InvariantCulture,
            $"<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 {totalWidth} {BarHeight}\" ");
        svg.Append(CultureInfo.InvariantCulture,
            $"width=\"{totalWidth * 2}\" height=\"{BarHeight}\" preserveAspectRatio=\"none\" shape-rendering=\"crispEdges\">");
        svg.Append(CultureInfo.InvariantCulture,
            $"<rect width=\"{totalWidth}\" height=\"{BarHeight}\" fill=\"#fff\"/>");

        var x = QuietZone;
        foreach (var (isBar, width) in bars)
        {
            if (isBar)
            {
                svg.Append(CultureInfo.InvariantCulture,
                    $"<rect x=\"{x}\" width=\"{width}\" height=\"{BarHeight}\" fill=\"#000\"/>");
            }
            x += width;
        }

        svg.Append("</svg>");
        return svg.ToString();
    }

 
    public static IReadOnlyList<(bool IsBar, int Width)> Encode(string data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var symbols = $"{StartStop}{data}{StartStop}";
        var elements = new List<(bool, int)>();

        for (var s = 0; s < symbols.Length; s++)
        {
            var symbol = symbols[s];
            if ((symbol == StartStop && s != 0 && s != symbols.Length - 1)
                || !Patterns.TryGetValue(symbol, out var pattern))
            {
                throw new ArgumentException($"Character '{symbol}' cannot be encoded in Code 39.", nameof(data));
            }

            if (s > 0)
            {
                elements.Add((false, NarrowWidth));  
            }

            for (var i = 0; i < pattern.Length; i++)
            {
                elements.Add((i % 2 == 0, pattern[i] == 'w' ? WideWidth : NarrowWidth));
            }
        }

        return elements;
    }
}
