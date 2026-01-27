using System.Data;
using System.Diagnostics;

namespace LatexView.Lib;

public sealed class LatexCompiler
{
    private readonly TmpFileManager _tmpFileManager;
    private readonly LatexTemplateProvider _temlpateProvider;

    public LatexCompiler(TmpFileManager tmpFileManager, LatexTemplateProvider temlpateProvider)
    {
        _tmpFileManager = tmpFileManager;
        _temlpateProvider = temlpateProvider;
    }

    public Task<string> CompileFromText(
        string text,
        CancellationToken cancellationToken = default)
    {
        var texPath = _tmpFileManager.CreateFile(text, ".tex");
        return Compile(texPath, cancellationToken);
    }

    public Task<string> CompileProject(
        string projectPath,
        string? mainPath = null,
        CancellationToken cancellationToken = default)
    {
        var searchPath = mainPath is null ? projectPath : Path.Combine(projectPath, mainPath);

        foreach (var file in Directory.EnumerateFiles(searchPath, "*.tex", SearchOption.AllDirectories))
        {
            if (Path.GetFileName(file) == "main.tex")
            {
                return Compile(file, cancellationToken);
            }
        }

        throw new DataException($"The 'main.tex' file was not found in the project '{projectPath}'.");
    }

    public Task<string> CompileFromTextAsFormula(
        string formula,
        Options options,
        CancellationToken cancellationToken = default)
    {
        var parameters = options.ToParametersDictionary();
        parameters["Formula"] = formula;
        var text = _temlpateProvider.GetTemplate("formula").GetText(parameters);
        return CompileFromText(text, cancellationToken);
    }

    public Task<string> CompileFromTextAsBody(
        string body,
        Options options,
        CancellationToken cancellationToken = default)
    {
        WriteBody();
        var parameters = options.ToParametersDictionary();
        var text = _temlpateProvider.GetTemplate("body").GetText(parameters);
        return CompileFromText(text, cancellationToken);

        void WriteBody()
        {
            using var inputFile = _tmpFileManager.CreateFile("text.txt");
            using var writer = new StreamWriter(inputFile);

            foreach (var c in body)
            {
                switch (c)
                {
                    case '\\':
                        writer.Write("\\textbackslash{}");
                        break;
                    case '{' or '}' or '$' or '&' or '#' or '%' or '_' or '^' or '~':
                        writer.Write('\\');
                        writer.Write(c);
                        if (c is '^' or '~')
                        {
                            writer.Write("{}");
                        }
                        break;
                    default:
                        writer.Write(c);
                        break;
                }
            }
        }
    }

    private static async Task<string> Compile(
        string filePath,
        CancellationToken cancellationToken)
    {
        var dirPath = Path.GetDirectoryName(filePath) ?? "./";
        var startInfo = new ProcessStartInfo
        {
            FileName = "lualatex",
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = dirPath,
        };

        startInfo.ArgumentList.AddRange(
            $"--output-directory={dirPath}", "--no-shell-escape", "--halt-on-error", "--interaction=batchmode", filePath);

        using var process = Process.Start(startInfo) ??
                            throw new DataException("Failed to start 'lualatex' process.");

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
            throw new DataException($"Failed to compile LaTeX file '{filePath}'.");
        }

        return Path.Combine(dirPath, Path.ChangeExtension(filePath, ".pdf"));
    }

    public sealed class Options
    {
        public static Options Default { get; } = new Options();

        public decimal BorderWidth { get; init; }

        public Dictionary<string, string> ToParametersDictionary()
        {
            return new Dictionary<string, string>
            {
                {nameof(BorderWidth), BorderWidth.ToString()},
            };
        }
    }
}
