using Discord.WebSocket;
using LemonBot.Configurations;
using LemonBot.Utilities;

namespace LemonBot.Features.Memes;

public class MemeManager
{
    public static bool Started { get; private set; }
    private DiscordSocketClient _client;
    private DailyMemes _memes;
    private MemeCommand _command;
    
    public MemeManager(DiscordSocketClient client)
    {
        _client = client;
        _memes = new();
        ConfigFile.Load(_memes);
        _command = new MemeCommand(_client, this);
    }

    public void Start()
    {
        if (Started)
        {
            Logger.Warning("Tried to start the MemeManager but it is already running");
            return;
        }
        
        Started = true;
        
        Task.Run(async () =>
        {
            Console.WriteLine("Starting daily memes");
            await _command.Register();

            while (true)
            {
                // schedule the next meme
                DateTime now = DateTime.Now;
                var nextRun = now.Date + _memes.Time;

                if (nextRun < now)
                {
                    nextRun = nextRun.AddDays(1);
                }
            
                var dif = nextRun - DateTime.Now;
                Console.WriteLine($@"Scheduling the next meme for {nextRun.ToString(TimeFormat.Format)} in {dif}");
                await Task.Delay(dif);
                
                // send the meme
                var meme = _memes.Memes[0];
                _memes.Memes.Remove(meme);

                _memes.Day++;

                await _client.GetGuild(_memes.Guild).GetTextChannel(_memes.TextChannel).SendFileAsync(meme.File, text: meme.Message);
                
                // move the meme to posted folder
                File.Move(meme.File, $"posted/{_memes.Day}.png");
            }
        });
        Logger.Info("MemeManager Stopped");
        Started = false;
    }

    public void AddMeme(DailyMemes.Meme meme, bool skipLine)
    {
        if (skipLine)
            _memes.Memes.Insert(0, meme);
        else
            _memes.Memes.Add(meme);
        
        ConfigFile.Save(_memes);
    }
}