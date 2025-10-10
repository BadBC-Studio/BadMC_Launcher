using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Controls.Minecraft;
public partial class JavaViewItem : ObservableObject {
    public JavaViewItem(JavaEntry javaEntry) {
        Entry = javaEntry;

        JavaId = $"{Entry.JavaType} {Entry.MajorVersion}";

        var is64Bit = Entry.Is64bit ? "64Bit" : "32Bit";
        JavaType = $"{is64Bit} / {Entry.JavaVersion}";

        JavaPath = Entry.JavaPath;

        JavaIconPath = $"ms-appx:///Assets/Icons/JavaIcons/{Entry.JavaType.ToLower()}.png";
    }

    public JavaEntry Entry { get; init; }

    public string JavaId { get; init; }

    public string JavaType { get; init; }

    public string JavaPath { get; init; }

    [ObservableProperty]
    public partial string? JavaIconPath { get; private set; }

    public static bool operator ==(JavaViewItem? left, JavaViewItem? right) {
        if (left is JavaViewItem && right is JavaViewItem) {
            return left.Equals(right);
        }
        return ReferenceEquals(left, right);
    }

    public static bool operator !=(JavaViewItem? left, JavaViewItem? right) {
        if (left is JavaViewItem && right is JavaViewItem) {
            return !left.Equals(right);
        }
        return !ReferenceEquals(left, right);
    }

    public override bool Equals(object? obj) {
        if (obj is JavaViewItem viewItem) {
            var leftDirectoryPath = Path.GetDirectoryName(this.JavaPath);
            var rightDirectoryPath = Path.GetDirectoryName(viewItem.JavaPath);

            return this.JavaPath.Equals(viewItem.JavaPath);
        }
        return ReferenceEquals(this, obj);
    }

    public override int GetHashCode() {
        // HashCode.Combine generates a hash code by combining the hash codes of the provided fields.
        // This ensures that the hash code is unique based on the values of these fields.
        return JavaPath.GetHashCode();
    }
}
