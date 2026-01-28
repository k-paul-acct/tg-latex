using LatexView.Api.Contracts.Validation;

namespace LatexView.Api.Contracts.Requests;

public sealed record CompileGitRequest(
    [GitRemoteAddress] string Remote,
    [PemFormat(AllowNull = true)] string? SshKey = null,
    [RelativePath(AllowNull = true)] string? MainPath = null);
