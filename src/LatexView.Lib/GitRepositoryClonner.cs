using System.Data;
using System.Diagnostics;

namespace LatexView.Lib;

public sealed class GitRepositoryClonner
{
    private readonly TmpFileManager _tmpFileManager;

    public GitRepositoryClonner(TmpFileManager tmpFileManager)
    {
        _tmpFileManager = tmpFileManager;
    }

    public async Task<string> Clone(
        string uri,
        string? sshKeyPath = null,
        CancellationToken cancellationToken = default)
    {
        var outputDirectory = _tmpFileManager.CreateDirectory();
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = $"clone --depth 1 \"{uri}\" {outputDirectory}",
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (sshKeyPath is not null)
        {
            startInfo.Environment["GIT_SSH_COMMAND"] = $"ssh -i {sshKeyPath} -o IdentitiesOnly=yes -o StrictHostKeyChecking=no";
        }

        using var process = Process.Start(startInfo) ??
                            throw new DataException("Failed to start 'git' process.");

        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            throw new DataException($"Failed to clone git repository '{uri}'.");
        }

        return outputDirectory;
    }
}
