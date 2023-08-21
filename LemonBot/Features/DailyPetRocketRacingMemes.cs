using Discord;
using Discord.WebSocket;
using LemonBot.Configurations;
using LemonBot.Utilities;

namespace LemonBot.Features;

public class DailyPetRocketRacingMemes
{
    private DailyMemes _config;
    private const string ConfigPath = "memes.json";
    private readonly DiscordSocketClient _client;

    public DailyPetRocketRacingMemes(DiscordSocketClient client)
    {
        _client = client;
        _config = new();
    }

    public void Start()
    {
        if (!ConfigHelpers.InitializeConfig(ConfigPath, out _config))
        {
            Logger.Warning("Memes are disabled. Please fill out memes.json");
            return;
        }

        Task.Run(ScheduleTask);
    }

    private async Task ScheduleTask()
    {
        while (true)
        {
            DateTime now = DateTime.Now;
            var nextRun = now.Date + _config.Time;

            if (nextRun < now)
            {
                nextRun = nextRun.AddDays(1);
            }
            
            var dif = nextRun - DateTime.Now;
            Console.WriteLine($@"Scheduling the next meme for {nextRun.ToString(TimeFormat.Format)} in {dif}");
            await Task.Delay(dif);
            
            if (!ConfigHelpers.InitializeConfig(ConfigPath, out _config))
            {
                Logger.Error("Failed to read memes.json. Memes are disabled!");
                return;
            }
            
            _config.Day++;
            Console.WriteLine($"Sending daily pet rocket racing meme day {_config.Day}");

            if (!_config.Memes.ContainsKey(_config.Day))
            {
                Logger.Warning($"No daily meme for day {_config.Day}!");
                continue;
            }
            
            var meme = _config.Memes[_config.Day];
            
            // send the meme
            await _client.GetGuild(_config.Guild).GetTextChannel(_config.TextChannel).SendFileAsync(
                attachment: new FileAttachment(meme.Image),
                text: $"{_config.UniversalMessage.Replace("%day%", _config.Day.ToString())} {meme.Message}"
            );

            var movePath = Path.Join("posted", meme.Image);

            if (!Directory.Exists(Path.GetDirectoryName(movePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(movePath)!);
            
            File.Move(meme.Image, Path.Join("posted", meme.Image));
            
            _config.Memes.Remove(_config.Day);
            ConfigHelpers.SaveConfig(_config, ConfigPath);
        }
    }
}