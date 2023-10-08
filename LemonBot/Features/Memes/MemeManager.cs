using System.Text.Json;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LemonBot.Configurations;
using LemonBot.Utilities;

namespace LemonBot.Features.Memes;

public class MemeManager
{
    private DiscordSocketClient _client;
    private DailyMemes _memes;
    private MemeCommand _command;
    
    public MemeManager(DiscordSocketClient client)
    {
        _client = client;
        LoadMemes();
        _command = new MemeCommand(_client, this);
    }

    public void Start()
    {
        Task.Run(async () =>
        {
            Console.WriteLine("Starting daily memes");
            await _command.Register();
        });
    }

    public void AddMeme()
    {
        
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        
    }


    private void LoadMemes()
    {
        var path = "./memes.json";
        if (!File.Exists(path))
        {
            Logger.Warning("memes.json has been created. Please configure it for daily memes to work properly");
        }
    }
}