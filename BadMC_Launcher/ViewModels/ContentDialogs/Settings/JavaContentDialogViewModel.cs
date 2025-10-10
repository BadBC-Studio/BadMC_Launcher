using System.ComponentModel;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Services.Settings;
using CommunityToolkit.Mvvm.Input;
using MinecraftLaunch.Base.Models.Game;
using MinecraftLaunch.Utilities;
using Windows.Storage.Pickers;
using Uno.Extensions.Specialized;
using WinRT.Interop;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Models.Enums;

namespace BadMC_Launcher.ViewModels.ContentDialogs.Settings;

public partial class JavaContentDialogViewModel : ObservableObject {
    private readonly MinecraftConfigsService minecraftConfigService;
    private Task? setJavaListTask;

    public JavaContentDialogViewModel() {
        minecraftConfigService = App.GetService<MinecraftConfigsService>();

        minecraftConfigService.PropertyChanged += JavaList_PropertyChanged;

        IsAutoJavaEnabled = minecraftConfigService.IsAutoJavaEnabled;
        IsJavasListLoading = false;

        // Initialize List
        JavasList = [];

        setJavaListTask = SetJavaList();
    }

    [ObservableProperty]
    public partial bool IsJavasListLoading { get; set; }

    [ObservableProperty]
    public partial bool IsAutoJavaEnabled { get; set; }

    [ObservableProperty]
    public partial ObservableDataList<JavaViewItem> JavasList { get; set; }

    [ObservableProperty]
    public partial JavaViewItem? JavasListSelectedItem { get; set; }

    [RelayCommand(FlowExceptionsToTaskScheduler = true)]
    private async Task AddJava() {
        // Create new file picker
        var filePicker = new FileOpenPicker();

        // Get window handle
        var windowHandle = WindowNative.GetWindowHandle(App.Current.MainWindow);
        InitializeWithWindow.Initialize(filePicker, windowHandle);

        if (OperatingSystem.IsWindows()) {
            filePicker.FileTypeFilter.Add(".exe");
        }

        // Show file picker dialog
        var file = await filePicker.PickSingleFileAsync();
        var status = false;

        var isItemDuplication = false;

        if (string.IsNullOrWhiteSpace(file.DisplayName)) {
            return;
        }

        switch (file) {
            case { DisplayName: "java" }:
                isItemDuplication = minecraftConfigService.JavaPaths.Contains(file.Path);
                minecraftConfigService.JavaPaths.Add(file.Path);
                status = true;
                break;
            case { DisplayName: "javaw" }: 
                var javaFolder = Directory.GetParent(file.Path);

                if (javaFolder != null) {
                    if (OperatingSystem.IsWindows()) {
                        isItemDuplication = minecraftConfigService.JavaPaths.Contains(@$"{javaFolder.FullName}\java{file.FileType}");
                        minecraftConfigService.JavaPaths.Add(@$"{javaFolder.FullName}\java{file.FileType}");
                        status = true;
                        break;
                    }

                    isItemDuplication = minecraftConfigService.JavaPaths.Contains($"{javaFolder.FullName}/java{file.FileType}");
                    minecraftConfigService.JavaPaths.Add($"{javaFolder.FullName}/java{file.FileType}");
                    status = true;
                }
                break;
        }

        if (!status) {
            // Show error toast if the selected file is not a valid Java executable
            App.GetService<NotificationService>().ShowNotification(new TipNotificationItem(
                MessageSeverityEnum.Error,
                App.GetService<ResourceLoader>().GetString("TipNotification_JavaNameErrorTitle"),
                App.GetService<ResourceLoader>().GetString("TipNotification_JavaNameErrorMessage")));
        }
        else if (isItemDuplication) {
            // Show duplication toast
            App.GetService<NotificationService>().ShowNotification(new TipNotificationItem(
                MessageSeverityEnum.Warning,
                App.GetService<ResourceLoader>().GetString("TipNotification_JavaDuplicationTitle")));
        }

    }

    [RelayCommand]
    private void DownloadJava(string parameter) {
        
    }

    [RelayCommand]
    private async Task SearchJavas() {
        IAsyncEnumerable<JavaEntry>? javas = JavaUtil.EnumerableJavaAsync();
        if (javas != null) {
            var javaList = await Task.Run(async () => await javas.ToListAsync());

            if (!javaList.All(item => JavasList.Contains(item.JavaPath))) {
                JavasList.MargeItems(javaList.Select(item => new JavaViewItem(item)));
                minecraftConfigService.JavaPaths.MargeItems(javaList.Select(item => item.JavaPath));
                OnPropertyChanged();
                return;
            }
        }
    }

    [RelayCommand]
    private void SetActiveJava() {
        var isCompleted = setJavaListTask?.IsCompleted;
        if (isCompleted is bool isTaskCompleted && isTaskCompleted) {
            // Get selected item
            var selectedItem = JavasListSelectedItem;

            // Set active Java path
            minecraftConfigService.ActiveJavaPath = minecraftConfigService.JavaPaths.FirstOrDefault(item => item == selectedItem?.JavaPath);
        }
    }

    [RelayCommand]
    private void ChangeIsAutoJavaEnabled() {
        minecraftConfigService.IsAutoJavaEnabled = IsAutoJavaEnabled;
    }

    [RelayCommand]
    private void DeleteJava(string parameter) {
        // Check if parameter is null or empty
        if (!minecraftConfigService.JavaPaths.Contains(parameter)) {
            return;
        }

        
        var deleteItem = JavasList.FirstOrDefault(item => item.JavaPath == parameter);
        if (deleteItem != null) {
            // Remove Java path
            JavasList.Remove(deleteItem);
            minecraftConfigService.JavaPaths.Remove(deleteItem.JavaPath);
            return;
        }
    }

    [RelayCommand]
    private void LocalViewJava(string parameter) {
        App.GetService<PathService>().TryOpenFolderOrFileFromPath(parameter);
    }

    private async Task SetJavaList() {
        if (!minecraftConfigService.JavaPaths.Any()) {
            return;
        }

        IsJavasListLoading = true;

        var javaEntries = await Task.Run(async () => {
            // Convert list
            var list = new List<JavaEntry>();
            foreach (var item in minecraftConfigService.JavaPaths) {
                list.Add(await JavaUtil.GetJavaInfoAsync(item));
            }
            return list;
        });

        // Clear list
        JavasList.Clear();

        // Add items
        foreach (var item in javaEntries) {
            JavasList.Add(new JavaViewItem(item));
        }

        // Find selected Java
        JavasListSelectedItem = JavasList.FirstOrDefault(item => item.JavaPath == minecraftConfigService.ActiveJavaPath);

        IsJavasListLoading = false;
    }

    private void JavaList_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        switch(e.PropertyName) {
            case nameof(MinecraftConfigsService.JavaPaths):
                if (!minecraftConfigService.JavaPaths.SequenceEqual(JavasList.Select(item => item.JavaPath))) {
                    // Update Java list
                    setJavaListTask = SetJavaList();
                }
                break;
            case nameof(MinecraftConfigsService.ActiveJavaPath):
                if (minecraftConfigService.ActiveJavaPath != JavasListSelectedItem?.JavaPath) {
                    // Update selected index
                    JavasListSelectedItem = JavasList.FirstOrDefault(item => item.JavaPath == minecraftConfigService.ActiveJavaPath);
                }
                break;
        }
    }
}
