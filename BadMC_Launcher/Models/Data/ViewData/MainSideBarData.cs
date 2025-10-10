using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Controls.MainSearch;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Views.Pages.Settings;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;

namespace BadMC_Launcher.Models.Data.ViewData;
internal static class MainSideBarData {
    internal static DistinctiveItemBindingList<MainSideBarItem> mainSideBarItems = new();

    internal static DistinctiveItemBindingList<MainSideBarItem> mainSideBarFooterItems = new() {
        // Initialize the footer items
        new MainSideBarItem() {
            ItemName = App.GetService<ResourceLoader>().GetString("SideBar_SettingsNameResource"),
            ItemIcon = new AnimatedIcon() {
                Source = new AnimatedSettingsVisualSource(),
                FallbackIconSource = new FontIconSource() { Glyph = "\uE713" },
            },
            NavigatePage = typeof(SettingsDashboardPage)
        }
    };

    internal static DistinctiveItemBindingList<IMainMenuSharchFilterItem> mainMenuSharchFilterItems = new() {
        // Initialize the search filter items
        new MainMenuSearchMinecraftEntryFilter() {
            ItemName = App.GetService<ResourceLoader>().GetString("SideBarSearchFilter_MinecraftEntryNameResource"),
            IconGlyph = "\uE7FC"
        }
    };

    internal static DistinctiveItemBindingList<MainMenuItem> mainMenuItems = new();
}
