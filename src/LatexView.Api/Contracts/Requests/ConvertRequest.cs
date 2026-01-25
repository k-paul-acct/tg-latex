namespace LatexView.Api.Contracts.Requests;

public sealed record ConvertRequest(string Formula, bool WhiteBackground, int Ppi, decimal BorderWidth);
