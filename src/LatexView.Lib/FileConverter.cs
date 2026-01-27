using System.Data;
using System.Diagnostics;

namespace LatexView.Lib;

public sealed class FileConverter
{
    public async Task Convert(
        string inputPath,
        string outputPath,
        Options options,
        CancellationToken cancellationToken = default)
    {
        var startInfo = new ProcessStartInfo()
        {
            FileName = "convert",
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        if (options.Ppi != 0)
        {
            startInfo.ArgumentList.AddRange("-density", options.Ppi.ToString());
        }

        startInfo.ArgumentList.AddRange(
            inputPath,
            "-quality", "100");

        if (options.BackgroundColor is not null)
        {
            startInfo.ArgumentList.AddRange(
                "-background", options.BackgroundColor,
                "-alpha", "remove",
                "-alpha", "off");
        }

        startInfo.ArgumentList.Add(outputPath);

        using var process = Process.Start(startInfo) ??
                            throw new DataException("Failed to start 'convert' process.");

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
            throw new DataException($"Failed to convert '{inputPath}' to '{outputPath}'.");
        }
    }

    public sealed class Options
    {
        public int Ppi { get; init; }
        public string? BackgroundColor { get; init; }
    }
}
