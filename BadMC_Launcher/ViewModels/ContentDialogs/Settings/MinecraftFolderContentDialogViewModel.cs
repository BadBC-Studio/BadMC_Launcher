using System.Collections.ObjectModel;
using System.ComponentModel;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Services.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.UI.Xaml.Input;
using Windows.Storage.Pickers;
using Windows.System;
using WinRT.Interop;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;
public partial class MinecraftFolderContentDialogViewModel : ObservableObject {
    private readonly MinecraftConfigsService minecraftConfigService = App.GetService<MinecraftConfigsService>();

    public MinecraftFolderContentDialogViewModel() {
        MinecraftFoldersList = minecraftConfigService.MinecraftFolders.ToObservableCollection();
        MinecraftFoldersListSelectedItem = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == minecraftConfigService.ActiveMinecraftFolderPath);

        OldRenameMinecraftFolderId = string.Empty;
        NewRenameMinecraftFolderId = string.Empty;
        RenameMinecraftFolderPath = string.Empty;

        minecraftConfigService.PropertyChanged += MinecraftConfig_PropertyChanged;

        IsMinecraftFoldersListEmpty = !MinecraftFoldersList.Any();
    }

    [ObservableProperty]
    public partial ObservableCollection<MinecraftFolderViewItem> MinecraftFoldersList { get; set; }

    [ObservableProperty]
    public partial bool IsMinecraftFoldersListEmpty { get; set; }

    [ObservableProperty]
    public partial bool IsCanRename { get; set; }

    [ObservableProperty]
    public partial MinecraftFolderViewItem? MinecraftFoldersListSelectedItem { get; set; }

    [ObservableProperty]
    public partial string OldRenameMinecraftFolderId { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ApplyRenameCommand))]

    public partial string NewRenameMinecraftFolderId { get; set; }

    [ObservableProperty]
    public partial string RenameMinecraftFolderPath { get; set; }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task AddMinecraftFolder() {
        // Create bew folder picker
        FolderPicker folderPicker = new FolderPicker();

        // Get window handle
        var hwnd = WindowNative.GetWindowHandle(App.Current.MainWindow);
        InitializeWithWindow.Initialize(folderPicker, hwnd);

        // Show folder picker dialog
        StorageFolder folder = await folderPicker.PickSingleFolderAsync();

        if (folder != null) {
            if (minecraftConfigService.MinecraftFolders.Any(item => item.MinecraftFolderPath == folder.Path)) {
                App.GetService<NotificationService>().ShowNotification(new TipNotificationItem(
                    MessageSeverityEnum.Warning,
                    App.GetService<ResourceLoader>().GetString("TipNotification_MinecraftFolderDuplicationTitle")));
                return;
            }

            var item = new MinecraftFolderViewItem(folder.Path);

            // Add folder to list
            minecraftConfigService.MinecraftFolders.Add(item);

            // Set new folder name
            OldRenameMinecraftFolderId = item.MinecraftFolderId;
            RenameMinecraftFolderPath = folder.Path;
            SendInvokeFuncMessage(string.Empty, MessengerTokenEnum.MinecraftFolderContentDialog_ShowRenameFlyoutToken);
        }
    }

    [RelayCommand]
    private void OpenMinecraftFolder() {
        if (string.IsNullOrWhiteSpace(OldRenameMinecraftFolderId)) {
            // Open folder
            App.GetService<PathService>().TryOpenFolderOrFileFromPath(RenameMinecraftFolderPath);
        }
    }

    [RelayCommand]
    private void TryApplyRename(KeyRoutedEventArgs args) {
        if (args.Key == VirtualKey.Enter) {
            // Get minecraft folder entry
            var minecraftPathEntry = minecraftConfigService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == RenameMinecraftFolderPath);
            if (minecraftPathEntry != null) {
                // Check if folder with new name already exists
                if (CanRename()) {
                    // Rename folder
                    minecraftPathEntry.MinecraftFolderId = NewRenameMinecraftFolderId;

                    // Hide Flyout
                    HideRenameFlyout();
                }
            }
        }
    }

    [RelayCommand(CanExecute = nameof(CanRename))]
    private void ApplyRename() {
        // Get minecraft folder entry
        var minecraftPathEntry = minecraftConfigService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == RenameMinecraftFolderPath);
        if (minecraftPathEntry != null) {
            // Check if folder with new name already exists
            if (CanRename()) {
                // Rename folder
                minecraftPathEntry.MinecraftFolderId = NewRenameMinecraftFolderId;

                // Hide Flyout
                HideRenameFlyout();
            }
        }
    }

    [RelayCommand]
    private void DeleteRenameFlyoutData() {
        // Clear data
        RenameMinecraftFolderPath = string.Empty;
        OldRenameMinecraftFolderId = string.Empty;
        NewRenameMinecraftFolderId = string.Empty;
    }

    [RelayCommand]
    private void SetActiveMinecraftFolder(SelectionChangedEventArgs args) {
        var selectedItem = MinecraftFoldersListSelectedItem;
        if (args.AddedItems.Any(item => { 
            if (item is MinecraftFolderViewItem entry) { 
                return entry.MinecraftFolderPath != minecraftConfigService.ActiveMinecraftFolderPath;
            }
            return false;
        })) {
            minecraftConfigService.ActiveMinecraftFolderPath = selectedItem?.MinecraftFolderPath;
        }
    }

    [RelayCommand]
    private void ViewFolderInLocal(string parameter) {
        App.GetService<PathService>().TryOpenFolderOrFileFromPath(parameter);
    }

    [RelayCommand]
    private void RenameItem(string parameter) {
        var folderEntryPath = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == parameter)?.MinecraftFolderId;

        if (folderEntryPath != null) {
            SendInvokeFuncMessage(string.Empty, MessengerTokenEnum.MinecraftFolderContentDialog_ShowRenameFlyoutToken);
            // Set new folder name
            OldRenameMinecraftFolderId = folderEntryPath;
            RenameMinecraftFolderPath = parameter;
        }
    }

    [RelayCommand]
    private void DeleteItem(string parameter) {
        var deleteItem = minecraftConfigService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == parameter);
        if (deleteItem != null && minecraftConfigService.MinecraftFolders.Remove(deleteItem)) {
            return;
        }
    }



    [RelayCommand]
    private void HideRenameFlyout() => SendInvokeFuncMessage(string.Empty, MessengerTokenEnum.MinecraftFolderContentDialog_HideRenameFlyoutToken);

    private ValueChangedMessage<T> SendInvokeFuncMessage<T>(T value, Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new ValueChangedMessage<T>(value), tokenEnum.ToString());
    }

    private bool CanRename() {
        // Check if text is not empty and not equal to old folder name
        if (!string.IsNullOrWhiteSpace(NewRenameMinecraftFolderId) && minecraftConfigService.MinecraftFolders.All(item => item.MinecraftFolderId != NewRenameMinecraftFolderId)) {
            // Set apply button enabled
            IsCanRename = true;
            return true;
        }

        // Set apply button disabled
        IsCanRename = true;
        return false;
    }

    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        // Update Property
        switch (e.PropertyName) {
            case nameof(MinecraftConfigsService.ActiveMinecraftFolderPath):
                if (minecraftConfigService.ActiveMinecraftFolderPath != MinecraftFoldersListSelectedItem?.MinecraftFolderPath) {
                    MinecraftFoldersListSelectedItem = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == minecraftConfigService.ActiveMinecraftFolderPath);
                }
                break;
            case nameof(MinecraftConfigsService.MinecraftFolders):
                if (!MinecraftFoldersList.SequenceEqual(minecraftConfigService.MinecraftFolders)) {
                    MinecraftFoldersList = minecraftConfigService.MinecraftFolders.ToObservableCollection();
                    MinecraftFoldersListSelectedItem = MinecraftFoldersList.FirstOrDefault(item => item.MinecraftFolderPath == minecraftConfigService.ActiveMinecraftFolderPath);
                }
                break;
        }

        // Set whether to display the empty list prompt
        IsMinecraftFoldersListEmpty = !MinecraftFoldersList.Any();
    }

    partial void OnMinecraftFoldersListChanged(ObservableCollection<MinecraftFolderViewItem> value) {
        IsMinecraftFoldersListEmpty = !MinecraftFoldersList.Any();
    }
}
