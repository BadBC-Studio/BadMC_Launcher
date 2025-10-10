using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using MinecraftLaunch.Base.Models.Game;
using Newtonsoft.Json.Linq;
using Uno.Extensions.Specialized;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Controls.NotificationItem;

namespace BadMC_Launcher.ViewModels.UserControls;
public partial class LaunchPadViewModel : ObservableObject {
    private readonly MinecraftConfigsService minecraftService = App.GetService<MinecraftConfigsService>();
    private ResourceLoader resourceLoader = App.GetService<ResourceLoader>();

    public LaunchPadViewModel() {
        MinecraftFolderEntrys = minecraftService.MinecraftFolders.ToObservableCollection();
        MinecraftEntrys = new();

        //Check active Minecraft path
        if (minecraftService.ActiveMinecraftFolderPath != null) {
            // Set MinecraftFolderEntrysSelectedItem
            MinecraftFolderEntrysSelectedItem = MinecraftFolderEntrys.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

            // Set Minecraft entries
            if (MinecraftFolderEntrysSelectedItem != null) {
                // Get minecraftItems
                MinecraftEntrys = new(MinecraftFolderEntrysSelectedItem.GetMinecraftItems().ToList());

                // Set ActiveMinecraftEntry
                MinecraftEntrysSelectedItem = MinecraftEntrys.FirstOrDefault(item => item.MinecraftId == MinecraftFolderEntrysSelectedItem?.ActiveMinecraftEntryId);
            }
        }
        LaunchButtonMinecraftId = MinecraftEntrysSelectedItem?.MinecraftId ?? resourceLoader.GetString("LaunchPad_LaunchButtonTagDefaultResource");
        LaunchButtonMinecraftIcon = MinecraftEntrysSelectedItem?.MinecraftImage ?? new() { UriSource = new(@"ms-appx:///Assets/Icons/MinecraftIcons/drowned.png") };

        minecraftService.PropertyChanged += MinecraftConfig_PropertyChanged;

        IsMinecraftEntrysEmpty = !MinecraftEntrys.Any();
    }

    //LaunchButton Property
    [ObservableProperty]
    public partial string? LaunchButtonMinecraftId { get; set; }

    [ObservableProperty]
    public partial BitmapImage? LaunchButtonMinecraftIcon { get; set; }

    //MinecraftFolderPathsList
    [ObservableProperty]
    public partial ObservableCollection<MinecraftFolderViewItem> MinecraftFolderEntrys { get; set; }

    [ObservableProperty]
    public partial MinecraftFolderViewItem? MinecraftFolderEntrysSelectedItem { get; set; }

