using Discord;
using Discord.WebSocket;
using LemonBot;
using LemonBot.Configurations;
using LemonBot.Features;
using LemonBot.Utilities;

_ = new Logger();

Console.WriteLine("LemonBot");

Config? config = ConfigHelpers.InitializeConfig<Config>("config.json");

if (config == null)
{
    Console.WriteLine("Please setup config.json");
    return;
}

var client = new DiscordSocketClient();
client.Log += Logger.DiscordLog;

await client.LoginAsync(TokenType.Bot, config.Discord);
await client.StartAsync();

_ = new DailyPetRocketRacingMemes(client);

TerminalManager.Start();
Console.WriteLine("Exiting");
await client.StopAsync();