using System.Text.Json;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LemonBot.Configurations;
using LemonBot.Utilities;

namespace LemonBot.Features.Memes;

public class MemeCommand
{
    private readonly DiscordSocketClient _client;
    private readonly MemeManager _memes;

    private HttpClient _http = new();
    
    public MemeCommand(DiscordSocketClient client, MemeManager memes)
    {
        _client = client;
        _memes = memes;
    }

    public async Task Register(ulong guildId)
    {
        var command = new SlashCommandBuilder();
        command.WithName("add_meme");
        command.WithDescription("Add a daily meme");
        command.AddOption("image", ApplicationCommandOptionType.Attachment, "the meme", true);
        command.AddOption("message", ApplicationCommandOptionType.String, "the message", false);
        command.AddOption("next_meme", ApplicationCommandOptionType.Boolean, "if the meme should be the next one to post", false);

        var guild = _client.GetGuild(guildId);
        if (guild == null)
        {
            Logger.Error("hey stinky the meme command server couldnt be found");
            return;
        }

        try
        {
            await guild.CreateApplicationCommandAsync(command.Build());
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

        DailyMemes.Meme meme = new();

        bool skipLine = false;

        foreach (var option in command.Data.Options)
        {
            switch (option.Name)
            {
                case "image":
                    var attachment = (IAttachment) option.Value;

                    var uri = new Uri(attachment.Url);
                    var response = await _http.GetAsync(uri);
                    var extension = GetExtension(response.Content.Headers.ContentType!.MediaType!);
                    if (extension == null)
                    {
                        await command.RespondAsync("Unsupported file format!", ephemeral: true);
                        return;
                    }

                    var path = _memes.GetNewMemeFileLocation(extension);
                    meme.File = path;

                    Directory.CreateDirectory(Directory.GetParent(path)!.FullName);

                    await using (var fs = new FileStream(path, FileMode.CreateNew))
                    {
                        await response.Content.CopyToAsync(fs);
                    };
                    break;

                case "message":
                    var message = option.Value;
                    if (message == null)
                        break;

                    meme.Message = (string)message;
                    break;

                case "next_meme":
                    var value = option.Value;
                    if (value == null)
                        break;

                    skipLine = (bool)option.Value;
                    break;
            }
        }

        _memes.AddMeme(meme, skipLine);
        
        await command.RespondAsync("added the meme to the " + (skipLine ? "start of" : "end of") + " the line", ephemeral: true);
        Console.WriteLine($"Added a meme");
    }

    private string? GetExtension(string contentType)
    {
        switch (contentType)
        {
            case "image/png": return "png";
            case "image/jpeg": return "jpeg";
            case "image/webp": return "webp";
            case "image/gif": return "gif";
            default: return null;
        }
    }
}