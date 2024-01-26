using System.Runtime.InteropServices;
using Discord.WebSocket;
using LemonBot.Configurations;
using LemonBot.Utilities;
using static LemonBot.Configurations.DailyMemes;

namespace LemonBot.Features.Memes;

public class MemeManager
{
    private Task? _task;
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
        if (_task != null && !_task.IsCompleted)
        {
            Logger.Warning("Tried to start the MemeManager but it is already running");
            return;
        }

        _task = Task.Run(async () =>
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
                Console.WriteLine("It's meme time");

                var dm = await _client.GetUser(259483144934260755).CreateDMChannelAsync();

                if (dm == null) 
                    Logger.Error("DM channel is null");

                // send the meme
                if (_memes.Memes.Count > 0)
                {
                    var meme = _memes.Memes[0];
                    _memes.Memes.Remove(meme);

                    int tries = 0;
                    while (tries < 60)
                    {
                        try
                        {
                            Console.WriteLine("Sending the meme");
                            await _client.GetGuild(_memes.Guild).GetTextChannel(_memes.TextChannel).SendFileAsync(
                                meme.File,
                                text: $"{_memes.UniversalMessage.Replace("%day%", _memes.Day.ToString())} {meme.Message}");

                            if (dm != null) 
                                await dm.SendMessageAsync($"``{_memes.Memes.Count}`` memes left in the queue.");

                            Console.WriteLine("Meme sent!");

                            break;
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e.ToString());
                            tries++;
                            await Task.Delay(1000 * 60);
                        }
                    }

                    if (tries < 60)
                    {
                        // move the meme to posted folder
                        try
                        {
                            Console.WriteLine("Moving meme");
                            Directory.CreateDirectory(_memes.PostedLocation);
                            File.Move(meme.File, $"{_memes.PostedLocation}/{_memes.Day}{Path.GetExtension(meme.File)}");
                            Console.WriteLine("Meme moved!");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString());
                            if (dm != null)
                                await dm.SendMessageAsync(
                                    $"``An error occurred moving the meme #{_memes.Day} ({meme.File})``");
                        }
                    }
                }

                try
                {
                    Console.WriteLine("Incrementing Day");
                    _memes.Day++;
                    ConfigFile.Save(_memes);
                    Console.WriteLine("Day incremented!");
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                    if (dm != null)
                        await dm.SendMessageAsync($"``Error incrementing day``");
                }
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