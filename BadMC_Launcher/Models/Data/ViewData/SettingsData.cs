using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Views.Pages.Settings;


namespace BadMC_Launcher.Models.Data.ViewData;

internal static class SettingsData {
    internal static DistinctiveItemBindingList<SettingsSideBarItem> settingsSideBarItems = new() {
        // Initialize the sidebar items
        new SettingsSideBarItem() {
            ItemName = App.GetService<ResourceLoader>().GetString("SettingsSideBarItems_LaunchSettingsPageName"),
            ItemIcon = new FontIcon() { Glyph = "\uE7FC" },
            NavigatePage = typeof(LaunchSettingsPage),
            PageHead = App.GetService<ResourceLoader>().GetString("SettingsSideBarItems_LaunchSettingsPageName"),
        },
        new SettingsSideBarItem() {
            ItemName = App.GetService<ResourceLoader>().GetString("SettingsSideBarItems_ThemeSettingsPageName"),
            ItemIcon = new FontIcon() { Glyph = "\uE790" },
            NavigatePage = typeof(ThemeSettingsPage),
            PageHead = App.GetService<ResourceLoader>().GetString("SettingsSideBarItems_ThemeSettingsPageName"),
        }
    };

    internal static DistinctiveItemBindingList<SettingsSideBarItem> settingsSideBarFooterItems = new();
}
