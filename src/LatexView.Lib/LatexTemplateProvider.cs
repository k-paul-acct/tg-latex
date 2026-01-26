using System.Text;

namespace LatexView.Lib;

public sealed class LatexTemplateProvider
{
    private readonly Dictionary<string, LatexTemplate> _templates;

    public LatexTemplateProvider()
    {
        _templates = [];
        var assembly = typeof(LatexTemplateProvider).Assembly;
        var resourceNames = assembly.GetManifestResourceNames();

        foreach (var name in resourceNames)
        {
            using var resource = assembly.GetManifestResourceStream(name);
            if (resource is null)
            {
                continue;
            }

            using var reader = new StreamReader(resource, Encoding.UTF8);
            var key = Path.GetFileNameWithoutExtension(name);
            var template = reader.ReadToEnd();
            _templates[key] = new LatexTemplate(template);
        }
    }

    public LatexTemplateProvider(IEnumerable<string> templatePaths)
    {
        _templates = [];

        foreach (var path in templatePaths)
        {
            var key = Path.GetFileNameWithoutExtension(path);
            var template = File.ReadAllText(path);
            _templates[key] = new LatexTemplate(template);
        }
    }

    public LatexTemplate GetTemplate(string name)
    {
        return _templates[name];
    }
}
