using System.Text.Json.Serialization;

namespace LemonBot.Configurations;

public class Config : ConfigFile
{
    public string Discord { get; set; } = "";

    public Config() : base("config.json")
    {
    }
}