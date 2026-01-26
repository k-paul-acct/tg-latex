using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LatexView.Api.Contracts;

public sealed partial class ColorAttribute : ValidationAttribute
{
    [GeneratedRegex("^#(?:[0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$")]
    private static partial Regex ColorRegex();

    public override bool IsValid(object? value)
    {
        return value switch
        {
            string s => ValidateColor(s),
            null => true,
            _ => false,
        };
    }

    private static bool ValidateColor(string s)
    {
        if (s is "white" or "black")
        {
            return true;
        }

        return ColorRegex().IsMatch(s);
    }
}
