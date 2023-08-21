namespace LemonBot.Configurations;

public class DailyMemes
{
    public ulong Guild { get; set; } = 0;
    public ulong TextChannel { get; set; } = 0;
    
    public TimeSpan Time { get; set; } = TimeSpan.Zero;
    public int Day { get; set; } = 0;
    public Dictionary<int, Meme> Memes { get; set; } = new()
    {
        {
            0, 
            new Meme()
            {
            Image = "",
            Message = ""
            }
        }
    };

    public string UniversalMessage { get; set; } = "day %day% of no pet rocket racing.";

    public class Meme
    {
        public string Image { get; set; } = "";
        public string Message { get; set; } = "";
    }
}