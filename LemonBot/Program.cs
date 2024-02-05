using Discord;
using Discord.WebSocket;
using LemonBot;
using LemonBot.Configurations;
using LemonBot.Features;
using LemonBot.Features.Memes;
using LemonBot.Utilities;

_ = new Logger();

Console.WriteLine("LemonBot");

Config config = new();
ConfigFile.Load(config);

if (!ConfigFile.Load(config))
{
    Console.WriteLine("Please setup config.json");
    return;
}

var client = new DiscordSocketClient(new DiscordSocketConfig()
{
    GatewayIntents = GatewayIntents.GuildMessages | GatewayIntents.Guilds
});

client.Log += Logger.DiscordLog;

await client.LoginAsync(TokenType.Bot, config.Discord);
await client.StartAsync();

var memeManager = new MemeManager(client);
var sendMessage = new SendMessage(client);

client.Ready += async Task () =>
{
    Console.WriteLine("Bot Ready");
    memeManager.Start();
    await sendMessage.Start();
};

TerminalManager.Start();
Console.WriteLine("Exiting");
await client.StopAsync();