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
        string remote,
        string? sshKeyPath = null,
        CancellationToken cancellationToken = default)
    {
        var outputDirectory = _tmpFileManager.CreateDirectory();
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        startInfo.ArgumentList.AddRange("clone", "--depth", "1", remote, outputDirectory);

        if (sshKeyPath is not null)
        {
            startInfo.Environment["GIT_SSH_COMMAND"] =
                $"ssh -i {sshKeyPath} -o IdentitiesOnly=yes -o StrictHostKeyChecking=no -o BatchMode=yes";
        }

        startInfo.Environment["GIT_TERMINAL_PROMPT"] = "0";

        using var process = Process.Start(startInfo) ??
                            throw new DataException("Failed to start 'git' process.");

        try
        {
            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            try
            {
                process.Kill(entireProcessTree: true);
            }
            catch
            {
            }

            throw;
        }

        if (process.ExitCode != 0)
        {
            throw new DataException($"Failed to clone git repository '{remote}'.");
        }

        return outputDirectory;
    }
}
