using LatexView.Api.Contracts.Requests;
using Refit;

internal interface ILatexCompilerApi
{
    [Post("/api/compile/formula")]
    Task<HttpResponseMessage> CompileFormula(CompileFormulaRequest request);

    [Post("/api/compile/body")]
    Task<HttpResponseMessage> CompileBody(CompileBodyRequest request);

    [Post("/api/compile/document")]
    Task<HttpResponseMessage> CompileDocument(); // TODO

    [Post("/api/compile/git")]
    Task<HttpResponseMessage> CompileGit(CompileGitRequest request);
}
