using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Services.Settings;
using GEmojiSharp;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
using Uno.Extensions.Specialized;

namespace BadMC_Launcher.Controls.Minecraft;

public partial class MinecraftFolderViewItem : ObservableObject {

    public MinecraftFolderViewItem(string minecraftFolderPath, string? minecraftFolderId = null) {
        MinecraftFolderId = !string.IsNullOrWhiteSpace(minecraftFolderId) ? minecraftFolderId : "NewFolder" + Emoji.All.ElementAtOrDefault(App.GetService<Random>().Next(Emoji.All.Count()))?.Raw;
        MinecraftFolderPath = minecraftFolderPath;
    }

    public event PropertyChangedEventHandler? ActiveMinecraftEntryIdChanged;

    [ObservableProperty]
    public partial string MinecraftFolderId { get; set; }

    public string MinecraftFolderPath { get; init; }

    [IgnoreListChanged]
    [ObservableProperty]
    public partial string? ActiveMinecraftEntryId { get; set; }

    [ObservableProperty]
    public partial ObservableDataList<string> StarredMinecraftIds { get; set; } = new();

    public MinecraftParser GetMinecraftParser() => new MinecraftParser(MinecraftFolderPath);

    public IEnumerable<MinecraftEntry> GetMinecrafts() => GetMinecraftParser().GetMinecrafts();

    public static bool operator ==(MinecraftFolderViewItem? left, MinecraftFolderViewItem? right) {
        if (left is MinecraftFolderViewItem && right is MinecraftFolderViewItem) {
            return left.Equals(right);
        }
        return ReferenceEquals(left, right);
    }

    public static bool operator !=(MinecraftFolderViewItem? left, MinecraftFolderViewItem? right) {
        if (left is MinecraftFolderViewItem && right is MinecraftFolderViewItem) {
            return !left.Equals(right);
        }
        return !ReferenceEquals(left, right);
    }

    public override bool Equals(object? obj) {
        if (obj is MinecraftFolderViewItem folderEntry) {
            return this.MinecraftFolderPath == folderEntry.MinecraftFolderPath;
        }
        return ReferenceEquals(this, obj);
    }

    partial void OnActiveMinecraftEntryIdChanged(string? value) {
        ActiveMinecraftEntryIdChanged?.Invoke(this.ActiveMinecraftEntryId, new PropertyChangedEventArgs(nameof(ActiveMinecraftEntryId)));
        App.GetService<MinecraftConfigsService>().SyncSettingSet();
    }

    public override int GetHashCode() {
        // HashCode.Combine generates a hash code by combining the hash codes of the provided fields.
        // This ensures that the hash code is unique based on the values of these fields.
        return MinecraftFolderPath.GetHashCode();
    }
}
