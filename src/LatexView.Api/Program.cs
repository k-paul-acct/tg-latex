using System.Net.Mime;
using LatexView.Api;
using LatexView.Api.Contracts.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<LatexProcessManager>();
builder.Services.AddTransient<TmpFileWorker>();

var app = builder.Build();

app.MapPost("/api/convert", async (
    ConvertRequest request,
    TmpFileWorker tmpFileWorker,
    LatexProcessManager processManager,
    CancellationToken cancellationToken) =>
{
    var content =
        $$$"""
        \documentclass[12pt, border={{{request.BorderWidth}}}pt]{standalone}

        \usepackage[english, russian]{babel}
            \babelfont{rm}{CMU Serif}
            \babelfont{sf}{CMU Sans Serif}
            \babelfont{tt}{CMU Typewriter Text}

        \usepackage{amsmath}
        \usepackage{amssymb}
        \usepackage{mathtools}
        \usepackage{icomma}
        \usepackage{nicefrac}
        \usepackage{nicematrix}
        \usepackage{relsize}
        \usepackage{array}

        \begin{document}
        $\displaystyle
            {{{request.Formula}}}
        $
        \end{document}

        """;

    try
    {
        var filePath = tmpFileWorker.CreateFile(content, ".tex");
        var pngPath = await processManager.ProcessFile(filePath, request.WhiteBackground, request.Ppi, cancellationToken);
        return Results.File(pngPath, MediaTypeNames.Image.Png);
    }
    catch (OperationCanceledException)
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }
    catch
    {
        return Results.BadRequest();
    }
});

app.Run();
