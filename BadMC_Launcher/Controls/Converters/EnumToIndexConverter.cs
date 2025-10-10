using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Enums;
using Microsoft.UI.Xaml.Data;

namespace BadMC_Launcher.Controls.Converters;

public class EnumToIndexConverter : IValueConverter {
    object IValueConverter.Convert(object value, Type targetType, object parameter, string language) {
        if (value is AccentColorModeEnum accentColorMode) {
            // Convert AccentColorModeEnum to an index
            return accentColorMode switch {
                AccentColorModeEnum.System => 0,
                AccentColorModeEnum.Custom => 1,
                AccentColorModeEnum.ImageMonet => 2,
                AccentColorModeEnum.ColorMonet => 3,
                _ => 0
            };
        }
        else if (value is AppTheme themeType) {
            // Convert AppTheme to an index

            return themeType switch {
                AppTheme.System => 0,
                AppTheme.Light => 1,
                AppTheme.Dark => 2,
                _ => 0
            };
        }
        else if (value is BackgroundTypeEnum backgroundType) {
            // Convert BackgroundTypeEnum to an index
            return backgroundType switch {
                BackgroundTypeEnum.StaticImage => 0,
                BackgroundTypeEnum.BingWallpaper => 1,
                BackgroundTypeEnum.Acrylic => 2,
                BackgroundTypeEnum.SolidColor => 3,
                _ => 0
            };
        }

        // If the value is not a ThemeTypeEnum, return 0 as a default index
        return 0;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language) {
        if (value is int accentIndex) {
            // Convert index back to AccentColorModeEnum
            return accentIndex switch {
                0 => AccentColorModeEnum.System,
                1 => AccentColorModeEnum.Custom,
                2 => AccentColorModeEnum.ImageMonet,
                3 => AccentColorModeEnum.ColorMonet,
                _ => 0
            };
        }
        else if (value is int themeIndex) {
            // Convert index back to AppTheme
            return themeIndex switch {
                0 => AppTheme.System,
                1 => AppTheme.Light,
                2 => AppTheme.Dark,
                _ => 0
            };
        }
        else if (value is int backgroundIndex) {
            // Convert index back to BackgroundTypeEnum
            return backgroundIndex switch {
                0 => BackgroundTypeEnum.StaticImage,
                1 => BackgroundTypeEnum.BingWallpaper,
                2 => BackgroundTypeEnum.Acrylic,
                3 => BackgroundTypeEnum.SolidColor,
                _ => 0
            };
        }

        // If the value is not an int, return 0.
        return 0;
    }
}
