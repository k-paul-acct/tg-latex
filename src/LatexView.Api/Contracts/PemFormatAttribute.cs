using System.Text.RegularExpressions;

namespace LatexView.Api.Contracts;

public sealed partial class PemFormatAttribute : NullHandlingPolicyValidationAttribute
{
    [GeneratedRegex(
        @"^-----BEGIN (?<label>[ 0-9A-Z]+)-----\r?\n" +
        @"(?<data>(?:[+/0-9A-Za-z]+={0,2}\r?\n)+)" +
        @"-----END \k<label>-----\r?$")]
    private static partial Regex PemRegex();

    protected override bool IsValidNotNull(object value)
    {
        if (value is not string s)
        {
            return false;
        }

        return PemRegex().IsMatch(s);
    }
}
