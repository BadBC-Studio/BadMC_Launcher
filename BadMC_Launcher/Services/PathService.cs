using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using BadMC_Launcher.Controls.NotificationItem;
using BadMC_Launcher.Models.Data;
using BadMC_Launcher.Models.Data.ConfigsData;
using BadMC_Launcher.Models.Data.Mappings;
using BadMC_Launcher.Models.Enums;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic.FileIO;
using MinecraftLaunch.Extensions;
using Uno.UI.Helpers;

namespace BadMC_Launcher.Services;

public class PathService {
    private readonly NotificationService notificationService;
    private readonly ResourceLoader resourceLoader;
    private readonly bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    private readonly HashSet<string> windowsPathReservedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "CON", "PRN", "AUX", "NUL",
        "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
        "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    public PathService(NotificationService _notificationService, ResourceLoader _resourceLoader) {
        notificationService = _notificationService;
        resourceLoader = _resourceLoader;
    }

    /// <summary>
    /// Check if the file path is valid.
    /// </summary>
    /// <param name="path">The path of the string to be checked.</param>
    /// <param name="isCheckPathPointing">If enabled, the path is additionally checked to see if it points to a valid file.</param>
    /// <returns>Returns true if the path is valid and absolute.</returns>
    public bool CheckPath(string path) {
        try {
            // Segment path
            var parts = path.Split([Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar],
                StringSplitOptions.RemoveEmptyEntries);

            if (IsPathInvalid(path)) {
                return false;
            }

            foreach (var part in parts) {
                if (isWindows) {
                    // Check is has invalid chars or Name
                    if (part.EndsWith('.') ||
                        part.EndsWith(' ') ||
                        windowsPathReservedNames.Contains(Path.GetFileNameWithoutExtension(part))) {
                        return false;
                    }
                }
                else {
                    if (part.StartsWith('-')) {
                        return false;
                    }
                }
            }
            return true;
        }
        catch (Exception ex) {
            ShowErrorToast(ex, path);
        }
        return false;
    }

    /// <summary>
    /// Basic path checking (not recommended)
    /// </summary>
    /// <param name="path">The path of the string to be checked.</param>
    /// <returns>Returns true if the path is valid and absolute.</returns>
    public bool IsPathInvalid(string path) {
        var rootPath = Path.GetPathRoot(path);
        // Check chars is ASCII.
        return path.Any(char.IsControl)
               // Check string is empty or path has invalid path char.
               || string.IsNullOrWhiteSpace(path)
               || path.IndexOfAny(Path.GetInvalidPathChars()) >= 0
               || !Path.IsPathFullyQualified(path)
               //Check is not ready drive path.
               || ( rootPath != null && !new DriveInfo(rootPath).IsReady )
               // Check is too long or device path.
               || ( isWindows && ( path.Length > 260
                   || path.StartsWith(@"\\.\")
                   || path.StartsWith(@"\\?\") ) );
    }

    /// <summary>
    /// Tries to read a json file and deserialize it to the specified class.
    /// </summary>
    /// <param name="filePath">Valid json file paths</param>
    /// <param name="jsonTypeInfo">Contains information about the class to be deserialized.</param>
    /// <param name="returnValue">If deserialization is successful, return the deserialized class, if not, return the default value of the class (never use the default value!)</param>
    /// <param name="propertyNameMapping">If a property name is changed after an update, the incorrect property name can be mapped to the correct property name by mapping.</param>
    /// <param name="propertyValueTypeMapping">If an update results in a change in the type of an attribute value, the incorrect attribute value type can be mapped to the correct attribute value type through mapping.</param>
    /// <returns>Returns a boolean indicating whether the Json file was deserialized successfully.</returns>
    public bool TryReadConfig<T>(string filePath, JsonTypeInfo<T> jsonTypeInfo, out T? returnValue, Dictionary<string, string>? propertyNameMapping = null, Dictionary<string, Func<object, object>>? propertyValueTypeMapping = null) {
        if (CheckPath(filePath) && Path.Exists(filePath)) {
            try {
                var fileValue = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileValue)) {
                    returnValue = fileValue.Deserialize(jsonTypeInfo);
                    return true;
                }
            }
            catch (Exception ex) {
                switch (ex) {
                    case JsonException:
                        if (TryChangeConfigWithMappings(filePath, jsonTypeInfo, propertyNameMapping, propertyValueTypeMapping)) {
                            TryReadConfig(filePath, jsonTypeInfo, out _, propertyNameMapping, propertyValueTypeMapping);
                        }
                        else {
                            if (!ShowErrorToast(ex, filePath)) {
                                throw;
                            }
                        }
                        break;
                    default:
                        if (!ShowErrorToast(ex, filePath)) {
                            throw;
                        }
                        break;
                }
            }
        }
        returnValue = default;
        return false;
    }

    /// <summary>
    /// Tries to read a Json file from a valid path and returns a JsonElement type.
    /// </summary>
    /// <param name="filePath">Valid json file paths</param>
    /// <param name="returnValue">If the deserialization succeeds, return a usable JsonElement instance, if it doesn't, return the default value of the JsonElement (never use the default value!)</param>
    /// <returns>Returns a boolean value indicating whether the Json file was read successfully.</returns>
    public bool TryReadConfigToJsonElement(string filePath, out JsonElement? returnValue) {
        if (CheckPath(filePath) && Path.Exists(filePath)) {
            try {
                var fileValue = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileValue)) {
                    var jsonValue = JsonDocument.Parse(fileValue);
                    if (jsonValue != null) {
                        returnValue = jsonValue.RootElement;
                        return true;
                    }
                }

            }
            catch (Exception ex) {
                ShowErrorToast(ex, filePath);
            }
        }
        returnValue = default;
        return false;
    }

    /// <summary>
    /// Serialize a class into a Json file.
    /// </summary>
    /// <param name="filePath">Valid json file paths</param>
    /// <param name="JsonTypeInfo">Contains information about the class to be serialized.</param>
    /// <returns>Returns a boolean value indicating whether the class has been properly serialized into the Json file.</returns>
    public bool WriteConfig<T>(string filePath, JsonTypeInfo<T> jsonTypeInfo, T value) {
        if (CheckPath(filePath)) {
            if (!File.Exists(filePath)) {
                File.Create(filePath);
            }
            try {
                var jsonValue = value.Serialize(jsonTypeInfo);
                File.WriteAllText(filePath, jsonValue);
                return true;
            }
            catch (Exception ex) {
                if (!ShowErrorToast(ex, filePath)) {
                    throw;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Try to update the Json file using the mapping table. (You must provide the propertyNameMapping or valueTypeMapping parameter to use this).
    /// </summary>
    /// <param name="filePath">Valid json file paths</param>
    /// <param name="jsonTypeInfo">Contains information about the class to be deserialized.</param>
    /// <param name="propertyNameMapping">If a property name is changed after an update, the incorrect property name can be mapped to the correct property name by mapping.</param>
    /// <param name="propertyValueTypeMapping">If an update results in a change in the type of an attribute value, the incorrect attribute value type can be mapped to the correct attribute value type through mapping.</param>
    /// <exception cref="ArgumentNullException">This exception is thrown when both propertyNameMapping and propertyValueTypeMapping are null.</exception>
    /// <returns>Returns a boolean indicating whether the wrong Json field was found and modified and serialized back into the Json file.</returns>
    public bool TryChangeConfigWithMappings<T>(string filePath, JsonTypeInfo<T> jsonTypeInfo, Dictionary<string, string>? propertyNameMapping, Dictionary<string, Func<object, object>>? propertyValueTypeMapping) {
        if (propertyNameMapping == null && propertyValueTypeMapping == null) {
            throw new ArgumentNullException($"{nameof(propertyNameMapping)}, {nameof(propertyValueTypeMapping)}", "propertyNameMapping and valueTypeMapping cannot both be null.");
        }
        try {
            if (CheckPath(filePath) && Path.Exists(filePath)) {
                var content = File.ReadAllText(filePath);

                if (string.IsNullOrWhiteSpace(content)) {
                    return false;
                }

                using JsonDocument doc = JsonDocument.Parse(content);
                JsonObject? root = JsonObject.Create(doc.RootElement);
                if (root == null) return false;

                bool modified = ProcessJsonNode(root, propertyNameMapping, propertyValueTypeMapping);

                if (modified) {
                    File.WriteAllText(filePath, root.ToJsonString(new() { WriteIndented = true }));
                    return true;
                }
            }
        }
        catch(Exception ex) {
            ShowErrorToast(ex, filePath);
        }
        return false;
    }

    /// <summary>
    /// Try using your system's file manager to open the folder or file pointed to by the path.
    /// </summary>
    /// <param name="path">Valid json file paths</param>
    /// <returns>Returns a value indicating whether the folder or file has been opened.</returns>
    public bool TryOpenFolderOrFileFromPath(string path) {
        try {
            // TODO: NOT WORK ON WINDOWS 10!
            if (CheckPath(path) && Path.Exists(path)) {
                using (Process.Start(new ProcessStartInfo(path) {
                    UseShellExecute = true,
                    Verb = "open"
                })) {
                    return true;
                }
            }
        }
        catch (Exception ex) {
            switch (ex) {
                case Win32Exception:
                    notificationService.ShowNotification(new TipMessageNotificationItem(
                    MessageSeverityEnum.Error,
                    resourceLoader.GetString("ToastNotification_Win32ErrorTitle"),
                    $"{path}\n\n{ex.Message}"));
                    break;
                case FileNotFoundException:
                    notificationService.ShowNotification(new TipMessageNotificationItem(
                    MessageSeverityEnum.Error,
                    resourceLoader.GetString("ToastNotification_FileNotFoundErrorTitle"),
                    $"{path}\n\n{ex.Message}"));
                    break;
                default:
                    throw;
            }
        }
        return false;
    }

    private bool ProcessJsonNode(JsonNode node, Dictionary<string, string>? updateMapping, Dictionary<string, Func<object, object>>? valueTypeMapping) {
        var DeleteItems = new Dictionary<string, string>();
        bool isModified = false;
        try {
            switch (node) {
                case JsonObject obj:
                    // Loop to find nested objects
                    foreach (var prop in obj.ToList()) {
                        if (prop.Value != null && ProcessJsonNode(prop.Value, updateMapping, valueTypeMapping)) {
                            isModified = true;
                        }

                        // Modify the key name to correspond to the mapping
                        if (updateMapping != null && prop.Value != null && updateMapping.TryGetValue(prop.Key, out var newKey)) {
                            if (newKey == "_Delete") {
                                DeleteItems.Add(prop.Key, prop.Value.ToJsonString(new() {
                                    WriteIndented = true,
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                                }));
                                obj.Remove(prop.Key);
                            }
                            else {
                                obj.Add(newKey, prop.Value.DeepClone());
                                obj.Remove(prop.Key);
                                isModified = true;
                            }
                        }

                        // Modify the key value type to correspond to the mapping
                        if (valueTypeMapping != null && prop.Value != null && valueTypeMapping.TryGetValue(prop.Key, out var newMappingValue)) {
                            obj[prop.Key] = JsonSerializer.SerializeToNode(newMappingValue(prop.Value));
                            isModified = true;
                        }
                    }
                    break;
                case JsonArray arr:
                    // If Array
                    for (int i = 0; i < arr.Count; i++) {
                        if (arr[i] is JsonNode element && ProcessJsonNode(element, updateMapping, valueTypeMapping)) {
                            // Update Modified
                            arr[i] = element.DeepClone();
                            isModified = true;
                        }
                    }
                    break;
            }
        }
        catch(Exception ex) {
            ShowErrorToast(ex);
        }
        return isModified;
    }
    private bool ShowErrorToast(Exception ex, string filePath = "") {
        switch (ex) {
            case JsonException:
                notificationService.ShowNotification(new TipMessageNotificationItem(
                    MessageSeverityEnum.Error,
                    resourceLoader.GetString("ToastNotification_ReadJsonErrorTitle"),
                    $"{resourceLoader.GetString("ToastNotification_JsonErrorMessage")}\n{filePath}\n\n{ex.Message}") {
                    PrimaryActionButton = string.IsNullOrWhiteSpace(filePath) ? new Button() {
                        Content = resourceLoader.GetString("ToastNotification_JsonErrorOpenJson_ButtonText"),
                        Command = new RelayCommand(() => TryOpenFolderOrFileFromPath(filePath))
                    } : null,
                    SecondaryActionButton = string.IsNullOrWhiteSpace(filePath) ? new Button() {
                        Content = resourceLoader.GetString("ToastNotification_JsonErrorDeleteJson_ButtonText"),
                        Command = new RelayCommand(() => {
                            if (File.Exists(filePath)) {
                                try {
                                    FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                                }
                                catch (Exception ex) {
                                    ShowErrorToast(ex, filePath);
                                }
                            }
                        })
                    } : null
                });
                break;
            case SecurityException:
            case UnauthorizedAccessException:
                notificationService.ShowNotification(new TipMessageNotificationItem(
                    MessageSeverityEnum.Error,
                    resourceLoader.GetString("ToastNotification_UnauthorizedAccessErrorTitle"),
                    $"{resourceLoader.GetString("ToastNotification_UnauthorizedAccessErrorMessage")}\n{filePath}\n\n{ex.Message}"));
                break;
            case PathTooLongException:
                notificationService.ShowNotification(new TipMessageNotificationItem(
                   MessageSeverityEnum.Error,
                   resourceLoader.GetString("ToastNotification_PathTooLongErrorTitle"),
                   $"{resourceLoader.GetString("ToastNotification_PathTooLongErrorMessage")}\n{filePath}\n\n{ex.Message}"));
                break;
            case IOException:
                notificationService.ShowNotification(new TipMessageNotificationItem(
                   MessageSeverityEnum.Error,
                   resourceLoader.GetString("ToastNotification_IOErrorTitle"),
                   $"{resourceLoader.GetString("ToastNotification_IOErrorMessage")}\n{filePath}\n\n{ex.Message}"));
                break;
            default:
                return false;
        }
        return true;
    }
}
