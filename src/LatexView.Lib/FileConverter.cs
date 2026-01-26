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
        var density = options.Ppi != 0
            ? $"-density {options.Ppi}"
            : null;
        var background = options.BackgroundColor is not null
            ? $"-background {options.BackgroundColor} -alpha remove -alpha off"
            : null;
        var startInfo = new ProcessStartInfo
        {
            FileName = "convert",
            Arguments = $"{density} {inputPath} -quality 100 {background} {outputPath}",
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo) ??
                            throw new DataException("Failed to start 'convert' process.");

        await process.WaitForExitAsync(cancellationToken);

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
