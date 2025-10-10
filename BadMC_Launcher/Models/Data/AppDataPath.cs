namespace BadMC_Launcher.Models.Data;
public static class AppDataPath {
    public static readonly string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BadMC_Launcher");

    public static readonly string VersionConfigsPath = Path.Combine(dataPath, "LaunchConfigs");

    public static readonly Dictionary<string, string> pathsList = new Dictionary<string, string>() {
        { "ConfigsPath", Path.Combine(dataPath, "Configs") },
        { "LogsPath", Path.Combine(dataPath, "Logs") },
        { "AssetsPath", Path.Combine(dataPath, "Assets") }
    };
}
