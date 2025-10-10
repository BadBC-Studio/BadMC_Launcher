using System.ComponentModel;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Controls.MainSearch;
using BadMC_Launcher.Interfaces;
using BadMC_Launcher.Models.Data.ViewData;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Views.UserControls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace BadMC_Launcher.ViewModels.Pages;

public partial class MainPageViewModel : ObservableObject {
    private bool isLaunchPadLightEliminationEnabled;
    ThemeConfigsService themeService = App.GetService<ThemeConfigsService>();

    public MainPageViewModel() {
        //Init Property
        WindowName = themeService.WindowName;
        IsLaunchPadOpen = true;

        IsMainSideBarToolShow = false;
        MainSideBarItems = MainSideBarData.mainSideBarItems;
        MainSideBarFooterItems = MainSideBarData.mainSideBarFooterItems;

        SearchFilterItems = MainSideBarData.mainMenuSharchFilterItems;
        SearchItems = new();
        SearchFilterSelectedItems = new();
        SearchText = string.Empty;

        SetBackground();
        themeService.PropertyChanged += ThemeService_PropertyChanged;
    }

    [ObservableProperty]
    public partial string WindowName { get; set; }
    
    [ObservableProperty]
    public partial Brush? AppBackground { get; set; }

    [ObservableProperty]
    public partial bool IsLaunchPadOpen { get; set; }

    //MainSideBar Items
    [ObservableProperty]
    public partial DistinctiveItemBindingList<MainSideBarItem> MainSideBarItems { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<MainSideBarItem> MainSideBarFooterItems { get; set; }

    [ObservableProperty]
    public partial MainSideBarItem? MainSideBarSelectedItem { get; set; }

    [ObservableProperty]
    public partial bool IsMainSideBarToolShow { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<IMainMenuSharchFilterItem> SearchFilterItems { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<IMainMenuSharchFilterItem> SearchFilterSelectedItems { get; set; }

    [ObservableProperty]
    public partial DistinctiveItemBindingList<MainMenuSearchResultItem> SearchItems { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; }

    [RelayCommand]
    private void ChangeToolVisibilityAndSelectedItem(object? parameter) {
        if (parameter == null) {
            IsMainSideBarToolShow = false;
            isLaunchPadLightEliminationEnabled = false;
            IsLaunchPadOpen = true;
        }
        else {
            IsMainSideBarToolShow = true;
            isLaunchPadLightEliminationEnabled = true;
            IsLaunchPadOpen = false;
        }

        var list = new List<MainSideBarItem>();
        list.AddRange(MainSideBarItems);
        list.AddRange(MainSideBarFooterItems);

        // Set SelectedItem
        MainSideBarSelectedItem = list.FirstOrDefault(item => item.NavigatePage == parameter?.GetType());
    }   

    [RelayCommand]
    private void NavigateToPage(NavigationViewItemInvokedEventArgs args) {
        if (args.InvokedItem != null) {
            var list = new List<MainSideBarItem>();
            list.AddRange(MainSideBarItems);
            list.AddRange(MainSideBarFooterItems);

            // Set SelectedItem
            var navigateItem = list.FirstOrDefault(item => item.ItemName == args.InvokedItem.ToString());

            if (navigateItem != null && navigateItem != MainSideBarSelectedItem) {
                SendInvokeFuncMessage(navigateItem.NavigatePage, MessengerTokenEnum.MainPage_PageNavigateToken);
                return;
            }
        }
        SendInvokeFuncMessage<bool>(MessengerTokenEnum.MainPage_PageCloseToken);
    }

    [RelayCommand]
    private void CloseLaunchPad(PointerRoutedEventArgs e) {
        if (isLaunchPadLightEliminationEnabled && !IsVisualTreeRootLaunchPad((DependencyObject)e.OriginalSource)) {
            IsLaunchPadOpen = false;
        }
    }

    [RelayCommand]
    private void PageGoBack() {
        SendInvokeFuncMessage<bool>(MessengerTokenEnum.MainPage_PageGoBackToken);
    }

    [RelayCommand]
    private void ClosePage() {
        SendInvokeFuncMessage<bool>(MessengerTokenEnum.MainPage_PageCloseToken);
    }

    [RelayCommand]
    private void DeselectItem() {
        MainSideBarSelectedItem = null;
    }

    [RelayCommand]
    private void SearchItem(AutoSuggestBoxTextChangedEventArgs e) {
        if (e.Reason == AutoSuggestionBoxTextChangeReason.UserInput) {
            var searchItems = new DistinctiveItemBindingList<MainMenuSearchResultItem>();
            foreach (var item in SearchFilterSelectedItems) {
                foreach (var searchMainMenuSearchResultItem in item.Search(SearchText)) {
                    searchItems.Add(searchMainMenuSearchResultItem);
                }
            }
            SearchItems = searchItems;
        }
    }

    [RelayCommand]
    private void SearchItemNavigateToPage(AutoSuggestBoxSuggestionChosenEventArgs e) {
        if (e.SelectedItem is MainMenuSearchResultItem selectedItem) {
            selectedItem.Navigate.Invoke();
        }
    }

    [RelayCommand]
    private void OpenSearchFilter(AutoSuggestBox parameter) {
        FlyoutBase.ShowAttachedFlyout(parameter);
    }

    [RelayCommand]
    private void ChangeSearchFilter(Segmented parameter) {
        var items = new List<IMainMenuSharchFilterItem>();
        foreach (var selectedItem in parameter.SelectedItems) {
            if (selectedItem is IMainMenuSharchFilterItem item) {
                items.Add(item);
            }
        }
        SearchFilterSelectedItems.Clear();
        SearchFilterSelectedItems.AddRange(items.ToObservableCollection());
    }

    private void SendInvokeFuncMessage<T>(T value, Enum tokenEnum) {
        WeakReferenceMessenger.Default.Send(new ValueChangedMessage<T>(value), tokenEnum.ToString());
    }

    private async void SetBackground() {
        AppBackground = await themeService.SetBackground(themeService.BackgroundType);
    }

    private RequestMessage<T> SendInvokeFuncMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }

    private bool IsVisualTreeRootLaunchPad(DependencyObject sourceElement) {
        while (sourceElement != null) {
            if (sourceElement is LaunchPad)
            {
                return true;
            }
            sourceElement = VisualTreeHelper.GetParent(sourceElement);
        }
        return false;
    }

    private async void ThemeService_PropertyChanged(object? sender, PropertyChangedEventArgs args) {
        if (args.PropertyName != nameof(ThemeConfigsService.BackgroundType)) {
            // Check BackgroundType
            switch (themeService.BackgroundType) {
                case BackgroundTypeEnum.StaticImage:
                    if (args.PropertyName != nameof(ThemeConfigsService.ImageBackgroundName) && args.PropertyName != nameof(ThemeConfigsService.BackgroundStretch)) {
                        return;
                    }
                    break;
                case BackgroundTypeEnum.BingWallpaper:
                    if (args.PropertyName != nameof(ThemeConfigsService.BackgroundStretch)) {
                        return;
                    }
                    break;
                case BackgroundTypeEnum.SolidColor:
                    if (args.PropertyName != nameof(ThemeConfigsService.SolidColorBackgroundHex)) {
                        return;
                    }
                    break;
                default:
                    return;
            }
        }

        // Set background
        AppBackground = await themeService.SetBackground(themeService.BackgroundType);
    }
}
 
