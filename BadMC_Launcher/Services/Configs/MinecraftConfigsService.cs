using System.ComponentModel;
using System.Text.Json.Serialization;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Classes.UI;
using BadMC_Launcher.Controls.Minecraft;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.Mappings;
using BadMC_Launcher.Models.Data.ConfigsData;
using BadMC_Launcher.Models.Enums;
using MinecraftLaunch.Base.Models.Authentication;
using BadMC_Launcher.Classes.DataClasses;
using MinecraftLaunch.Base.Models.Game;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Services.Configs;
using Hardware.Info;

namespace BadMC_Launcher.Services.Configs;
public class MinecraftConfigsService : ConfigClass {
    private PathService pathService;
    private LaunchSettingsService launchSettingsService;
    private HardwareInfo systemInfo;

    internal bool IsSyncEnabled = false;

    public MinecraftConfigsService(PathService _pathService, LaunchSettingsService _launchSettingsService, HardwareInfo _hardwareInfo) {
        pathService = _pathService;
        launchSettingsService = _launchSettingsService;
        systemInfo = _hardwareInfo;

        //Triggers an event when a property is changed
        MinecraftConfigs.minecraftAccounts.ListChanged += OnListChanged<Account>;
        MinecraftConfigs.javaPaths.ListChanged += OnListChanged<string>;
        MinecraftConfigs.minecraftFolders.ListChanged += OnListChanged<MinecraftFolderViewItem>;
        MinecraftConfigs.jvmArguments.ListChanged += OnListChanged<string>;
    }

    public MinecraftConfigsService() {
        pathService = App.GetService<PathService>();
        launchSettingsService = App.GetService<LaunchSettingsService>();
        systemInfo = App.GetService<HardwareInfo>();

        //Triggers an event when a property is changed
        MinecraftConfigs.minecraftAccounts.ListChanged += OnListChanged<Account>;
        MinecraftConfigs.javaPaths.ListChanged += OnListChanged<string>;
        MinecraftConfigs.minecraftFolders.ListChanged += OnListChanged<MinecraftFolderViewItem>;
        MinecraftConfigs.jvmArguments.ListChanged += OnListChanged<string>;
    }

