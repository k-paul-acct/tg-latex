namespace LatexView.Api;

public sealed class TmpFileWorker : IDisposable
{
    private static readonly string TmpPath = Path.GetTempPath();

    private readonly string _basePath;
    private bool _isInitialized;

    public TmpFileWorker()
    {
        var name = GenerateRandomName();
        _basePath = Path.Combine(TmpPath, name);
        _isInitialized = false;
    }

    public string CreateFile(string content, string? extension = null)
    {
        EnsureTmpDirectory();
        var fileName = GenerateRandomName() + extension;
        var filePath = Path.Combine(_basePath, fileName);
        File.WriteAllText(filePath, content);
        return filePath;
    }

    public void Dispose()
    {
        if (_isInitialized)
        {
            Directory.Delete(_basePath, recursive: true);
        }
    }

    private void EnsureTmpDirectory()
    {
        if (_isInitialized)
        {
            return;
        }

        Directory.CreateDirectory(_basePath);
        _isInitialized = true;
    }

    private static string GenerateRandomName()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-0123456789_";

        Span<char> nameSpan = stackalloc char[16];

        var startIdx = Random.Shared.Next(0, 52);
        var endIdx = Random.Shared.Next(0, 52);

        Random.Shared.GetItems(chars, nameSpan);
        nameSpan[0] = chars[startIdx];
        nameSpan[^1] = chars[endIdx];

        return new string(nameSpan);
    }
}
