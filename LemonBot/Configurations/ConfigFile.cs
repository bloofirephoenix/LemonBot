using LemonBot.Utilities;

namespace LemonBot.Configurations;

public abstract class ConfigFile
{
    public readonly string Path;
    
    public ConfigFile(string path)
    {
        Path = path;
    }
    
    public void Save()
    {
        if (!File.Exists(Path))
        {
            Logger.Warning($"{Path} does not exist. Generating config.");
            
        }
    }
}