    public DistinctiveItemBindingList<Account> MinecraftAccounts {
        get => MinecraftConfigs.minecraftAccounts;
        set {
            if (!MinecraftConfigs.minecraftAccounts.SequenceEqual(value)) {
                MinecraftConfigs.minecraftAccounts.Clear();
                MinecraftConfigs.minecraftAccounts.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }
    public DistinctiveItemBindingList<string> JavaPaths {
        get {
            // Check if the Java folder exists
            foreach (var item in MinecraftConfigs.javaPaths.ToList()) {
                if (!Path.Exists(item)) {
                    MinecraftConfigs.javaPaths.Remove(item);
                }
            }
            
            return MinecraftConfigs.javaPaths;
        }
        set {
            if (!MinecraftConfigs.javaPaths.SequenceEqual(value)) {
                MinecraftConfigs.javaPaths.Clear();
                MinecraftConfigs.javaPaths.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }
    public DistinctiveItemBindingList<MinecraftFolderViewItem> MinecraftFolders {
        get {
            // Check if the Minecraft folder exists
            foreach (var item in MinecraftConfigs.minecraftFolders.ToList()) {
                if (!Path.Exists(item.MinecraftFolderPath)) {
                    MinecraftConfigs.minecraftFolders.Remove(item);
                }
            }

            return MinecraftConfigs.minecraftFolders;
        }
        set {
            if (!MinecraftConfigs.minecraftFolders.SequenceEqual(value)) {
                MinecraftConfigs.minecraftFolders.Clear();
                MinecraftConfigs.minecraftFolders.MargeItems(value);

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string? ActiveJavaPath {
        get => MinecraftConfigs.activeJavaPath;
        set {
            // Check folder is existing
            if (!Path.Exists(value)) {
                MinecraftConfigs.activeJavaPath = string.Empty;
            }
            else {
                MinecraftConfigs.activeJavaPath = value;
            }

            // Trigger Event
            OnPropertyChanged(nameof(ActiveJavaPath));

            //Write to Json
            SyncSettingSet();
        }
    }

    public string? ActiveMinecraftFolderPath {
        get => MinecraftConfigs.activeMinecraftFolder;
        set {
            // Check folder is existing
            if (!Path.Exists(value)) {
                MinecraftConfigs.activeMinecraftFolder = string.Empty;
            }
            else {
                MinecraftConfigs.activeMinecraftFolder = value;
            }

            // Trigger Event
            OnPropertyChanged(nameof(ActiveMinecraftFolderPath));

            //Write to Json
            SyncSettingSet();
        }
    }

    public Account? ActiveMinecraftAccount {
        get => MinecraftConfigs.activeMinecraftAccount;
        set {
            if (MinecraftConfigs.activeMinecraftAccount != value) {
                MinecraftConfigs.activeMinecraftAccount = value;

                // Trigger Event
                OnPropertyChanged(nameof(ActiveMinecraftAccount));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsAutoJavaEnabled {
        get => MinecraftConfigs.isAutoJavaEnabled;
        set {
            if (MinecraftConfigs.isAutoJavaEnabled != value) {
                MinecraftConfigs.isAutoJavaEnabled = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoJavaEnabled));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsFullscreen {
        get => MinecraftConfigs.isFullscreen;
        set {
            if (MinecraftConfigs.isFullscreen != value) {
                MinecraftConfigs.isFullscreen = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsFullscreen));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public Size WindowSize {
        get => MinecraftConfigs.windowSize;
        set {
            if (MinecraftConfigs.windowSize != value) {
                MinecraftConfigs.windowSize = new Size(value.Width != 0 ? value.Width : launchSettingsService.DefaultWindowSize.Width,
                    value.Height != 0 ? value.Height : launchSettingsService.DefaultWindowSize.Height);

                // Trigger Event
                OnPropertyChanged(nameof(WindowSize));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string VersionIsolationFilterId {
        get => MinecraftConfigs.versionIsolationFilterId;
        set {
            if (MinecraftConfigs.versionIsolationFilterId != value) {
                MinecraftConfigs.versionIsolationFilterId = value;

                // Trigger Event
                OnPropertyChanged(nameof(VersionIsolationFilterId));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsAutoMemorySize {
        get => MinecraftConfigs.isAutoMemorySize;
        set {
            if (MinecraftConfigs.isAutoMemorySize != value) {
                MinecraftConfigs.isAutoMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoMemorySize));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint MaxGameMemory {
        get => MinecraftConfigs.maxGameMemory;
        set {
            if (MinecraftConfigs.maxGameMemory != value) {
                // Check max game memory
                systemInfo.RefreshMemoryStatus();
                if (value == 0 || value > systemInfo.MemoryStatus.TotalPhysical.BytesToMb() || value <= MinecraftConfigs.minGameMemory) {

                    OnPropertyChanged(nameof(MaxGameMemory));
                    return;
                }

                MinecraftConfigs.maxGameMemory = value;

                // Trigger Event
                OnPropertyChanged(nameof(MaxGameMemory));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint MinGameMemory {
        get => MinecraftConfigs.minGameMemory;
        set {
            if (MinecraftConfigs.minGameMemory != value) {
                // Check min game memory
                systemInfo.RefreshMemoryStatus();
                    OnPropertyChanged(nameof(MinGameMemory));
                    return;
            }

            MinecraftConfigs.minGameMemory = value;

            // Trigger Event
            OnPropertyChanged(nameof(MinGameMemory));

            //Write to Json
            SyncSettingSet();

        }
    }

    public string LauncherName {
        get => MinecraftConfigs.launcherName;
        set {
            if (MinecraftConfigs.launcherName != value) {
                if (string.IsNullOrWhiteSpace(value)) {
                    MinecraftConfigs.launcherName = launchSettingsService.DefaultLauncherName;
                }
                else {
                    MinecraftConfigs.launcherName = value;
                }

                // Trigger Event
                OnPropertyChanged(nameof(LauncherName));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public ServerInfo? LaunchServer {
        get => MinecraftConfigs.launcherServer;
        set {
            if (MinecraftConfigs.launcherServer != value) {
                MinecraftConfigs.launcherServer = value;

                // Trigger Event
                OnPropertyChanged(nameof(LaunchServer));

                //Write to Json
                SyncSettingSet();
            }
        }
    }

    public DistinctiveItemBindingList<string> JvmArguments {
        get => MinecraftConfigs.jvmArguments;
        set {
            if (!MinecraftConfigs.jvmArguments.SequenceEqual(value)) {
                MinecraftConfigs.jvmArguments.RaiseListChangedEvents = false;

                MinecraftConfigs.jvmArguments.Clear();
                MinecraftConfigs.jvmArguments.AddRange(value);

                MinecraftConfigs.jvmArguments.RaiseListChangedEvents = true;

                // Trigger Event
                OnPropertyChanged(nameof(JvmArguments));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    private void OnListChanged<T>(object? sender, ListChangedEventArgs e) {
        if (sender is DistinctiveItemBindingList<T> senderList && !string.IsNullOrWhiteSpace(senderList.PropertyName)) {
            switch (senderList.PropertyName) {
                case nameof(MinecraftFolders):
                    if (MinecraftFolders.All(item => item.MinecraftFolderPath != ActiveMinecraftFolderPath) && !string.IsNullOrWhiteSpace(ActiveMinecraftFolderPath)) {
                        ActiveMinecraftFolderPath = string.Empty;
                    }
                    break;
                case nameof(JavaPaths):
                    if (JavaPaths.All(item => item != ActiveJavaPath) && !string.IsNullOrWhiteSpace(ActiveJavaPath)) {
                        ActiveJavaPath = string.Empty;
                    }
                    break;
                case nameof(MinecraftAccounts):
                    if (MinecraftAccounts.All(item => item.Uuid != ActiveMinecraftAccount?.Uuid) && ActiveMinecraftAccount != null) {
                        ActiveMinecraftAccount = null;
                    }
                    break;
            }
            // Notify list changed
            OnPropertyChanged(senderList.PropertyName);
        }
        else {
            if (e.PropertyDescriptor != null) {
                OnPropertyChanged(e.PropertyDescriptor.Name);
            }
        }

        SyncSettingSet();
    }

    public override bool SyncSettingGet() {
        if (pathService.TryReadConfig(Path.Combine(AppDataPath.pathsList["ConfigsPath"], "MinecraftConfigs.json"), MinecraftConfigsServiceContext.Default.MinecraftConfigsService, out var jsonClass, UpdateMapping.minecraftConfigPropertyNameMapping, UpdateMapping.minecraftConfigPropertyTypeMapping) && jsonClass != null) {
            //TODO: 解蜜
            MinecraftConfigs.activeJavaPath = jsonClass.ActiveJavaPath;
            MinecraftConfigs.activeMinecraftFolder = jsonClass.ActiveMinecraftFolderPath;
            //MinecraftConfig.activeMinecraftAccount = jsonClass.ActiveMinecraftAccount;
            MinecraftConfigs.isAutoJavaEnabled = jsonClass.IsAutoJavaEnabled;

            MinecraftConfigs.isFullscreen = jsonClass.IsFullscreen;
            MinecraftConfigs.windowSize = jsonClass.WindowSize;

            MinecraftConfigs.versionIsolationFilterId = jsonClass.VersionIsolationFilterId;

            MinecraftConfigs.isAutoMemorySize = jsonClass.IsAutoMemorySize;
            MinecraftConfigs.minGameMemory = jsonClass.MinGameMemory;
            MinecraftConfigs.maxGameMemory = jsonClass.MaxGameMemory;

            MinecraftConfigs.launcherName = jsonClass.LauncherName;
            MinecraftConfigs.launcherServer = jsonClass.LaunchServer;

            return true;
        }
        return false;
    }

    public override bool SyncSettingSet() {
        if (IsSyncEnabled == false) {
            return false;
        }
        MinecraftConfigsService classValue = this;
        //TODO: 加蜜
        return pathService.WriteConfig(Path.Combine(AppDataPath.pathsList["ConfigsPath"], "MinecraftConfigs.json"), MinecraftConfigsServiceContext.Default.MinecraftConfigsService, classValue);
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(MinecraftConfigsService))]
internal partial class MinecraftConfigsServiceContext : JsonSerializerContext;
