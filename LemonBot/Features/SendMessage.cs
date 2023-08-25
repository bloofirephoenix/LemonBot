using System.Text.Json;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LemonBot.Utilities;

namespace LemonBot.Features;

public class SendMessage
{
    private DiscordSocketClient _client;
    
    public SendMessage(DiscordSocketClient client)
    {
        _client = client;
    }
    
    public async Task Start()
    {
        Console.WriteLine("Starting send message");
        var command = new SlashCommandBuilder();
        command.WithName("send_msg");
        command.WithDescription("send message");
        command.AddOption("msg", ApplicationCommandOptionType.String, "the message to send", true);

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
        if (command.CommandName != "send_msg")
            return;

        if (command.User.Id != 259483144934260755)
        {
            await command.RespondAsync("no", ephemeral: true);
            return;
        }

        await command.Channel.SendMessageAsync((string) command.Data.Options.First().Value);
        await command.RespondAsync("done", ephemeral: true);
    }
}