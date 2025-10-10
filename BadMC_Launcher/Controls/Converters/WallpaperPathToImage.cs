using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.ConfigsData;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BadMC_Launcher.Controls.Converters;

public class WallpaperPathToImage : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, string language) {
        if (value is string path && !string.IsNullOrEmpty(path)) {
            // Convert the string path to an image
            return new BitmapImage() {
                UriSource = new Uri(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers", path))
            };
        }

        return new BitmapImage();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is BitmapImage image && !string.IsNullOrEmpty(image.UriSource.ToString())) {
            // Convert the string path to an image
            return Path.GetFileName(image.UriSource.LocalPath);
        }

        return new BitmapImage();
    }
}
