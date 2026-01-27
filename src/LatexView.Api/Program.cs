using System.Net.Mime;
using LatexView.Api.Contracts.Requests;
using LatexView.Lib;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CompilerOptions>(builder.Configuration.GetSection("CompilerOptions"));

builder.Services.AddSingleton(new LatexTemplateProvider());
builder.Services.AddScoped<GitRepositoryClonner>();
builder.Services.AddScoped<LatexCompiler>();
builder.Services.AddScoped<TmpFileManager>();
builder.Services.AddScoped<FileConverter>();

builder.Services.AddValidation();

var app = builder.Build();

app.MapPost("/api/compile/formula", async (
    CompileFormulaRequest request,
    LatexCompiler compiler,
    FileConverter converter,
    ILogger<Program> logger,
    IOptionsMonitor<CompilerOptions> options,
    CancellationToken cancellationToken) =>
{
    try
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(options.CurrentValue.Timeout);

        var compilerOptions = new LatexCompiler.Options { BorderWidth = request.BorderWidth };
        var pdfPath = await compiler.CompileFromTextAsFormula(request.Formula, compilerOptions, cts.Token);
        var converterOptions = new FileConverter.Options { BackgroundColor = request.BackgroundColor, Ppi = request.Ppi };
        var pngPath = Path.ChangeExtension(pdfPath, ".png");
        await converter.Convert(pdfPath, pngPath, converterOptions, cts.Token);
        return Results.File(pngPath, MediaTypeNames.Image.Png);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception e)
    {
        logger.LogError(e, message: null);
        return Results.BadRequest();
    }
});

app.MapPost("/api/compile/body", async (
    CompileBodyRequest request,
    LatexCompiler compiler,
    ILogger<Program> logger,
    IOptionsMonitor<CompilerOptions> options,
    CancellationToken cancellationToken) =>
{
    try
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(options.CurrentValue.Timeout);

        var pdfPath = await compiler.CompileFromTextAsBody(request.Body, LatexCompiler.Options.Default, cts.Token);
        return Results.File(pdfPath, MediaTypeNames.Application.Pdf);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception e)
    {
        logger.LogError(e, message: null);
        return Results.BadRequest();
    }
});

app.MapPost("/api/compile/git", async (
    CompileGitRequest request,
    TmpFileManager tmpFileManager,
    GitRepositoryClonner clonner,
    LatexCompiler compiler,
    ILogger<Program> logger,
    IOptionsMonitor<CompilerOptions> options,
    CancellationToken cancellationToken) =>
{
    try
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(options.CurrentValue.Timeout);

        var sshKeyPath = request.SshKey is not null
            ? tmpFileManager.CreateFile(request.SshKey, extension: null)
            : null;
        var repoPath = await clonner.Clone(request.Remote, sshKeyPath, cancellationToken);
        var pdfPath = await compiler.CompileProject(repoPath, request.MainPath, cts.Token);
        return Results.File(pdfPath, MediaTypeNames.Application.Pdf);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception e)
    {
        logger.LogError(e, message: null);
        return Results.BadRequest();
    }
});

app.Run();
