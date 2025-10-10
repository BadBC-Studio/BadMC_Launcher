using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BadMC_Launcher.Classes;
using BadMC_Launcher.Classes.UI;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.ConfigsData;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Classes.DataClasses;
public class SingleMinecraftConfigs : ConfigClass {
    private readonly SingleMinecraftConfigsData singleMinecraftConfigInstance;
    private readonly PathService pathService = App.GetService<PathService>();

    [JsonConstructor]
    public SingleMinecraftConfigs(string TargetMinecraftEntryPath) {
        this.TargetMinecraftEntryPath = TargetMinecraftEntryPath;
        singleMinecraftConfigInstance = new(TargetMinecraftEntryPath);

        // Get info
        SyncSettingGet();

        JvmArguments.ListChanged += OnListChanged;
    }

    public string TargetMinecraftEntryPath {
        get => singleMinecraftConfigInstance.targetMinecraftEntryPath;
        set {
            if (singleMinecraftConfigInstance.targetMinecraftEntryPath != value) {
                singleMinecraftConfigInstance.targetMinecraftEntryPath = value;

                // Trigger Event
                OnPropertyChanged(nameof(TargetMinecraftEntryPath));

                // Write to Json or other logic
                SyncSettingSet();
            }
        }
    }

    public bool IsAutoJavaEnabled {
        get => singleMinecraftConfigInstance.isAutoJavaEnabled;
        set {
            if (singleMinecraftConfigInstance.isAutoJavaEnabled != value) {
                singleMinecraftConfigInstance.isAutoJavaEnabled = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoJavaEnabled));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsFullscreen {
        get => singleMinecraftConfigInstance.isFullscreen;
        set {
            if (singleMinecraftConfigInstance.isFullscreen != value) {
                singleMinecraftConfigInstance.isFullscreen = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsFullscreen));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public Size WindowSize {
        get => singleMinecraftConfigInstance.windowSize;
        set {
            if (singleMinecraftConfigInstance.windowSize != value) {
                singleMinecraftConfigInstance.windowSize = value;

                // Trigger Event
                OnPropertyChanged(nameof(WindowSize));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string VersionIsolationFilterId {
        get => singleMinecraftConfigInstance.versionIsolationFilterId;
        set {
            if (singleMinecraftConfigInstance.versionIsolationFilterId != value) {
                singleMinecraftConfigInstance.versionIsolationFilterId = value;

                // Trigger Event
                OnPropertyChanged(nameof(VersionIsolationFilterId));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public bool IsAutoMemorySize {
        get => singleMinecraftConfigInstance.isAutoMemorySize;
        set {
            if (singleMinecraftConfigInstance.isAutoMemorySize != value) {
                singleMinecraftConfigInstance.isAutoMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(IsAutoMemorySize));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint MaxMemorySize {
        get => singleMinecraftConfigInstance.maxMemorySize;
        set {
            if (singleMinecraftConfigInstance.maxMemorySize != value) {
                singleMinecraftConfigInstance.maxMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(MaxMemorySize));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public uint MinMemorySize {
        get => singleMinecraftConfigInstance.minMemorySize;
        set {
            if (singleMinecraftConfigInstance.minMemorySize != value) {
                singleMinecraftConfigInstance.minMemorySize = value;

                // Trigger Event
                OnPropertyChanged(nameof(MinMemorySize));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string JavaPath {
        get => singleMinecraftConfigInstance.javaPath;
        set {
            if (singleMinecraftConfigInstance.javaPath != value) {
                singleMinecraftConfigInstance.javaPath = value;

                // Trigger Event
                OnPropertyChanged(nameof(JavaPath));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public string LauncherName {
        get => singleMinecraftConfigInstance.launcherName;
        set {
            if (singleMinecraftConfigInstance.launcherName != value) {
                singleMinecraftConfigInstance.launcherName = value;

                // Trigger Event
                OnPropertyChanged(nameof(LauncherName));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public ServerInfo? LaunchServer {
        get => singleMinecraftConfigInstance.launchServer;
        set {
            if (singleMinecraftConfigInstance.launchServer != value) {
                singleMinecraftConfigInstance.launchServer = value;

                // Trigger Event
                OnPropertyChanged(nameof(LaunchServer));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    public BindingList<string> JvmArguments {
        get => singleMinecraftConfigInstance.jvmArguments;
        set {
            if (!singleMinecraftConfigInstance.jvmArguments.SequenceEqual(value)) {
                singleMinecraftConfigInstance.jvmArguments.RaiseListChangedEvents = false;

                singleMinecraftConfigInstance.jvmArguments.Clear();
                singleMinecraftConfigInstance.jvmArguments.AddRange(value);

                singleMinecraftConfigInstance.jvmArguments.RaiseListChangedEvents = true;

                // Trigger Event
                OnPropertyChanged(nameof(LaunchServer));

                // Write to Json
                SyncSettingSet();
            }
        }
    }

    private void OnListChanged(object? sender, ListChangedEventArgs e) {
        if (e.PropertyDescriptor != null) {
            OnPropertyChanged(e.PropertyDescriptor.Name);
        }

        SyncSettingSet();
    }

    public override bool SyncSettingGet() {
        if(TargetMinecraftEntryPath != null
            && File.Exists(Path.Combine(TargetMinecraftEntryPath, Path.Combine(AppDataPath.VersionConfigsPath, "MinecraftConfigs.json")))
            && pathService.TryReadConfig(Path.Combine(TargetMinecraftEntryPath, Path.Combine(AppDataPath.VersionConfigsPath, "MinecraftConfigs.json")), SingleMinecraftConfigsContext.Default.SingleMinecraftConfigs, out var jsonClass) && jsonClass != null) {
            singleMinecraftConfigInstance.isFullscreen = jsonClass.IsFullscreen;
            singleMinecraftConfigInstance.versionIsolationFilterId = jsonClass.VersionIsolationFilterId;
            singleMinecraftConfigInstance.isAutoMemorySize = jsonClass.IsAutoMemorySize;
            singleMinecraftConfigInstance.windowSize = jsonClass.WindowSize;
            singleMinecraftConfigInstance.minMemorySize = jsonClass.MinMemorySize;
            singleMinecraftConfigInstance.maxMemorySize = jsonClass.MaxMemorySize;
            singleMinecraftConfigInstance.javaPath = jsonClass.JavaPath;
            singleMinecraftConfigInstance.launcherName = jsonClass.LauncherName;
            singleMinecraftConfigInstance.jvmArguments = jsonClass.JvmArguments;
            return true;
        }
        return false;
    }

    public override bool SyncSettingSet() {
        if (TargetMinecraftEntryPath != null) {
            return pathService.WriteConfig(Path.Combine(TargetMinecraftEntryPath, Path.Combine(AppDataPath.VersionConfigsPath, "MinecraftConfigs.json")), SingleMinecraftConfigsContext.Default.SingleMinecraftConfigs, this);
        }
        return false;
    }
}

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(SingleMinecraftConfigs))]
internal partial class SingleMinecraftConfigsContext : JsonSerializerContext;
