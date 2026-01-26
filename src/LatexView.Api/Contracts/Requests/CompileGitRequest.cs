namespace LatexView.Api.Contracts.Requests;

// TODO: Sanitize.
public sealed record CompileGitRequest(string Uri, string? SshKey = null);
