using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Helpers;
using Windows.Globalization;
using Windows.UI;

namespace BadMC_Launcher.ViewModels.Pages.Settings;
public partial class ThemeSettingsPageViewModel : ObservableObject {
    private readonly ThemeConfigsService themeService = App.GetService<ThemeConfigsService>();
    
    public ThemeSettingsPageViewModel() {
        ThemeType = themeService.ThemeType;
        AccentColorMode = themeService.AccentMode;

        ViewAccentColorHex = "Resource \"AccentFillColorDefaultBrush\" Error";

        // TODO: 改颜色
        ImageMonetColors = new ObservableCollection<SolidColorBrush>();

        #region MonetDEBUG
        //ImageMonetColors.Add(new SolidColorBrush("#0077FF".ToColor()));
        //ImageMonetColors.Add(new SolidColorBrush("#FF0000".ToColor()));
        //ImageMonetColors.Add(new SolidColorBrush("#CDFCDF".ToColor()));
        //ImageMonetColors.Add(new SolidColorBrush("#CDF555".ToColor()));
        //ImageMonetColors.Add(new SolidColorBrush("#ABCDF5".ToColor()));
        //ImageMonetColors.Add(new SolidColorBrush("#00AAFF".ToColor()));
        //SelectedImageMonetColor = new SolidColorBrush("#0077FF".ToColor());
        #endregion

        AccentColorHex = themeService.AccentColorHex;
        MonetColorHex = themeService.MonetAccentColorHex;
        BackgroundType = themeService.BackgroundType;
        BackgroundStretch = themeService.BackgroundStretch;
        SolidColorBackgroundHex = themeService.SolidColorBackgroundHex;
        WindowName = themeService.WindowName;
        Languages = AppParameters.Languages;
        Language = themeService.Language;
        GetAccentColorHex();

        themeService.PropertyChanged += OnThemeConfigChanged;
    }

    // Theme
    [ObservableProperty]
    public partial AppTheme ThemeType { get; set; }

    // Accent Color
    [ObservableProperty]
    public partial string ViewAccentColorHex { get; set; }

    [ObservableProperty]
    public partial AccentColorModeEnum AccentColorMode { get; set; }

    [ObservableProperty]
    public partial string AccentColorHex { get; set; }

    // Image Monet Colors
    // TODO: 类型可能搞错了(
    [ObservableProperty]
    public partial ObservableCollection<SolidColorBrush> ImageMonetColors { get; set; }

    // TODO: 类型可能又搞错了(
    [ObservableProperty]
    public partial SolidColorBrush? SelectedImageMonetColor { get; set; }

    // Color Monet Colors
    // TODO: 类型还是可能搞错了(
    [ObservableProperty]
    public partial string MonetColorHex { get; set; }

    // Background
    [ObservableProperty]
    public partial BackgroundTypeEnum BackgroundType { get; set; }

    [ObservableProperty]
    public partial Stretch BackgroundStretch { get; set; }

    [ObservableProperty]
    public partial string SolidColorBackgroundHex { get; set; }

    [ObservableProperty]
    public partial string WindowName { get; set; }

    [ObservableProperty]
    public partial HashSet<string> Languages { get; set; }

    [ObservableProperty]
    public partial string Language { get; set; }

    // Theme
    [RelayCommand]
    private void SetThemeType() {
        themeService.ThemeType = ThemeType;
    }

    // Accent Color
    [RelayCommand]
    private void SetAccentColorHex() {
        themeService.AccentColorHex = AccentColorHex;
    }

    // Monet colors
    [RelayCommand]
    private void SetMonetAndApply(Brush parameter) {
        // TODO: 万恶的Monet
    }

    [RelayCommand]
    private void SetMonetColorHex() {
        //TODO: 万恶的Monet
    }

    // Background
    [RelayCommand]
    private void SetSolidColorBackgroundHex() {
        themeService.SolidColorBackgroundHex = SolidColorBackgroundHex;
    }

    partial void OnAccentColorModeChanged(AccentColorModeEnum value) {
        themeService.AccentMode = value;
    }

    partial void OnBackgroundTypeChanged(BackgroundTypeEnum value) {
        themeService.BackgroundType = value;
    }

    private void GetAccentColorHex() {
        var resource = Application.Current.Resources["SystemAccentColor"];
        if (resource is Color color) {
            ViewAccentColorHex = color.ToNoAlphaHex();
        }
    }

    private void OnThemeConfigChanged(object? sender, PropertyChangedEventArgs args) {
        // Update Property
        switch (args.PropertyName) {
            // Background
            case nameof(ThemeConfigsService.BackgroundType):
                if (themeService.BackgroundType != BackgroundType) {
                    BackgroundType = themeService.BackgroundType;
                    // TODO: Update Monet Colors
                }
                break;
            case nameof(ThemeConfigsService.BackgroundStretch):
                if (themeService.BackgroundStretch != BackgroundStretch) {
                    BackgroundStretch = themeService.BackgroundStretch;
                }
                break;
            case nameof(ThemeConfigsService.SolidColorBackgroundHex):
                if (themeService.SolidColorBackgroundHex != SolidColorBackgroundHex) {
                    SolidColorBackgroundHex = themeService.SolidColorBackgroundHex;
                }
                break;

            // Theme
            case nameof(ThemeConfigsService.ThemeType):
                if (themeService.ThemeType != ThemeType) {
                    ThemeType = themeService.ThemeType;
                }
                break;

            // Accent Color
            case nameof(ThemeConfigsService.AccentMode):
                if (themeService.AccentMode != AccentColorMode) {
                    AccentColorMode = themeService.AccentMode;
                }
                break;
            case nameof(ThemeConfigsService.AccentColorHex):
                if (themeService.AccentColorHex != AccentColorHex) {
                    AccentColorHex = themeService.AccentColorHex;
                }
                break;
            case nameof(ThemeConfigsService.MonetAccentColorHex):
                if (themeService.MonetAccentColorHex != MonetColorHex) {
                    MonetColorHex = themeService.MonetAccentColorHex;
                }
                break;
        }
    }
}
