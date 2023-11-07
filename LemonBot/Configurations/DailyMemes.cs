namespace LemonBot.Configurations;

public class DailyMemes : ConfigFile
{
    public ulong Guild { get; set; } = 0;
    public ulong TextChannel { get; set; } = 0;
    
    public string MemeLocation { get; set; } = string.Empty;
    public string PostedLocation { get; set; } = string.Empty;
    public TimeSpan Time { get; set; } = TimeSpan.Zero;
    public int Day { get; set; }

    public string UniversalMessage { get; set; } = "day %day% of no pet rocket racing.";
    public List<Meme> Memes { get; set; } = new();

    public class Meme
    {
        public string File { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public DailyMemes() : base("memes.json") {}
}