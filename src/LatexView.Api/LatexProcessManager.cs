using System.Data;
using System.Diagnostics;

namespace LatexView.Api;

public sealed class LatexProcessManager : IDisposable
{
    private readonly CancellationTokenSource _cts = new(TimeSpan.FromSeconds(10));

    public void Dispose()
    {
        _cts.Dispose();
    }

    public async Task<string> ProcessFile(string filePath, bool whiteBackground, int ppi)
    {
        var pdfPath = await ProcessLatex(filePath);
        var pngPath = await ProcessConvert(pdfPath, whiteBackground, ppi);

        if (!Path.Exists(pngPath))
        {
            throw new DataException("Processing failed: PNG file was not generated");
        }

        return pngPath;
    }

    private async Task<string> ProcessLatex(string filePath)
    {
        using var proc = new Process();
        var directory = Path.GetDirectoryName(filePath);

        proc.StartInfo.FileName = "lualatex";
        proc.StartInfo.Arguments = $"-shell-escape --output-format=pdf -output-directory={directory} {filePath}";

        proc.Start();

        await proc.WaitForExitAsync(_cts.Token);

        return Path.ChangeExtension(filePath, ".pdf");
    }

    private async Task<string> ProcessConvert(string filePath, bool whiteBackground, int ppi)
    {
        using var proc = new Process();
        var pngPath = Path.ChangeExtension(filePath, ".png");
        var background = whiteBackground ? "-background white -alpha remove -alpha off" : null;

        proc.StartInfo.FileName = "convert";
        proc.StartInfo.Arguments =
            $"-density {ppi} -units PixelsPerInch {filePath} -quality 100 {background} {pngPath}";

        proc.Start();

        await proc.WaitForExitAsync(_cts.Token);

        return pngPath;
    }

    public void DeleteDirectory(string filePath)
    {
        var path = Path.GetDirectoryName(filePath) ?? throw new InvalidOperationException();
        Directory.Delete(path, true);
    }
}
