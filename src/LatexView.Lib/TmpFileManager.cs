namespace LatexView.Lib;

public sealed class TmpFileManager : IDisposable
{
    private static readonly string TmpPath = Path.GetTempPath();

    private readonly string _basePath;
    private bool _isInitialized;

    public TmpFileManager()
    {
        var name = GenerateRandomName();
        _basePath = Path.Combine(TmpPath, name);
        _isInitialized = false;
    }

    public string DirectoryPath => _basePath;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public string CreateFile(string content, string? extension = null)
    {
        EnsureTmpDirectory();
        var fileName = GenerateRandomName() + extension;
        var filePath = Path.Combine(_basePath, fileName);
        var options = new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.ReadWrite,
            Share = FileShare.None,
            UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite, // TODO: Windows support.
        };
        using var file = new FileStream(filePath, options);
        using var writer = new StreamWriter(file);
        writer.Write(content);
        return filePath;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    public FileStream CreateFile(string name)
    {
        EnsureTmpDirectory();
        var filePath = Path.Combine(_basePath, name);
        var options = new FileStreamOptions
        {
            Mode = FileMode.Create,
            Access = FileAccess.ReadWrite,
            Share = FileShare.None,
            UnixCreateMode = UnixFileMode.UserRead | UnixFileMode.UserWrite, // TODO: Windows support.
        };
        var file = new FileStream(filePath, options);
        return file;
    }

    public string CreateDirectory()
    {
        EnsureTmpDirectory();
        var name = GenerateRandomName();
        var path = Path.Combine(_basePath, name);
        Directory.CreateDirectory(path);
        return path;
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
