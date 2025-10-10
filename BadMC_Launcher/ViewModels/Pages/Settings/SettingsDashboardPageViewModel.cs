using BadMC_Launcher.Models.Data.ViewData;
using BadMC_Launcher.Controls;
using CommunityToolkit.Mvvm.Input;
using BadMC_Launcher.Extensions;
using CommunityToolkit.Mvvm.Messaging;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace BadMC_Launcher.ViewModels.Pages.Settings;

public partial class SettingsDashboardPageViewModel : ObservableObject {
    public SettingsDashboardPageViewModel() {
        SideBarItems = SettingsData.settingsSideBarItems;
        FootSideBarItems = SettingsData.settingsSideBarFooterItems;
    }
    [ObservableProperty]
    public partial DistinctiveItemBindingList<SettingsSideBarItem> SideBarItems { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<SettingsSideBarItem> FootSideBarItems { get; set; }

    [ObservableProperty]
    public partial SettingsSideBarItem? SideBarSelectedItem { get; set; }

    [RelayCommand]
    private void SetInitSelectedItem() {
        if (SideBarItems.Count > 0) {
            SideBarSelectedItem = SideBarItems[0];
        }
    }

    [RelayCommand]
    private void NavigateToPage(NavigationViewSelectionChangedEventArgs args) {
        if (args.SelectedItem is SettingsSideBarItem item) {
            WeakReferenceMessenger.Default.Send(new ValueChangedMessage<Type>(item.NavigatePage), MessengerTokenEnum.SettingsDashboardPage_PageNavigateToken.ToString());
        }
    }
}
