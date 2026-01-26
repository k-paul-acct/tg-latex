using System.Net.Mime;
using LatexView.Api.Contracts.Requests;
using LatexView.Lib;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new LatexTemplateProvider());
builder.Services.AddTransient<LatexCompiler>();
builder.Services.AddTransient<TmpFileManager>();
builder.Services.AddTransient<FileConverter>();

builder.Services.AddValidation();

var app = builder.Build();

app.MapPost("/api/compile/formula", async (
    CompileFormulaRequest request,
    LatexCompiler compiler,
    FileConverter converter,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    try
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30));

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
    CancellationToken cancellationToken) =>
{
    try
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30));

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

app.Run();
