using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BadMC_Launcher.Classes.DataClasses;
using BadMC_Launcher.Classes.UI;
using BadMC_Launcher.Controls;
using MinecraftLaunch.Base.Models.Game;

namespace BadMC_Launcher.Services.Settings;

public class LaunchSettingsService {
    private readonly ResourceLoader resourceService;

    public LaunchSettingsService(ResourceLoader _resourceService) {
        resourceService = _resourceService;

        DefaultLauncherName = resourceService.GetString("Global_DefaultLauncherNameResource");

        DefaultJvmArgments.AddRange([
            new JvmArgumentItem() {
                Argument = "--demo",
                ViewIcon = new FontIconSource () { Glyph = "\uEC74" },
                TipText =  resourceService.GetString("DefaultJvmArgments_Demo")
            },
        ]);

        VersionIsolationFilters.AddRange([
            new VersionIsolationFilter() {
                Id = "Base_DisabledVersionFilter",
                ViewName = resourceService.GetString("VersionIsolationFilter_DisabledFilterViewName"),
                ViewIcon = new FontIconSource() { Glyph = "\uECCA" },
                IsVersionIsolation = (entry) => false
            },
            new VersionIsolationFilter() {
                Id = "Base_EnabledVersionFilter",
                ViewName = resourceService.GetString("VersionIsolationFilter_EnabledFilterViewName"),
                ViewIcon = new FontIconSource() { Glyph = "\uE930" },
                IsVersionIsolation = (entry) => true
            },

            new VersionIsolationFilter() {
                Id = "Base_ModLoaderVersionFilter",
                ViewName = resourceService.GetString("VersionIsolationFilter_ModLoaderFilterViewName"),
                ViewIcon = new FontIconSource() { Glyph = "\uECCD" },
                IsVersionIsolation = (entry) => entry is ModifiedMinecraftEntry modifiedEntry &&
                modifiedEntry.ModLoaders.Any(item => item.Type != MinecraftLaunch.Base.Enums.ModLoaderType.OptiFine ||
                item.Type != MinecraftLaunch.Base.Enums.ModLoaderType.Unknown)
            },
            new VersionIsolationFilter() {
                Id = "Base_ModifiedVersionFilter",
                ViewName = resourceService.GetString("VersionIsolationFilter_ModifiedFilterViewName"),
                ViewIcon = new FontIconSource() { Glyph = "\uEC7A" },
                IsVersionIsolation = (entry) => entry is ModifiedMinecraftEntry
            },
            new VersionIsolationFilter() {
                Id = "Base_NotReleaseVersionFilter",
                ViewName = resourceService.GetString("VersionIsolationFilter_NotReleaseFilterViewName"),
                ViewIcon = new FontIconSource() { Glyph = "\uE81E" },
                IsVersionIsolation = (entry) => entry.IsVanilla
            },
        ]);
    }
    public ObservableDataList<VersionIsolationFilter> VersionIsolationFilters { get; } = new ObservableDataList<VersionIsolationFilter>();
    
    public Size DefaultWindowSize { get; } = new Size(854, 480);

    public string DefaultLauncherName { get; }

    public ObservableDataList<JvmArgumentItem> DefaultJvmArgments { get; } = new ObservableDataList<JvmArgumentItem>();
}
