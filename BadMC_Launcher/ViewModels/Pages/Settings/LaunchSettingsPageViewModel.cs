using System.Collections.Generic;
using System.ComponentModel;
using BadMC_Launcher.Classes.DataClasses;
using BadMC_Launcher.Classes.UI;
using BadMC_Launcher.Controls;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Views.ContentDialogs.Settings;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Hardware.Info;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Utilities;
using Serilog;
using Uno.Extensions;
using Windows.UI.Popups;

namespace BadMC_Launcher.ViewModels.Pages.Settings;

public partial class LaunchSettingsPageViewModel : ObservableObject {
    private readonly CancellationTokenSource cancelLoopToken = new();
    private readonly XamlRoot? mainPageXamlRoot;
    private readonly MinecraftConfigsService minecraftService = App.GetService<MinecraftConfigsService>();
    private readonly ResourceLoader sourceService = App.GetService<ResourceLoader>();
    private readonly LaunchSettingsService launchSettingsService = App.GetService<LaunchSettingsService>();
    private readonly HardwareInfo systemInfo = App.GetService<HardwareInfo>();
    private bool isRangeSelectorDragStarted = false;
    private MinecraftFolderViewItem? minecraftFolder;
    private JavaEntry? java;
   
    public LaunchSettingsPageViewModel() {
        mainPageXamlRoot = SendGetValueMessage<XamlRoot?>(MessengerTokenEnum.MainPage_XamlRootToken).Response;
        minecraftFolder = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

        minecraftService.PropertyChanged += MinecraftConfig_PropertyChanged;

        MinecraftFolderId = minecraftFolder != null ? minecraftFolder.MinecraftFolderId : sourceService.GetString("Global_NullMinecraftFolderId");
        MinecraftFolderPath = minecraftFolder != null ? minecraftFolder.MinecraftFolderPath : sourceService.GetString("Global_NullMinecraftFolderPath");

        JavaId = sourceService.GetString("Global_NullJavaId");
        JavaPath = sourceService.GetString("Global_NullJavaPath");

        IsAutoGameMemorySize = minecraftService.IsAutoMemorySize;
        MinGameMemory = minecraftService.MinGameMemory;
        MaxGameMemory = minecraftService.MaxGameMemory;

        VersionIsolationFilters = launchSettingsService.VersionIsolationFilters;
        VersionIsolationFilterSelectedItem = launchSettingsService.VersionIsolationFilters.FirstOrDefault(item => item.Id == minecraftService.VersionIsolationFilterId) ?? VersionIsolationFilters[0];

        WindowWidth = minecraftService.WindowSize.Width;
        WindowHeight = minecraftService.WindowSize.Height;
        DefaultWindowSize = launchSettingsService.DefaultWindowSize;

        LauncherName = minecraftService.LauncherName;
        DefaultLauncherName = launchSettingsService.DefaultLauncherName;

        FullJvmArgumentsText = string.Join(" ", minecraftService.JvmArguments);

        RefreshMemory(cancelLoopToken.Token);

        GetJavaInfo();
    }

    // Minecraft folder settings
    [ObservableProperty]
    public partial string MinecraftFolderId { get; set; }

    [ObservableProperty]
    public partial string MinecraftFolderPath { get; set; }

    // Java settings
    [ObservableProperty]
    public partial string JavaId { get; set; }

    [ObservableProperty]
    public partial string JavaPath { get; set; }

    // Memory settings
    [ObservableProperty]
    public partial bool IsAutoGameMemorySize { get; set; }

    [ObservableProperty]
    public partial uint MaxMemoryView { get; set; }

    [ObservableProperty]
    public partial uint UsedMemoryView { get; set; }

    [ObservableProperty]
    public partial uint GameMemoryView { get; set; }

    [ObservableProperty]
    public partial uint MinGameMemory { get; set; }

    [ObservableProperty]
    public partial uint MaxGameMemory { get; set; }

    // Version isolation settings
    [ObservableProperty]
    public partial ObservableDataList<VersionIsolationFilter> VersionIsolationFilters { get; set; }

    [ObservableProperty]
    public partial VersionIsolationFilter? VersionIsolationFilterSelectedItem { get; set; }

    // Window settings
    [ObservableProperty]
    public partial bool IsFullscreen { get; set; }

    [ObservableProperty]
    public partial uint WindowWidth { get; set; }

