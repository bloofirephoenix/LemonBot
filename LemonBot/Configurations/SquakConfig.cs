namespace LemonBot.Configurations;

public class SquakConfig : ConfigFile
{
    public ulong[] RandomSquakChannels = [];
    public int MinSquakTime = 0;
    public int MaxSquakTime = 0;
    public double FuckYouResponseChance = 0;

    public SquakConfig() : base("squak.json") { }
}
