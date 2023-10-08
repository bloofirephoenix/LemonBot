using System.Text.Json;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LemonBot.Utilities;

namespace LemonBot.Features.Memes;

public class MemeCommand
{
    private readonly DiscordSocketClient _client;
    private readonly MemeManager _memes;
    
    public MemeCommand(DiscordSocketClient client, MemeManager memes)
    {
        _client = client;
        _memes = memes;
    }

    public async Task Register()
    {
        var command = new SlashCommandBuilder();
        command.WithName("add_meme");
        command.WithDescription("Add a daily meme");
        command.AddOption("image", ApplicationCommandOptionType.Attachment, "the meme", true);
        command.AddOption("message", ApplicationCommandOptionType.String, "the message", false);

        try
        {
            await _client.CreateGlobalApplicationCommandAsync(command.Build());
        }
        catch (HttpException e)
        {
            Logger.Error(e.ToString());
            var json = JsonSerializer.Serialize(e.Errors, new JsonSerializerOptions()
            {
                WriteIndented = true
            });
            Logger.Error(json);
        }

        _client.SlashCommandExecuted += SlashCommandHandler;
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        if (command.CommandName != "add_meme")
            return;
        
        if (command.User.Id != 259483144934260755)
        {
            Logger.Warning($"{command.User.Username} tried to add a meme but is not allowed to");
            await command.RespondAsync($"no can do", ephemeral: true);
            return;
        }
        
        var attachment = (IAttachment) command.Data.Options.First().Value;
        
        // download the file
        string file = Guid.NewGuid().ToString();
        Console.WriteLine(attachment.Url);
        
        //var uri = new Uri(attachment.Url);
        //var response = await _http.GetAsync(uri);
        //await using var fs = new FileStream(Path.Join(_config.MemeLocation, $"{day}.png"), FileMode.CreateNew);
        //await response.Content.CopyToAsync(fs);
//
        //// get a message if there is one
        //foreach (var option in command.Data.Options)
        //{
        //    if (option.Name == "message")
        //    {
        //        var message = option.Value;
        //        if (message == null)
        //            break;
        //        
        //        _config.MemeMessages.Add(day, (string) message);
        //        ConfigHelpers.SaveConfig(_config, "memes.json");
        //        break;
        //    }
        //}
//
        //await command.RespondAsync($"added meme for day {day}", ephemeral: true);
        //Console.WriteLine($"Added a meme for day {day}");
    }
}