using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Classes.Comparers;
public class JavaPathListComparer : IEqualityComparer<string> {
    public bool Equals(string? left, string? right) {
        var leftDirectoryPath = Path.GetDirectoryName(left);
        var rightDirectoryPath = Path.GetDirectoryName(right);

        return left is not null && right is not null
            ? leftDirectoryPath == rightDirectoryPath
            : ReferenceEquals(left, right);
    }
    
    public int GetHashCode(string obj) {
        return obj.ToLower().GetHashCode();
    }
}
