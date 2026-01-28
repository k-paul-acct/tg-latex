namespace LatexView.Bot.Models;

public sealed class CompileRequest
{
    public long UserId { get; set; }
    public long MessageId { get; set; }
    public long ResultMessageId { get; set; }
    public DateTimeOffset CreatedTimestamp { get; set; }
    public DateTimeOffset UpdatedTimestamp { get; set; }
    public CompileRequestOptions Options { get; set; } = CompileRequestOptions.Default;

    public User User { get; set; } = null!;
}
