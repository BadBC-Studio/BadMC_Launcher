using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.ConfigsData;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media.Imaging;

namespace BadMC_Launcher.Controls.Converters;

class StringPathToImageConverter : IValueConverter {
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language) {
        if (value is string path && !string.IsNullOrEmpty(path)) {
            // Convert string to BitmapImage
            return new BitmapImage() {
                UriSource = new Uri(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers", path))
            };
        }
        // Default to white if the value is not a string
        return new BitmapImage();
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is BitmapImage image) {
            try {
                return Path.GetFileName(image.UriSource.LocalPath);
            }
            catch (ArgumentException) {
                return string.Empty;
            }
        }
        // Default to white if the value is error
        return string.Empty;
    }
}
