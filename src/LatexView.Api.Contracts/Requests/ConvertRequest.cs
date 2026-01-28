using LatexView.Api.Contracts.Validation;

namespace LatexView.Api.Contracts.Requests;

public sealed record CompileFormulaRequest(
    string Formula,
    [Color(AllowNull = true)] string? BackgroundColor,
    int Ppi,
    decimal BorderWidth);