    [ObservableProperty]
    public partial uint WindowHeight { get; set; }

    [ObservableProperty]
    public partial Size DefaultWindowSize { get; set; }

    // Launcher name settings
    [ObservableProperty]
    public partial string LauncherName { get; set; }

    [ObservableProperty]
    public partial string DefaultLauncherName { get; set; }

    // Jvm argument settings
    [ObservableProperty]
    public partial string FullJvmArgumentsText { get; set; }

    [RelayCommand]
    private void CancelLoop() {
        if (cancelLoopToken.Token.CanBeCanceled) {
            cancelLoopToken.Cancel();
        }
        
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task ShowMinecraftFolderManagerDialog() {
        if (mainPageXamlRoot != null) {
            var dialog = App.GetService<MinecraftFolderContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task ShowJavaManagerDialog() {
        if (mainPageXamlRoot != null) {
            var dialog = App.GetService<JavaContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    [RelayCommand]
    private void DisableNumberBoxEvent() {
        isRangeSelectorDragStarted = true;
    }

    [RelayCommand]
    private void EnableNumberBoxEvent() {
        isRangeSelectorDragStarted = false;
    }

    [RelayCommand]
    private void SetGameMemory() {
        if (isRangeSelectorDragStarted) {
            return;
        }
        // Set min memory
        minecraftService.MinGameMemory = MinGameMemory;

        // Set max memory
        minecraftService.MaxGameMemory = MaxGameMemory;
    }

    [RelayCommand]
    private void SetVersionIsolationFilterId() {
        minecraftService.VersionIsolationFilterId = VersionIsolationFilterSelectedItem?.Id ?? launchSettingsService.VersionIsolationFilters[0].Id;
    }

    [RelayCommand]
    private void SetLauncherName() {
        minecraftService.LauncherName = LauncherName;
    }

    [RelayCommand]
    private void OpenMultiPlayerPage() {
        // TODO: 等着吧（恼）

        App.GetService<NotificationService>().ShowNotification(new TipMessageNotificationItem(MessageSeverityEnum.Error, "草你怎么真点了我还没做完啊Σ(っ °Д °;)っ"));
    }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task OpenAddJvmArgumentsDialog() {
        if (mainPageXamlRoot != null) {
            var dialog = App.GetService<JvmArgumentsContentDialog>();
            dialog.XamlRoot = mainPageXamlRoot;

            await dialog.ShowAsync();
        }
    }

    partial void OnFullJvmArgumentsTextChanged(string value) {
        var items = FullJvmArgumentsText.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (items != null && items.Length > 0) {
            DistinctiveItemBindingList<string> strings = new DistinctiveItemBindingList<string>();
            strings.AddRange(items);

            if (!items.SequenceEqual(strings)) {
                FullJvmArgumentsText = string.Join(" ", strings);
            }

            minecraftService.JvmArguments = strings;
        }
    }

    partial void OnWindowWidthChanged(uint value) {
        minecraftService.WindowSize = new Size(value, WindowHeight);
    }

    partial void OnWindowHeightChanged(uint value) {
        minecraftService.WindowSize = new Size(WindowWidth, value);
    }

    partial void OnIsAutoGameMemorySizeChanged(bool value) {
        minecraftService.IsAutoMemorySize = value;

        if (value) {
            GameMemoryView = MaxMemoryView.GetAutoGameMemoryMb(UsedMemoryView);
        }
        else {
            GameMemoryView = MaxGameMemory;
        }
    }

    partial void OnIsFullscreenChanged(bool value) {
        minecraftService.IsFullscreen = value;
    }

    private async void GetJavaInfo() {
        java = await JavaUtil.GetJavaInfoAsync(minecraftService.ActiveJavaPath);

        // Set Java information
        JavaId = minecraftService.IsAutoJavaEnabled ?
             $"{sourceService.GetString("Global_AutoJavaPath")}" :
             java != null ? $"{java.JavaType} {java.JavaVersion}" : sourceService.GetString("Global_NullJavaId");
        JavaPath = java != null ? java.JavaPath : sourceService.GetString("Global_NullJavaPath");
    }

    // Refresh memory status
    private async void RefreshMemory(CancellationToken cancellationToken) {
        try {
            while (true) {
                systemInfo.RefreshMemoryStatus();

                MaxMemoryView = systemInfo.MemoryStatus.TotalPhysical.BytesToMb();
                UsedMemoryView = (systemInfo.MemoryStatus.TotalPhysical - systemInfo.MemoryStatus.AvailablePhysical).BytesToMb();

                if (IsAutoGameMemorySize) {
                    GameMemoryView = MaxMemoryView.GetAutoGameMemoryMb(UsedMemoryView);
                }
                else {
                    GameMemoryView = MaxGameMemory;
                }

                // Wait 1 second
                await Task.Delay(1000, cancellationToken);
            }
        }
        catch(TaskCanceledException ex) {
            Log.Information($"RefreshMemory method is Exited: {ex.Source}");
        }
    }

    

    // Send Message to get value
    private RequestMessage<T> SendGetValueMessage<T>(Enum tokenEnum) {
        return WeakReferenceMessenger.Default.Send(new RequestMessage<T>(), tokenEnum.ToString());
    }

    private void MinecraftConfig_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(MinecraftConfigsService.ActiveJavaPath):
            case nameof(MinecraftConfigsService.IsAutoJavaEnabled):
                if (minecraftService.ActiveJavaPath != JavaId || JavaId != sourceService.GetString("Global_NullJavaId")) {
                    GetJavaInfo();
                }
                break;
            case nameof(MinecraftConfigsService.ActiveMinecraftFolderPath):
            case nameof(MinecraftConfigsService.MinecraftFolders):
                if (minecraftService.ActiveMinecraftFolderPath != MinecraftFolderPath || minecraftService.MinecraftFolders.Any(item => item.MinecraftFolderId != MinecraftFolderId)) {
                    minecraftFolder = minecraftService.MinecraftFolders.FirstOrDefault(item => item.MinecraftFolderPath == minecraftService.ActiveMinecraftFolderPath);

                    MinecraftFolderId = minecraftFolder != null ? minecraftFolder.MinecraftFolderId : sourceService.GetString("Global_NullMinecraftFolderId");
                    MinecraftFolderPath = minecraftFolder != null ? minecraftFolder.MinecraftFolderPath : sourceService.GetString("Global_NullMinecraftFolderPath");
                }
                break;
            case nameof(MinecraftConfigsService.IsAutoMemorySize):
                if (minecraftService.IsAutoMemorySize != IsAutoGameMemorySize) {
                    IsAutoGameMemorySize = minecraftService.IsAutoMemorySize;
                }
                break;
            case nameof(MinecraftConfigsService.MinGameMemory):
                if (minecraftService.MinGameMemory != MinGameMemory) {
                    MinGameMemory = minecraftService.MinGameMemory;
                }
                break;
            case nameof(MinecraftConfigsService.MaxGameMemory):
                if (minecraftService.MaxGameMemory != MaxGameMemory) {
                    MaxGameMemory = minecraftService.MaxGameMemory;
                }
                break;
             case nameof(MinecraftConfigsService.VersionIsolationFilterId):
                if (minecraftService.VersionIsolationFilterId != VersionIsolationFilterSelectedItem?.Id) {
                    VersionIsolationFilterSelectedItem = launchSettingsService.VersionIsolationFilters.FirstOrDefault(item => item.Id == minecraftService.VersionIsolationFilterId) ?? VersionIsolationFilters[0];
                }
                break;
             case nameof(MinecraftConfigsService.IsFullscreen):
                if (minecraftService.IsFullscreen != IsFullscreen) {
                    IsFullscreen = minecraftService.IsFullscreen;
                }
                break;
             case nameof(MinecraftConfigsService.WindowSize):
                if (minecraftService.WindowSize.Width != WindowWidth || minecraftService.WindowSize.Height != WindowHeight) {
                    WindowWidth = minecraftService.WindowSize.Width;
                    WindowHeight = minecraftService.WindowSize.Height;
                }
                break;
             case nameof(MinecraftConfigsService.LauncherName):
                if (minecraftService.LauncherName != LauncherName) {
                    LauncherName = minecraftService.LauncherName;
                }
                break;
             case nameof(MinecraftConfigsService.JvmArguments):
                if (string.Join(" ", minecraftService.JvmArguments) != FullJvmArgumentsText) {
                    FullJvmArgumentsText = string.Join(" ", minecraftService.JvmArguments);
                }
                break;
        }
    }
}
