using System.Text.Json;
using LemonBot.Utilities;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace LemonBot.Configurations;

public abstract class ConfigFile
{
    public readonly string Path;
    
    public ConfigFile(string path)
    {
        Path = path;
    }
    
    public static void Save<T>(T config) where T : ConfigFile
    {
        File.WriteAllText(config.Path, JsonSerializer.Serialize(config, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));
    }

    public static bool Load<T>(T config) where T : ConfigFile
    {
        if (!File.Exists(config.Path))
        {
            Logger.Warning($"{config.Path} does not exist. Generating config.");
            Save(config);
            return false;
        }
        JsonConvert.PopulateObject(File.ReadAllText(config.Path), config);
        return true;
    }
}