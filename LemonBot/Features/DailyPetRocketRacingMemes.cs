using System.Net;
using System.Text.Json;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LemonBot.Configurations;
using LemonBot.Utilities;

namespace LemonBot.Features;

public class DailyPetRocketRacingMemes
{
    /*private DailyMemes _config = new();
    private const string ConfigPath = "memes.json";
    private readonly DiscordSocketClient _client;
    private HttpClient _http = new();

    public DailyPetRocketRacingMemes(DiscordSocketClient client)
    {
        _client = client;
    }

    public void Start()
    {
        Console.WriteLine("Starting daily pet rocket racing memes");
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

            var image = Path.Join(_config.MemeLocation, $"{_config.Day}.png");
            
            if (!File.Exists(image))
            {
                Logger.Warning($"No daily meme for day {_config.Day}!");
                continue;
            }

            _config.MemeMessages.TryGetValue(_config.Day, out var message);

            if (message == null)
                message = "";
            
            // send the meme
            await _client.GetGuild(_config.Guild).GetTextChannel(_config.TextChannel).SendFileAsync(
                attachment: new FileAttachment(image),
                text: $"{_config.UniversalMessage.Replace("%day%", _config.Day.ToString())} {message}"
            );

            var movePath = Path.Join(_config.PostedLocation, $"{_config.Day}.png");

            if (!Directory.Exists(Path.GetDirectoryName(movePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(movePath)!);
            
            File.Move(image, movePath);
            
            _ = _config.MemeMessages.Remove(_config.Day);
            ConfigHelpers.SaveConfig(_config, ConfigPath);
        }
    }*/
}