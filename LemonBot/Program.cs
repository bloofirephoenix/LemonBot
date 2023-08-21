using Discord;
using Discord.WebSocket;
using LemonBot;
using LemonBot.Configurations;
using LemonBot.Features;
using LemonBot.Utilities;

_ = new Logger();

Console.WriteLine("LemonBot");



if (!ConfigHelpers.InitializeConfig("config.json", out Config config))
{
    Console.WriteLine("Please setup config.json");
    return;
}

var client = new DiscordSocketClient();
client.Log += Logger.DiscordLog;

await client.LoginAsync(TokenType.Bot, config.Discord);
await client.StartAsync();

new DailyPetRocketRacingMemes(client).Start();

TerminalManager.Start();
Console.WriteLine("Exiting");
await client.StopAsync();