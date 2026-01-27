using System.Collections.ObjectModel;

namespace LatexView.Lib;

internal static class ObjectModelCollectionExtensions
{
    public static void AddRange<T>(this Collection<T> self, params IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            self.Add(value);
        }
    }

    public static void AddRange<T>(this Collection<T> self, params ReadOnlySpan<T> values)
    {
        foreach (var value in values)
        {
            self.Add(value);
        }
    }
}
