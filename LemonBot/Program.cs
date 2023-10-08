using Discord;
using Discord.WebSocket;
using LemonBot;
using LemonBot.Configurations;
using LemonBot.Features;
using LemonBot.Features.Memes;
using LemonBot.Utilities;

_ = new Logger();

Console.WriteLine("LemonBot");

if (!ConfigHelpers.InitializeConfig("config.json", out Config config))
{
    Console.WriteLine("Please setup config.json");
    return;
}

var client = new DiscordSocketClient(new DiscordSocketConfig()
{
    GatewayIntents = GatewayIntents.GuildMessages
});

client.Log += Logger.DiscordLog;

await client.LoginAsync(TokenType.Bot, config.Discord);
await client.StartAsync();

client.Ready += async Task () =>
{
    new MemeManager(client).Start();
    await new SendMessage(client).Start();
};

TerminalManager.Start();
Console.WriteLine("Exiting");
await client.StopAsync();