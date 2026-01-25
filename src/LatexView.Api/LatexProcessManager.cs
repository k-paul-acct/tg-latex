using System.Data;
using System.Diagnostics;

namespace LatexView.Api;

public sealed class LatexProcessManager
{
    public async Task<string> ProcessFile(
        string filePath,
        bool whiteBackground,
        int ppi,
        CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30));

        var pdfPath = await ProcessLatex(filePath, cts.Token) ??
                      throw new DataException("Processing failed: PDF file was not generated.");
        var pngPath = await ProcessConvert(pdfPath, whiteBackground, ppi, cts.Token) ??
                      throw new DataException("Processing failed: PNG file was not generated.");

        return pngPath;
    }

    private static async Task<string?> ProcessLatex(
        string filePath,
        CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(filePath);
        var startInfo = new ProcessStartInfo
        {
            FileName = "lualatex",
            ArgumentList =
            {
                $"--output-directory={directory}",
                "--halt-on-error",
                filePath,
            },
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo) ??
                            throw new DataException("Failed to start lualatex process.");

        await process.WaitForExitAsync(cancellationToken);

        return process.ExitCode == 0 ? Path.ChangeExtension(filePath, ".pdf") : null;
    }

    private static async Task<string?> ProcessConvert(
        string filePath,
        bool whiteBackground,
        int ppi,
        CancellationToken cancellationToken)
    {
        var pngPath = Path.ChangeExtension(filePath, ".png");
        var background = whiteBackground ? "-background white -alpha remove -alpha off" : null;
        var startInfo = new ProcessStartInfo
        {
            FileName = "convert",
            Arguments = $"-density {ppi} {filePath} -quality 100 {background} {pngPath}",
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo) ??
                            throw new DataException("Failed to start convert process.");

        await process.WaitForExitAsync(cancellationToken);

        return process.ExitCode == 0 ? pngPath : null;
    }
}
