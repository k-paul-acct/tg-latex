using System.Text.RegularExpressions;

namespace LatexView.Api.Contracts.Validation;

public sealed partial class ColorAttribute : NullHandlingPolicyValidationAttribute
{
    [GeneratedRegex("^#(?:[0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$")]
    private static partial Regex ColorRegex();

    protected override bool IsValidNotNull(object value)
    {
        if (value is not string s)
        {
            return false;
        }

        if (s is "white" or "black")
        {
            return true;
        }

        return ColorRegex().IsMatch(s);
    }
}
