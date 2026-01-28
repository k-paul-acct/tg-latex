namespace LatexView.Bot.Models;

public sealed class User
{
    public long Id { get; set; }
    public string? Username { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public DateTimeOffset CreatedTimestamp { get; set; }

    public ICollection<CompileRequest> CompileRequests { get; } = null!;
}
