using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Classes.DataClasses;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.ConfigsData;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.UI;

namespace BadMC_Launcher.Services.Configs;
public class ThemeConfigsService : ConfigClass {
    private readonly PathService pathService;
    private readonly ResourceLoader resourceLoader;
    private readonly NotificationService notificationService;
    private IThemeService? AppThemeService => null;

    internal bool isSyncEnabled = false;

    public ThemeConfigsService(PathService _pathService, 
        ResourceLoader _resourceLoader, 
        NotificationService _notificationService) {
        pathService = _pathService;
        resourceLoader = _resourceLoader;
        notificationService = _notificationService;
    }

    public ThemeConfigsService() {
        pathService = App.GetService<PathService>();
        resourceLoader = App.GetService<ResourceLoader>();
        notificationService = App.GetService<NotificationService>();
    }

    // Background
    public BackgroundTypeEnum BackgroundType {
        get => ThemeConfigs.backgroundType;
        set {
            if (ThemeConfigs.backgroundType != value) {
                ThemeConfigs.backgroundType = value;

                // Trigger Event
                OnPropertyChanged(nameof(BackgroundType));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public string ImageBackgroundName {
        get => ThemeConfigs.imageBackgroundName;
        set {
            if (ThemeConfigs.imageBackgroundName != value) {
                ThemeConfigs.imageBackgroundName = value;

                // Trigger Event
                OnPropertyChanged(nameof(ImageBackgroundName));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public Stretch BackgroundStretch {
        get => ThemeConfigs.backgroundStretch;
        set {
            if (ThemeConfigs.backgroundStretch != value) {
                ThemeConfigs.backgroundStretch = value;

                // Trigger Event
                OnPropertyChanged(nameof(BackgroundStretch));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public string SolidColorBackgroundHex {
        get => ThemeConfigs.solidColorBackgroundHex;
        set {
            if (ThemeConfigs.solidColorBackgroundHex != value) {
                ThemeConfigs.solidColorBackgroundHex = value;

                // Trigger Event
                OnPropertyChanged(nameof(SolidColorBackgroundHex));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    // Theme
    public AppTheme ThemeType { 
        get => ThemeConfigs.themeType;
        set {
            if (ThemeConfigs.themeType != value) {
                ThemeConfigs.themeType = value;

                SetAppTheme(ThemeType);

                // Trigger Event
                OnPropertyChanged(nameof(ThemeType));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    // Accent Color
    public AccentColorModeEnum AccentMode {
        get => ThemeConfigs.accentMode;
        set {
            if (ThemeConfigs.accentMode != value) {
                ThemeConfigs.accentMode = value;

                // Trigger Event
                OnPropertyChanged(nameof(AccentMode));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public string AccentColorHex {
        get => ThemeConfigs.accentColorHex;
        set {
            if (ThemeConfigs.accentColorHex != value) {
                ThemeConfigs.accentColorHex = value;

                // Trigger Event
                OnPropertyChanged(nameof(AccentColorHex));

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    public string MonetAccentColorHex {
        get => ThemeConfigs.monetAccentColorHex;
        set {
            if (ThemeConfigs.monetAccentColorHex != value) {
                ThemeConfigs.monetAccentColorHex = value;

                // Trigger Event
                OnPropertyChanged(nameof(MonetAccentColorHex));          

                // Sync Setting
                SyncSettingSet();
            }
        }
    }

    // Window Name
    public string WindowName {
        get => ThemeConfigs.windowName;
        set {
            if (ThemeConfigs.windowName != value) {
                ThemeConfigs.windowName = value;

                // Trigger Event
                OnPropertyChanged(nameof(WindowName));
            }
        }
    }
    // Language
    public string Language {
        get => ThemeConfigs.language;
        set {
            if (ThemeConfigs.language != value) {
                ThemeConfigs.language = value;

                // Trigger Event
                OnPropertyChanged(nameof(Language));
            }
        }
    }

    public async void SetAppTheme(AppTheme theme) {
        if (AppThemeService != null && AppThemeService.Theme != ThemeType) {
            await AppThemeService.SetThemeAsync(ThemeType);
        }
    }

    private void SetAccentColor() {
        if (ThemeConfigs.accentMode == AccentColorModeEnum.Custom) {
            if (!string.IsNullOrWhiteSpace(ThemeConfigs.accentColorHex)) {
                var lightColorHsl = ThemeConfigs.accentColorHex.ToColor().ToHsl();
                var colors = new AccentColors(ThemeConfigs.accentColorHex.ToColor());

                App.Current.Resources["SystemAccentColor"] = colors.MainColor;
                App.Current.Resources["SystemAccentColorLight1"] = colors.LightColorPrimary;
                App.Current.Resources["SystemAccentColorLight2"] = colors.LightColorSecondary;
                App.Current.Resources["SystemAccentColorLight3"] = colors.LightColorTertiary;
                App.Current.Resources["SystemAccentColorDark1"] = colors.DarkColorPrimary;
                App.Current.Resources["SystemAccentColorDark2"] = colors.DarkColorSecondary;
                App.Current.Resources["SystemAccentColorDark3"] = colors.DarkColorTertiary;
            }
        }
    }

    public void GetMonetColorsFromBackground() {
        // TODO
    }

    public void UseMonet() {
        // TODO
    }

    public void SetTextColor(bool isDark) {
        
    }

    public async Task<Brush> SetBackground(BackgroundTypeEnum backgroundType) {
        if (App.GetService<PathService>().CheckPath(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers"))) {
            if (!Directory.Exists(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers"))) {
                Directory.CreateDirectory(Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers"));
            }
            switch (backgroundType) {
                case BackgroundTypeEnum.StaticImage:
                    var path = Path.Combine(AppDataPath.pathsList["AssetsPath"], "Wallpapers", ThemeConfigs.imageBackgroundName);
                    if (!pathService.CheckPath(path)) {
                        notificationService.ShowNotification(new TipMessageNotificationItem(MessageSeverityEnum.Error,
                            resourceLoader.GetString("ToastNotification_BackgroundErrorTitle"),
                            $"{resourceLoader.GetString("ToastNotification_BackgroundErrorMessage")} {path}"));
                        return SetBrush(Color.FromArgb(0, 119, 255, 1)); ;
                    }
                    return SetBrush(new BitmapImage(new Uri(path)));
                case BackgroundTypeEnum.BingWallpaper:
                    var bingWallpaperPath = await GetBingWallpaperUrl();
                    if (string.IsNullOrEmpty(bingWallpaperPath)) {
                        return SetBrush(Color.FromArgb(0, 119, 255, 1));
                    }
                    return SetBrush(new BitmapImage(new Uri(bingWallpaperPath)));
                case BackgroundTypeEnum.Acrylic:

                    //TODO: Only MacOS
                    return SetBrush(Color.FromArgb(0, 119, 255, 1));
                default:
                    var color = ThemeConfigs.solidColorBackgroundHex.ToColor();
                    return SetBrush(Color.FromArgb(color.A, color.R, color.G, color.B));
            }
        }
        return SetBrush(Color.FromArgb(0, 119, 255, 1));
    }

    public override bool SyncSettingGet() {
        if (pathService.TryReadConfig(Path.Combine(AppDataPath.pathsList["ConfigsPath"], @"ThemeConfigs.json"), ThemeConfigsServiceContext.Default.ThemeConfigsService, out var jsonClass) && jsonClass != null) {
            ThemeConfigs.backgroundType = jsonClass.BackgroundType;
            ThemeConfigs.themeType = jsonClass.ThemeType;
            ThemeConfigs.imageBackgroundName = jsonClass.ImageBackgroundName;
            ThemeConfigs.backgroundStretch = jsonClass.BackgroundStretch;
            ThemeConfigs.windowName = jsonClass.WindowName;

            if (AccentMode == AccentColorModeEnum.Custom) {
                SetAccentColor();
            }

            return true;
        }
        return false;
    }

    public override bool SyncSettingSet() {
        if (pathService.WriteConfig(Path.Combine(AppDataPath.pathsList["ConfigsPath"], @"ThemeConfigs.json"), ThemeConfigsServiceContext.Default.ThemeConfigsService, this)) {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<ThemeConfigsService>(this), "MinecraftConfigChanged");
            return true;
        }
        return false;
    }
    public Brush SetBrush<T>(T color) {
        if (typeof(T) == typeof(Color) && color != null) {
            return new SolidColorBrush((Color)(object)color);
        }
        if (typeof(T) == typeof(BitmapImage) && color != null) {
            return new ImageBrush() {
                ImageSource = (BitmapImage)(object)color,
                Stretch = ThemeConfigs.backgroundStretch
            };
        }
        throw new InvalidOperationException("Unsupported type for SetBrushAsync");
    }

    public async Task<string> GetBingWallpaperUrl() {
        try {
            var jsonText = await App.GetService<HttpClient>().GetStringAsync("https://cn.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=zh-CN");
            using JsonDocument doc = JsonDocument.Parse(jsonText);
            var status = doc.RootElement.TryGetProperty("images", out var imagesJsonElement);
            status = imagesJsonElement[0].TryGetProperty("url", out var urlJsonElement);
            if (status == true) {
                var url = urlJsonElement.GetString();
                if (url != null) {
                    return "https://cn.bing.com" + url;
                }
            }
            notificationService.ShowNotification(new TipMessageNotificationItem(
                MessageSeverityEnum.Error,
                resourceLoader.GetString("ToastNotification_BingWallpaperErrorTitle"),
                resourceLoader.GetString("ToastNotification_BingWallpaperJsonErrorMessage")));
        }
        catch (Exception ex) {
            switch (ex) {
                case HttpRequestException:
                    notificationService.ShowNotification(new TipMessageNotificationItem(
                        MessageSeverityEnum.Error,
                        resourceLoader.GetString("ToastNotification_BingWallpaperErrorTitle"),
                        resourceLoader.GetString("ToastNotification_BingWallpaperGetErrorMessage")));
                    break;
                case JsonException:
                    notificationService.ShowNotification(new TipMessageNotificationItem(
                        MessageSeverityEnum.Error,
                        resourceLoader.GetString("ToastNotification_BingWallpaperErrorTitle"),
                        resourceLoader.GetString("ToastNotification_BingWallpaperJsonErrorMessage")
                    ));
                    break;
                default:
                    throw;
            }
        }
        return string.Empty;
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(ThemeConfigsService))]
internal partial class ThemeConfigsServiceContext : JsonSerializerContext;
