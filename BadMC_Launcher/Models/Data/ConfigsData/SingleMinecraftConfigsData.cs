using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes.UI;
using BadMC_Launcher.Extensions;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Services.Settings;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Models.Data.ConfigsData;
internal class SingleMinecraftConfigsData {
    private readonly MinecraftConfigsService minecraftConfigsService = App.GetService<MinecraftConfigsService>();

    internal string targetMinecraftEntryPath;

    internal bool isAutoJavaEnabled;

    internal bool isFullscreen;
    internal Size windowSize;

    internal string versionIsolationFilterId;

    internal bool isAutoMemorySize;
    internal uint maxMemorySize;
    internal uint minMemorySize;

    internal string javaPath;

    internal string launcherName;
    internal ServerInfo? launchServer;

    internal BindingList<string> jvmArguments = new();

    public SingleMinecraftConfigsData(string minecraftEntryPath) {
        targetMinecraftEntryPath = minecraftEntryPath;

        isAutoJavaEnabled = minecraftConfigsService.IsAutoJavaEnabled;
        isFullscreen = minecraftConfigsService.IsFullscreen;
        windowSize = minecraftConfigsService.WindowSize;
        versionIsolationFilterId = minecraftConfigsService.VersionIsolationFilterId;
        isAutoMemorySize = minecraftConfigsService.IsAutoMemorySize;
        maxMemorySize = minecraftConfigsService.MaxGameMemory;
        minMemorySize = minecraftConfigsService.MinGameMemory;
        javaPath = minecraftConfigsService.ActiveJavaPath ?? string.Empty;
        launcherName = minecraftConfigsService.LauncherName ?? string.Empty;
        launchServer = minecraftConfigsService.LaunchServer;
        jvmArguments.AddRange(minecraftConfigsService.JvmArguments);
    }
}
