using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls.Minecraft;
using Microsoft.UI.Xaml.Media.Imaging;
using MinecraftLaunch.Base.Models.Game;
using Uno.Disposables;

namespace BadMC_Launcher.Extensions;
public static class JavaEntryExtension {

    public async static Task<string> GetJavaIconPathAsync(this JavaEntry javaEntry) {
        var imageFolder = await Package.Current.InstalledLocation.GetFolderAsync("Assets/Icons/JavaIcons");
        var image = await imageFolder.TryGetItemAsync($"{javaEntry.JavaType.ToLower()}.png");
        if (image != null) {
            return $"ms-appx:///Assets/Icons/JavaIcons/{image.Name}";
        }
        return "ms-appx:///Assets/Icons/JavaIcons/openjdk.png";
    }
}
