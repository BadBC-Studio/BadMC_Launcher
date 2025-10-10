using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.WindowsRuntime;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.ViewModels.Pages.Settings;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization.NumberFormatting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BadMC_Launcher.Views.Pages.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class LaunchSettingsPage : Page {
    private LaunchSettingsPageViewModel viewModel => (LaunchSettingsPageViewModel)DataContext;
    public LaunchSettingsPage() {
		InitializeComponent();
        DataContext = new LaunchSettingsPageViewModel();

        DecimalFormatter formatter = new DecimalFormatter();
        formatter.FractionDigits = 0;

        MinGameMemoryNumberBox.NumberFormatter = formatter;
        MaxGameMemoryNumberBox.NumberFormatter = formatter;
    }

    private string MbToGbString(object Memory) {
        if (Memory is uint number) {
            return Math.Round(number / 1024.0, 2).ToString();
        }
        return string.Empty;
    }

    private string GetAvailableMemoryView(object usedMemory, object maxMemory) {
        if (maxMemory is uint maxNumber && usedMemory is uint usedNumber) {
            return Math.Round((maxNumber - usedNumber) / 1024.0, 2).ToString();
        }
        return string.Empty;
    }

    private double GetProgressRingMemoryView(object memory, object maxMemory) {
        if (memory is uint number && maxMemory is uint maxNumber) {
            return Math.Round(( (double)number / maxNumber ) * 100.0, 2);
        }
        return 0.0;
    }

    private double GetProgressRingMemoryView(object memory, object maxMemory, object rootMemory) {
        if (memory is uint number && maxMemory is uint maxNumber && rootMemory is uint rootNumber) {
            return Math.Round(( (double)(number + rootNumber) / maxNumber ) * 100.0, 2);
        }
        return 0.0;
    }

    private string GetGameMemoryScore(object gameMemory, object usedMemory, object maxMemory) {
        if (gameMemory is uint numberGameMemory && usedMemory is uint numberUsedMemory && maxMemory is uint numberMaxMemory) {
            var gameMemoryGb = Math.Round(numberGameMemory / 1024.0, 2);
            var usedMemoryGb = Math.Round(numberUsedMemory / 1024.0, 2);
            var maxMemoryGb = Math.Round(numberMaxMemory / 1024.0, 2);

            var loader = App.GetService<ResourceLoader>();

            if (gameMemoryGb <= 0.0) {
                return loader.GetString("LaunchSettingsPage_GameMemoryScore0");
            }
            else if ((usedMemoryGb + gameMemoryGb) > maxMemoryGb) {
                return loader.GetString("LaunchSettingsPage_GameMemoryScore1");
            }
            else if (gameMemoryGb <= 0.5) {
                return loader.GetString("LaunchSettingsPage_GameMemoryScore2");
            }
            else if (gameMemoryGb < 2) {
                return loader.GetString("LaunchSettingsPage_GameMemoryScore3");

            }
            else if (gameMemoryGb < 4) {
                return loader.GetString("LaunchSettingsPage_GameMemoryScore4");

            }
            else if (gameMemoryGb < 16) {
                return loader.GetString("LaunchSettingsPage_GameMemoryScore5");

            }
            else if (gameMemoryGb < 32) {
                return loader.GetString("LaunchSettingsPage_GameMemoryScore6");

            }
            return loader.GetString("LaunchSettingsPage_GameMemoryScore7");
        }
        return string.Empty;
    }

    private void OnMemoryFlyoutOpened(object sender, object args) {
        GameMemoryRangeSelector.Focus(FocusState.Programmatic);
    }
}

