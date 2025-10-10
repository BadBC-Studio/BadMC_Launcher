using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml.Data;
using Windows.UI;

namespace BadMC_Launcher.Controls.Converters;
public class HexToColorConverter : IValueConverter {
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language) {
        if (value is string hex && !string.IsNullOrEmpty(hex)) {
            // Convert hex string to color
            return hex.ToColor();
        }
        // Default to white if the value is not a valid hex string
        return "#FFFFFFFF".ToColor(); 
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is Color color) {
            // Convert color to hex string
            return color.ToHex();
        }
        // Default to white if the value is not a Color
        return "#FFFFFFFF";
    }
}
