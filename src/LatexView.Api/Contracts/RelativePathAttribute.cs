namespace LatexView.Api.Contracts;

public sealed class RelativePathAttribute : NullHandlingPolicyValidationAttribute
{
    protected override bool IsValidNotNull(object value)
    {
        if (value is not string s)
        {
            return false;
        }

        return !Path.IsPathRooted(s);
    }
}
