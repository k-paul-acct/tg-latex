using System.Net.Mime;
using LatexView.Api;
using LatexView.Api.Contracts.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<LatexProcessManager>();

var app = builder.Build();

var fileWorker = new FileWorker("/dev/shm/");

app.MapPost("/api/convert", async (ConvertRequest request, LatexProcessManager processManager) =>
{
    IResult result;
    var content = $$$"""
                     \documentclass[12pt, border={{{request.BorderWidth}}}pt]{standalone}

                     \usepackage[english, russian]{babel}
                     \usepackage{fontspec}
                     \setmainfont{TeX Gyre Schola}
                     \usepackage{newunicodechar}

                     % math
                     \usepackage{amsmath}
                     \usepackage{amssymb}
                     \usepackage{mathtools}
                     \usepackage{icomma}
                     \usepackage{nicefrac}
                     \usepackage{nicematrix}
                     \usepackage{relsize}
                     \usepackage{array}

                     % math
                     \makeatletter
                     \newcommand*\rel@kern[1]{\kern#1\dimexpr\macc@kerna}
                     \newcommand*\widebar[1]{
                     \begingroup
                     \def\mathaccent##1##2{
                     \rel@kern{0.8}
                     \overline{\rel@kern{-0.8}\macc@nucleus\rel@kern{0.2}}
                     \rel@kern{-0.2}
                     }
                     \macc@depth\@ne
                     \let\math@bgroup\@empty \let\math@egroup\macc@set@skewchar
                     \mathsurround\z@ \frozen@everymath{\mathgroup\macc@group\relax}
                     \macc@set@skewchar\relax
                     \let\mathaccentV\macc@nested@a
                     \macc@nested@a\relax111{#1}
                     \endgroup
                     }
                     \makeatother

                     \DeclareMathOperator{\arcsec}{arcsec}
                     \DeclareMathOperator{\arccosec}{arccosec}
                     \DeclareMathOperator{\sgn}{sgn}
                     \DeclareMathOperator{\const}{const}

                     \begin{document}
                         $\displaystyle {{{request.Formula}}}$
                     \end{document}
                     """;

    Console.WriteLine(content);
    
    var filePath = fileWorker.CreateFile(content, ".tex");

    try
    {
        var pngPath = await processManager.ProcessFile(filePath, request.WhiteBackground, request.Ppi);
        var bytes = File.ReadAllBytes(pngPath);

        result = Results.File(bytes, MediaTypeNames.Image.Png);
    }
    catch
    {
        result = Results.BadRequest();
    }

    processManager.DeleteDirectory(filePath);

    return result;
});

app.Run();