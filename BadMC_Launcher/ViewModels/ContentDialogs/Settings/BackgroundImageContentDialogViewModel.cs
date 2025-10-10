using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Services.Configs;
using CommunityToolkit.Mvvm.Input;
using Uno.Extensions.Toolkit;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;

public partial class BackgroundImageContentDialogViewModel : ObservableObject {
    private readonly ThemeConfigsService themeService;

    public BackgroundImageContentDialogViewModel() {
        themeService = App.GetService<ThemeConfigsService>();

        BackgroundNames = new ObservableCollection<string>();
        BackgroundName = themeService.ImageBackgroundName;
    }

    [ObservableProperty]
    public partial ObservableCollection<string> BackgroundNames { get; set; }

    [ObservableProperty]
    public partial string BackgroundName { get; set; }

    [RelayCommand]
    private void SetBackgroundImage(string Parameter) {

    }

    private void OnThemeConfigChanged(object? sender, PropertyChangedEventArgs args) {
        // Update Property
        switch (args.PropertyName) {
            // Background
            case nameof(ThemeConfigsService.ImageBackgroundName):
                if (themeService.ImageBackgroundName != BackgroundName && !string.IsNullOrWhiteSpace(themeService.ImageBackgroundName)) {
                    BackgroundName = themeService.ImageBackgroundName;
                }
                break;
        }
    }
}
