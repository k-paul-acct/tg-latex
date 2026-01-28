using LatexView.Api.Contracts.Requests;
using LatexView.Bot.Data;

internal sealed class CompilerService
{
    private const string DefaultFileName = "file";

    private readonly AppDbContext _dbContext;
    private readonly ILatexCompilerApi _latexCompilerApi;

    public CompilerService(AppDbContext dbContext, ILatexCompilerApi latexCompilerApi)
    {
        _dbContext = dbContext;
        _latexCompilerApi = latexCompilerApi;
    }

    public async Task<CompilationResult> Compile(long userId, int messageId, string text)
    {
        // TODO: Add to DB.
        if (text.StartsWith('$') && text.EndsWith('$'))
        {
            var request = new CompileFormulaRequest(text, "white", 600, 1);
            using var response = await _latexCompilerApi.CompileFormula(request);
            response.EnsureSuccessStatusCode();
            var fileName = response.Content.Headers.ContentDisposition?.FileName ?? DefaultFileName;
            var length = response.Content.Headers.ContentLength ?? 0;
            var stream = new MemoryStream((int)length);
            await response.Content.CopyToAsync(stream);
            stream.Position = 0;
            return new CompilationResult(stream, fileName);
        }

        if (text.Contains(@"\documentclass") && text.Contains(@"\begin{document}") && text.Contains(@"\end{document}"))
        {
            // TODO: Compile document.
            throw new NotImplementedException();
        }

        // TODO: Compile body, git.
        throw new NotImplementedException();
    }

    public async Task<CompilationResult> CompileUpdate(long userId, int messageId, string text)
    {
        throw new NotImplementedException();
    }
}
