namespace LemonBot.Configurations;

public class DailyMemes
{
    public ulong Guild { get; set; } = 0;
    public ulong TextChannel { get; set; } = 0;
    
    public string MemeLocation { get; set; } = "";
    public string PostedLocation { get; set; } = "";
    public TimeSpan Time { get; set; } = TimeSpan.Zero;
    public int Day { get; set; } = 0;
    
    public string UniversalMessage { get; set; } = "day %day% of no pet rocket racing.";
    public Dictionary<int, string> MemeMessages { get; set; } = new()
    {
        {
            0, ""
        }
    };
}