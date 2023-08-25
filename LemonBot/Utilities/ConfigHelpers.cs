using System.Text.Json;

namespace LemonBot.Utilities;

public static class ConfigHelpers
{
    /// <summary>
    /// Attempts to load the config specified
    /// </summary>
    /// <returns>If the config was successfully loaded or not</returns>
    public static bool InitializeConfig<T>(string path, out T config) where T : new()
    {
        config = new T();
        if (!File.Exists(path))
        {
            Logger.Debug($"Creating config {path}");
            SaveConfig(config, path);
            return false;
        }
        var tempConfig = JsonSerializer.Deserialize<T>(File.ReadAllText(path));
        if (tempConfig == null)
            return false;

        config = tempConfig;
        return true;
    }

    public static void SaveConfig<T>(T config, string path)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
    
        File.WriteAllText(path, JsonSerializer.Serialize(config, options));
    }
}