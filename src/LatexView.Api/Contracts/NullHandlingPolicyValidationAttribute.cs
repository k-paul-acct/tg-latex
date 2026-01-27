using System.ComponentModel.DataAnnotations;

namespace LatexView.Api.Contracts;

public abstract class NullHandlingPolicyValidationAttribute : ValidationAttribute
{
    public bool AllowNull { get; init; }

    protected abstract bool IsValidNotNull(object value);

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return AllowNull;
        }

        return IsValidNotNull(value);
    }
}
