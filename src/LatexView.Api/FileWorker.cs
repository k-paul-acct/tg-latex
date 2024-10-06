namespace LatexView.Api;

public class FileWorker
{
    private readonly string _basePath;

    public FileWorker(string basePath)
    {
        _basePath = basePath;
    }

    public string CreateFile(string content, string extension)
    {
        var folderPath = CreateTmpFolder();
        var fileName = GenerateRandomName() + extension;
        var filePath = Path.Combine(folderPath, fileName);

        File.WriteAllText(filePath, content);

        return filePath;
    }

    private string CreateTmpFolder()
    {
        var folderName = GenerateRandomName();
        var path = Path.Combine(_basePath, folderName);
        Directory.CreateDirectory(path);

        return path;
    }

    private static string GenerateRandomName()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-";
        Span<char> nameSpan = stackalloc char[16];
        Random.Shared.GetItems(chars, nameSpan);
        return nameSpan.ToString();
    }
}
