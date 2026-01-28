internal sealed class CompilationResult : IDisposable, IAsyncDisposable
{
    public Stream DocumentStream { get; }
    public string DocumentName { get; }

    public CompilationResult(Stream documentStream, string documentName)
    {
        DocumentStream = documentStream;
        DocumentName = documentName;
    }

    public void Dispose()
    {
        DocumentStream.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return DocumentStream.DisposeAsync();
    }
}
