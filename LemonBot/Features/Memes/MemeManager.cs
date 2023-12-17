using System.Runtime.InteropServices;
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
                Console.WriteLine($"Scheduling the next meme for {nextRun.ToString(TimeFormat.Format)} in {dif}");
                await Task.Delay(dif);

                var dm = await _client.GetUser(259483144934260755).CreateDMChannelAsync();
                // send the meme
                if (_memes.Memes.Count > 0)
                {
                    var meme = _memes.Memes[0];
                    _memes.Memes.Remove(meme);
                    await _client.GetGuild(_memes.Guild).GetTextChannel(_memes.TextChannel).SendFileAsync(meme.File,
                        text: $"{_memes.UniversalMessage.Replace("%day%", _memes.Day.ToString())} {meme.Message}");

                    // move the meme to posted folder
                    try
                    {
                        Directory.CreateDirectory(_memes.PostedLocation);
                        File.Move(meme.File, $"{_memes.PostedLocation}/{_memes.Day}{Path.GetExtension(meme.File)}");
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                        await dm.SendMessageAsync($"``An error occurred moving the meme #{_memes.Day} ({meme.File})``");
                    }
                }
                
                // send the number of memes left to me in a dm
                await dm.SendMessageAsync($"``{_memes.Memes.Count}`` memes left in the queue.");

                _memes.Day++;
                ConfigFile.Save(_memes);
            }
        });
    }

    public void AddMeme(DailyMemes.Meme meme, bool skipLine)
    {
        if (skipLine)
            _memes.Memes.Insert(0, meme);
        else
            _memes.Memes.Add(meme);
        
        ConfigFile.Save(_memes);
    }

    public string GetNewMemeFileLocation(string extension)
    {
        return $"{_memes.MemeLocation}/{Guid.NewGuid()}.{extension}";
    }
}