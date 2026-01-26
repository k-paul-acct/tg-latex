using System.Text;
using System.Text.RegularExpressions;

namespace LatexView.Lib;

public sealed partial class LatexTemplate
{
    [GeneratedRegex("<!--[A-Za-z_]+-->")]
    private static partial Regex ParameterRegex();

    private readonly Func<Dictionary<string, string>, string> _createFunc;

    public LatexTemplate(ReadOnlySpan<char> template)
    {
        var collectPartsAction = static (Dictionary<string, string> _, StringBuilder _) => { };
        var lastIdx = 0;

        foreach (var match in ParameterRegex().EnumerateMatches(template))
        {
            var part = template[lastIdx..match.Index].ToString();
            var key = template.Slice(match.Index + 4, match.Length - 7).ToString();
            lastIdx = match.Index + match.Length;
            collectPartsAction = Continuation(collectPartsAction, part, key);
        }

        var lastPart = template[lastIdx..].ToString();
        collectPartsAction = Continuation(collectPartsAction, lastPart);

        _createFunc = (parameters) =>
        {
            var builder = new StringBuilder();
            collectPartsAction(parameters, builder);
            return builder.ToString();
        };
    }

    private static Action<Dictionary<string, string>, StringBuilder> Continuation(
        Action<Dictionary<string, string>, StringBuilder> current,
        string part,
        string? key = null)
    {
        if (key is null)
        {
            return (parameters, builder) =>
            {
                current(parameters, builder);
                builder.Append(part);
            };
        }

        return (parameters, builder) =>
        {
            current(parameters, builder);
            builder.Append(part);
            builder.Append(parameters[key]);
        };
    }

    public string GetText(Dictionary<string, string> parameters)
    {
        return _createFunc(parameters);
    }
}
