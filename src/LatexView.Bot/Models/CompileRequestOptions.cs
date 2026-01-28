namespace LatexView.Bot.Models;

public sealed class CompileRequestOptions
{
    public static CompileRequestOptions Default { get; } = new();

    public bool UsePdfAVersion { get; set; }
}
