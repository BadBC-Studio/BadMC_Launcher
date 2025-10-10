using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Extensions;
public static class IEnumerableExtension {
    public static int GetIndex<T>(this IEnumerable<T> source, Func<T, bool> func) {
        var item = source.FirstOrDefault(func);
        return source.IndexOf(item);
    }

    public static bool TryElementAt<T>(this IEnumerable<T> source, int index, out T? returnItem) {
        if (index >= 0 && index < source.Count()) {
            returnItem = source.ElementAtOrDefault(index);
            return true;
        }
        returnItem = default;
        return false;
    }

    public static bool HasIndex<T>(this IEnumerable<T> list, int index) {
        return index >= 0 && index < list.Count();
    }
}
