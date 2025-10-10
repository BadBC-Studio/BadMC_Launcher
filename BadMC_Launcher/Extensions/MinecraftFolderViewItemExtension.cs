using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Services;
using BadMC_Launcher.Controls.Minecraft;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Components.Parser;
using BadMC_Launcher.Models.Enums;

namespace BadMC_Launcher.Extensions;
public static class MinecraftFolderViewItemExtension {
    public static MinecraftViewItem?  GetMinecraftItem(this MinecraftFolderViewItem minecraftPath, string minecraftEntryId) {
        try {
            return minecraftPath.GetMinecraftItem(minecraftPath.GetMinecraftParser().GetMinecraft(minecraftEntryId));
        }
        catch {

        }
        return null;
    }

    public static MinecraftViewItem? GetMinecraftItem(this MinecraftFolderViewItem minecraftFolderEntry, MinecraftEntry minecraftEntry) {
        try {
            if (minecraftEntry != null) {
                var isStarred = false;
                var minecraftEntryImageEnum = minecraftEntry.GetMinecraftImageEnum();
                BitmapImage? image = null;
                if (minecraftEntryImageEnum == MinecraftEntryImageEnum.Custom) {
                    var path = Path.Combine(minecraftEntry.MinecraftFolderPath, @"MConfigs\icon.png");
                    if (File.Exists(path)) {
                        image = new(new Uri(path));
                    }
                }
                else {
                    if (App.GetService<AppAssetsService>().MinecraftImageInstance[minecraftEntryImageEnum].Value.TryGetTarget(out BitmapImage? instance)) {
                        image = instance;
                    }
                }

                if (image == null) {
                    image = new(new Uri($@"ms-appx:///Assets/Icons/MinecraftIcons/{MinecraftEntryImageEnum.Unknown.ToString().ToLower()}.png"));
                }

                if (minecraftFolderEntry.StarredMinecraftIds != null) {
                    isStarred = minecraftFolderEntry.StarredMinecraftIds.IndexOf(minecraftEntry.Id) >= 0;
                }

                var entryItem = new MinecraftViewItem() {
                    MinecraftEntry = minecraftEntry,
                    MinecraftImage = image,
                    MinecraftTags = minecraftEntry.GetMinecraftEntryTags(),
                    IsStarred = isStarred
                };
                
                if (minecraftFolderEntry.StarredMinecraftIds != null) {
                    minecraftFolderEntry.StarredMinecraftIds.CollectionChanged += entryItem.SetIsStarredEvent;
                }
               
                return entryItem;
            }

        }
        catch {

        }
        return null;
    }

    public static IEnumerable<MinecraftViewItem> GetMinecraftItems(this MinecraftFolderViewItem minecraftPath) {
        var minecraftParser = minecraftPath.GetMinecraftParser();
        var items = new ObservableCollection<MinecraftViewItem>();
        foreach (var entry in minecraftParser.GetMinecrafts()) {
            var isStarred = false;
            string path;

            //Get Minecraft Icon
            var minecraftEntryImageEnum = entry.GetMinecraftImageEnum();
            if (minecraftEntryImageEnum == MinecraftEntryImageEnum.Custom) {
                path = Path.Combine(entry.MinecraftFolderPath, @"BadBCConfigs\icon.png");
                if (!File.Exists(path)) {
                    throw new FileNotFoundException($"\"{path}\" is not found.");
                }
            }
            else {
                path = @$"ms-appx:///Assets/Icons/MinecraftIcons/{minecraftEntryImageEnum.ToString().ToLower()}.png";
            }

            //IsStarred
            if (minecraftPath.StarredMinecraftIds != null) {
                isStarred = minecraftPath.StarredMinecraftIds.IndexOf(entry.Id) >= 0;
            }
            items.Add(new MinecraftViewItem() {
                MinecraftEntry = entry,
                MinecraftImage = new BitmapImage() { UriSource = new Uri(path) },
                MinecraftTags = entry.GetMinecraftEntryTags(),
                IsStarred = isStarred,
            });
        }
        return items;
    }

   
}
