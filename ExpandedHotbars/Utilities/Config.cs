using System.Threading.Tasks;

namespace ExpandedHotbars.Utilities;

/// <summary>
/// Configuration File Utilities
/// </summary>
public static class Config {
    public static string ConfigPath => FileHelpers.GetFileInfo("Configs").FullName;
    public static string CharacterConfigPath => FileHelpers.GetFileInfo("Configs", FileHelpers.GetCharacterPath()).FullName;

    /// <summary>
    /// Loads a configuration file from PluginConfigs\ExpandedHotbars\Configs\{FileName}
    /// Creates a `new T()` or uses passed in defaultValue object if the file can't be loaded
    /// </summary>
    public static async Task<T> LoadConfig<T>(string fileName, T? defaultValue = null) where T : class, new()
        => await FileHelpers.LoadFile(FileHelpers.GetFileInfo("Configs", fileName).FullName, defaultValue);

    /// <summary>
    /// Loads a character specific config file from PluginConfigs\ExpandedHotbars\Configs\{ContentId}\{FileName}
    /// Creates a `new T()` or uses passed in defaultValue object if the file can't be loaded
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static async Task<T> LoadCharacterConfig<T>(string fileName, T? defaultValue = null) where T : class, new()
        => await FileHelpers.LoadFile(FileHelpers.GetFileInfo("Configs", FileHelpers.GetCharacterPath(), fileName).FullName, defaultValue);

    /// <summary>
    /// Saves a configuration file to PluginConfigs\ExpandedHotbars\Configs\{FileName}
    /// </summary>
    public static async Task SaveConfig<T>(T configObject, string fileName)
        => await FileHelpers.SaveFile(configObject, FileHelpers.GetFileInfo("Configs", fileName).FullName);

    /// <summary>
    /// Saves a character specific config file to PluginConfigs\ExpandedHotbars\Configs\{ContentId}\{FileName}
    /// </summary>
    /// <remarks>Requires the character to be logged in</remarks>
    public static async Task SaveCharacterConfig<T>(T configObject, string fileName)
        => await FileHelpers.SaveFile(configObject, FileHelpers.GetFileInfo("Configs", FileHelpers.GetCharacterPath(), fileName).FullName);
}
