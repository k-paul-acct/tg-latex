namespace LatexView.Api.Contracts.Requests;

public record ConvertRequest(string Formula, bool WhiteBackground, int Ppi, decimal BorderWidth);