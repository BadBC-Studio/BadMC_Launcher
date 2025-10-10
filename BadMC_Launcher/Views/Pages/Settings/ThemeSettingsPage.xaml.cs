using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.ViewModels.Pages.Settings;
using Flurl.Util;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BadMC_Launcher.Views.Pages.Settings;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ThemeSettingsPage : Page {
    private readonly ThemeSettingsPageViewModel viewModel = new ThemeSettingsPageViewModel();

    public ThemeSettingsPage() {
        this.InitializeComponent();
        DataContext = viewModel;
    }

    private Visibility AccentColorModeToVisibility(AccentColorModeEnum colorMode, int controlMode) {
        switch (colorMode) {
            case AccentColorModeEnum.System:
                switch (controlMode) {
                    case 0:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            case AccentColorModeEnum.Custom:
                switch (controlMode) {
                    case 1:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            case AccentColorModeEnum.ImageMonet:
                switch (controlMode) {
                    case 2:
                    case 4:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            case AccentColorModeEnum.ColorMonet:
                switch (controlMode) {
                    case 3:
                    case 4:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            default:
                return Visibility.Collapsed;
        }
    }

    private Visibility BackgroundTypeToVisibility(BackgroundTypeEnum backgroundType, int controlMode) {
        switch (backgroundType) {
            // Static image
            case BackgroundTypeEnum.StaticImage:
                switch (controlMode) {
                    case 0:
                    case 2:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }

            // Bing wallpaper
            case BackgroundTypeEnum.BingWallpaper:
                switch (controlMode) {
                    case 2:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }

            // Solid color background
            case BackgroundTypeEnum.SolidColor:
                switch (controlMode) {
                    case 1:
                        return Visibility.Visible;
                    default:
                        return Visibility.Collapsed;
                }
            default:
                return Visibility.Collapsed;
        }
    }

    private string ToNoAlphaHex(Color color) => color.ToNoAlphaHex();
}
