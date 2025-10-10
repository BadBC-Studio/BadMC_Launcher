using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Extensions;
using Microsoft.Windows.ApplicationModel.Resources;
using MinecraftLaunch.Base.Models.Authentication;
using MinecraftLaunch.Base.Models.Game;
using BadMC_Launcher.Controls.Minecraft;
using System.ComponentModel;
using BadMC_Launcher.Services.Settings;
using BadMC_Launcher.Classes.Comparers;
using BadMC_Launcher.Models.Enums;
using BadMC_Launcher.Services.Configs;
using BadMC_Launcher.Classes.UI;

namespace BadMC_Launcher.Models.Data.ConfigsData;
internal static class MinecraftConfigs {
    private static LaunchSettingsService launchSettingsService = App.GetService<LaunchSettingsService>();

    internal static DistinctiveItemBindingList<Account> minecraftAccounts = new() { PropertyName = nameof(MinecraftConfigsService.MinecraftAccounts) };

    internal static DistinctiveItemBindingList<string> javaPaths = new(new JavaPathListComparer()) { PropertyName = nameof(MinecraftConfigsService.JavaPaths) };

    internal static DistinctiveItemBindingList<MinecraftFolderViewItem> minecraftFolders = new() { PropertyName = nameof(MinecraftConfigsService.MinecraftFolders) };

    internal static bool isAutoJavaEnabled = false;

    internal static string? activeMinecraftFolder;

    internal static Account? activeMinecraftAccount;

    internal static string? activeJavaPath;

    internal static bool isFullscreen = false;

    internal static Size windowSize = launchSettingsService.DefaultWindowSize;

    internal static string versionIsolationFilterId = launchSettingsService.VersionIsolationFilters[0].Id;

    internal static bool isAutoMemorySize = true;

    internal static uint maxGameMemory = 1024;

    internal static uint minGameMemory = 512;

    internal static string launcherName = launchSettingsService.DefaultLauncherName;

    internal static ServerInfo? launcherServer = null;

    internal static DistinctiveItemBindingList<string> jvmArguments = new();
}
