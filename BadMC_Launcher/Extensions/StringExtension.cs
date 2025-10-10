using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.ConfigsData;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BadMC_Launcher.Extensions;

public static class StringExtension {
    public static BitmapImage FindImageFromBackground(this string path) => new BitmapImage() {
        UriSource = new Uri(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers", ThemeConfigs.imageBackgroundName))
    };
}
