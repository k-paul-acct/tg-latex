namespace LatexView.Api.Contracts.Requests;

public sealed record CompileFormulaRequest(string Formula, [Color] string? BackgroundColor, int Ppi, decimal BorderWidth);
