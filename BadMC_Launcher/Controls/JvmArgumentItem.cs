using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Controls;

public class JvmArgumentItem {

    public required string Argument { get; init; }

    public IconSource ViewIcon { get; init; } = new FontIconSource() { Glyph = "\uE943" };

    public string TipText { get; init; } = App.GetService<ResourceLoader>().GetString("DefaultJvmArgments_Unknown");

    public override string ToString() {
        return Argument;
    }

    public static bool operator ==(JvmArgumentItem? left, JvmArgumentItem? right) {
        if (left is JvmArgumentItem && right is JvmArgumentItem) {
            return left.Equals(right);
        }
        return ReferenceEquals(left, right);
    }

    public static bool operator !=(JvmArgumentItem? left, JvmArgumentItem? right) {
        if (left is JvmArgumentItem && right is JvmArgumentItem) {
            return !left.Equals(right);
        }
        return !ReferenceEquals(left, right);
    }

    public override bool Equals(object? obj) {
        if (obj is JvmArgumentItem item) {
            return Argument.Equals(item.Argument);
        }
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode() {
        return Argument.GetHashCode();
    }
}
