using System.Text.RegularExpressions;

namespace LatexView.Api.Contracts.Validation;

public sealed partial class GitRemoteAddressAttribute : NullHandlingPolicyValidationAttribute
{
    [GeneratedRegex(@"^(?:(?<user>[^\s@]+)@)?(?<host>[^\s:]+):(?<path>[^\s]+)$")]
    private static partial Regex ScpLikeRegex();

    protected override bool IsValidNotNull(object value)
    {
        if (value is not string s)
        {
            return false;
        }

        if (s.StartsWith("https://", StringComparison.Ordinal))
        {
            return true;
        }

        return ScpLikeRegex().IsMatch(s);
    }
}
