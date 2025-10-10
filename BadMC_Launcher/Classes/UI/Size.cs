using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BadMC_Launcher.Classes.UI;

public struct Size(uint width, uint height) {
    public uint Width { get; set; } = width;

    public uint Height { get; set; } = height;

    public static bool operator ==(Size? left, Size? right) {
        if (left is Size && right is Size) {
            return left.Equals(right);
        }
        return object.Equals(left, right);
    }

    public static bool operator !=(Size? left, Size? right) {
        if (left is Size && right is Size) {
            return !left.Equals(right);
        }
        return !object.Equals(left, right);
    }

    public override bool Equals(object? obj) {
        if (obj is Size size) {
            return Width == size.Width && Height == size.Height;
        }
        return object.Equals(this, obj);
    }

    public override int GetHashCode() {
        return HashCode.Combine(Width.GetHashCode(), Height.GetHashCode());
    }
}
