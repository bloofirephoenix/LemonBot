using System.Text.Json;

namespace LemonBot.Utilities;

public static class ConfigHelpers
{
    public static T? InitializeConfig<T>(string path) where T : new()
    {
        if (!File.Exists(path))
        {
            Logger.Debug($"Creating config {path}");
            var defaultConfig = new T();
            SaveConfig(defaultConfig, path);
            
            return default;
        }
        return JsonSerializer.Deserialize<T>(File.ReadAllText(path));
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