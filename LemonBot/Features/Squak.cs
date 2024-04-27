using Discord;
using Discord.WebSocket;
using LemonBot.Configurations;
using LemonBot.Utilities;

namespace LemonBot.Features;

public class Squak
{
    DiscordSocketClient _client;
    SquakConfig _config = new();


    public Squak(DiscordSocketClient client)
    {
        _client = client;
    }

    public void Start()
    {
        ConfigFile.Load(_config);

        _client.MessageReceived += OnMessageRecieved;

        if (_config.RandomSquakChannels.Length == 0)
            return;

        Task.Run(async () =>
        {
            while (true)
            {
                int delay = Random.Shared.Next(_config.MinSquakTime, _config.MaxSquakTime);
                Console.WriteLine($"Squaking in {TimeSpan.FromMilliseconds(delay)}");
                
                await Task.Delay(delay);
                
                int i = Random.Shared.Next(0, _config.RandomSquakChannels.Length);
                var channel = _config.RandomSquakChannels[i];

                Console.WriteLine("Squaking");
                await SendSquak((ISocketMessageChannel)await _client.GetChannelAsync(channel), false);
            }
        });
    }

    private async Task OnMessageRecieved(SocketMessage message)
    {
        if (message.Author.Id == _client.CurrentUser.Id)
            return;
        bool mentioned = false;
        foreach (var user in message.MentionedUsers)
        {
            if (mentioned)
                break;
            if (user.Id == _client.CurrentUser.Id)
                mentioned = true;
        }

        if (_config.RandomSquakChannels.Contains(message.Channel.Id))
        {
            foreach (var role in message.MentionedRoles)
            {
                if (mentioned)
                    break;
                if (role.Guild.CurrentUser.Roles.Contains(role))
                    mentioned = true;
            }

            mentioned = mentioned || message.MentionedEveryone;
        }

        var reference = new MessageReference(message.Id);

        if (mentioned)
        {
            Console.WriteLine("Squaking because @");
            await SendSquak(message.Channel, true, reference);
        }
        else if (_config.RandomSquakChannels.Contains(message.Channel.Id) && Random.Shared.NextDouble() <= _config.FuckYouResponseChance)
        {
            Console.WriteLine("Telling someone to fuck off for no reason");
            await SendSquak(message.Channel, true, reference, true);
        }
    }

    private async Task SendSquak(ISocketMessageChannel channel, bool canSayFuckYou, MessageReference? reference = null, bool shouldSayFuckYou = false)
    {
        var message = "Squak!";
        if (channel is ITextChannel)
        {
            ITextChannel guidChannel = (ITextChannel) channel;
            var user = await guidChannel.Guild.GetCurrentUserAsync();
            if (!user.GetPermissions(guidChannel).SendMessages)
            {
                Console.WriteLine("Cannot squak because no permission");
                return;
            }
        }
        if (canSayFuckYou && (Random.Shared.NextDouble() <= _config.FuckYouResponseChance || shouldSayFuckYou))
        {
            message = "fuk u";
        }
        await channel.SendMessageAsync(message, messageReference: reference);
    }
}
