using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.WinUI.Helpers;

namespace BadMC_Launcher.Models.Data.ConfigsData;
internal static class ThemeConfigs {
    internal static BackgroundTypeEnum backgroundType = BackgroundTypeEnum.StaticImage;

    internal static AppTheme themeType = AppTheme.System;

    internal static string imageBackgroundName = "WALLPAPER.PNG";

    internal static Stretch backgroundStretch = Stretch.UniformToFill;

    internal static string solidColorBackgroundHex = "#FFFFFFFF";

    internal static AccentColorModeEnum accentMode = AccentColorModeEnum.System;
    
    internal static string accentColorHex = App.Current.Resources["SystemAccentColor"] is SolidColorBrush solidColorBrush ? solidColorBrush.Color.ToHex() : "#0077FF";

    internal static string monetAccentColorHex = App.Current.Resources["SystemAccentColor"] is SolidColorBrush solidColorBrush ? solidColorBrush.Color.ToHex() : "#0077FF";

    internal static string windowName = "BadMC Launcher";

    internal static string language = Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride ?? "en-US";
}