    //MinecraftsList
    [ObservableProperty]
    public partial ObservableCollection<MinecraftViewItem> MinecraftEntrys { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ViewLoaclMinecraftFolderCommand), nameof(LaunchMinecraftJavaCommand))]
    public partial MinecraftViewItem? MinecraftEntrysSelectedItem { get; set; }

    [ObservableProperty]
    public partial bool IsMinecraftEntrysEmpty { get; set; }

    [RelayCommand(CanExecute = nameof(SetIsNotActiveMinecraftEntryEmpty))]
    public void LaunchMinecraftJava() {
        App.GetService<NotificationService>().ShowNotification(new TipMessageNotificationItem(
            MessageSeverityEnum.Important,
            "Tip of the Day",
            "This is a tip to help you use the app better.",
            new FontIconSource() { Glyph = "\uE713" }));
    }

    // Invoke Refresh Command
    [RelayCommand]
    private void InvokeRefreshMinecraftEntrys() {
        RefreshMinecraftEntrys();
    }

    // Refresh MinecraftEntrys
    [RelayCommand]
    private void RefreshMinecraftEntrys(RefreshRequestedEventArgs args) {
        using (args.GetDeferral()) {
            RefreshMinecraftEntrys();
        }
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task OpenMinecraftFoldersManager() {
        var mainPageXamlRoot = SendGetValueMessage<XamlRoot?>(MessengerTokenEnum.MainPage_XamlRootToken).Response;

        if (mainPageXamlRoot != null) {

            var dialog = App.GetService<MinecraftFolderContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(SetIsNotActiveMinecraftEntryEmpty))]
    private void ViewLoaclMinecraftFolder() {
        if (MinecraftFolderEntrysSelectedItem != null) {
            App.GetService<PathService>().TryOpenFolderOrFileFromPath(MinecraftFolderEntrysSelectedItem.MinecraftFolderPath);
        }
    }

    [RelayCommand]
    private void ChangeActiveMinecraftFolder() {
        if (MinecraftFolderEntrysSelectedItem != null) {
            minecraftService.ActiveMinecraftFolderPath = MinecraftFolderEntrysSelectedItem.MinecraftFolderPath;

            // Get active minecraft path entry
            if (minecraftService.ActiveMinecraftFolderPath != null) {
                // Set MinecraftEntrys
                MinecraftEntrys = new(MinecraftFolderEntrysSelectedItem.GetMinecraftItems().ToList());

                // Set ActiveMinecraftEntry
                MinecraftEntrysSelectedItem = MinecraftEntrys.FirstOrDefault(item => item.MinecraftId == MinecraftFolderEntrysSelectedItem.ActiveMinecraftEntryId);
            }
            else {
                // Init MinecraftEntrys
                MinecraftEntrys = new();
            }
        }
    }

    partial void OnMinecraftEntrysSelectedItemChanged(MinecraftViewItem? value) {
        if (value != null) {
            // Get ActiveMinecraftEntry
            var minecraftFolderEntry = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

            if (minecraftFolderEntry != null) {
                minecraftFolderEntry.ActiveMinecraftEntryId = value?.MinecraftId;
            }
        }
        LaunchButtonMinecraftId = MinecraftEntrysSelectedItem?.MinecraftId ?? resourceLoader.GetString("LaunchPad_LaunchButtonTagDefaultResource"); ;
        LaunchButtonMinecraftIcon = MinecraftEntrysSelectedItem?.MinecraftImage ?? new() { UriSource = new(@"ms-appx:///Assets/Icons/MinecraftIcons/drowned.png")};
    }

    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs args) {
        var selectedItem = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

        // Update Property
        switch (args.PropertyName) {
            case nameof(MinecraftConfigsService.ActiveMinecraftFolderPath):
                if (MinecraftFolderEntrysSelectedItem
                        ?.MinecraftFolderPath != minecraftService.ActiveMinecraftFolderPath) {
                    // Set MinecraftFolderEntries SelectedIndex
                    MinecraftFolderEntrysSelectedItem = MinecraftFolderEntrys.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath); 

                    // Update MinecraftEntrys
                    MinecraftEntrys.Clear();
                    MinecraftEntrys.AddRange(selectedItem?.GetMinecraftItems());

                    MinecraftEntrysSelectedItem = MinecraftEntrys.FirstOrDefault(item => item.MinecraftId == selectedItem?.ActiveMinecraftEntryId);
                }
                
                break;
            case nameof(MinecraftConfigsService.MinecraftFolders):
                if (!MinecraftFolderEntrys.SequenceEqual(minecraftService.MinecraftFolders)) {
                    // Update MinecraftFolderEntrys
                    MinecraftFolderEntrys = minecraftService.MinecraftFolders.ToObservableCollection();

                    // Get ActiveMinecraftEntry
                    MinecraftFolderEntrysSelectedItem = MinecraftFolderEntrys.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

                    // Update MinecraftEntrys
                    MinecraftEntrys.Clear();
                    MinecraftEntrys.AddRange(selectedItem?.GetMinecraftItems());

                    MinecraftEntrysSelectedItem = MinecraftEntrys.FirstOrDefault(item => item.MinecraftId == selectedItem?.ActiveMinecraftEntryId);
                }
                break;
        }

        // Set whether to display the empty list prompt
        IsMinecraftEntrysEmpty = !MinecraftEntrys.Any();
    }

    //Set CanExecute for ViewLoaclMinecraftFolderCommand
    private bool SetIsNotActiveMinecraftEntryEmpty() => MinecraftEntrysSelectedItem != null;

    private void RefreshMinecraftEntrys() {
        var selectedItem = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);
        if (selectedItem != null) {
            // Get minecraftItems
            MinecraftEntrys = new(selectedItem.GetMinecraftItems().ToList());

            // Set ActiveMinecraftEntry
            MinecraftEntrysSelectedItem = MinecraftEntrys.FirstOrDefault(item => item.MinecraftId == selectedItem.ActiveMinecraftEntryId);

            return;
        }

        // Clear MinecraftEntrys
        MinecraftEntrys.Clear();
    }

    partial void OnMinecraftEntrysChanged(ObservableCollection<MinecraftViewItem> value) {
        IsMinecraftEntrysEmpty = !MinecraftEntrys.Any();
    }

    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }
}
