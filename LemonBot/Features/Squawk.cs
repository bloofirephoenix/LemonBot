using System;
using Discord;
using Discord.WebSocket;
using LemonBot.Utilities;

namespace LemonBot.Features;

public class Squawk(DiscordSocketClient client)
{
    public void Start()
    {
        Console.WriteLine("enabling squawk.");
        Console.WriteLine("warming up the vocal chords as we speak");
        client.MessageReceived += OnMessageReceived;
        TaskManager.RunTask(Run);
    }

    private async Task OnMessageReceived(SocketMessage message)
    {
        if (message.Author.Id == client.CurrentUser.Id)
            return;
        bool mentioned = false;
        foreach (var user in message.MentionedUsers)
        {
            if (user.Id == client.CurrentUser.Id)
            {
                mentioned = true;
                break;
            }
        }
        
        if (!mentioned)
        {
            foreach (var role in message.MentionedRoles)
            {
                if (role.Guild.CurrentUser.Roles.Contains(role))
                {
                    mentioned = true;
                    break;
                }
            }
        }

        mentioned = mentioned || message.MentionedEveryone;

        var reference = new MessageReference(message.Id);
        if (mentioned)
        {
            Console.WriteLine("Squawking because I was mentioned >:(");
            var sayFuckYou = Random.Shared.NextDouble() < Config.Instance!.Squawking.FuckYouResponseChance;
            await SendSquawk(message.Channel, reference, sayFuckYou);
        }
        else if (Config.Instance!.Squawking.RandomSquawkChannels.Contains(message.Channel.Id) && Random.Shared.NextDouble() < Config.Instance.Squawking.RandomResponseChance)
        {
            Console.WriteLine("squawking because someone said something");
            var sayFuckYou = Random.Shared.NextDouble() < Config.Instance.Squawking.FuckYouResponseChance;
            
            await SendSquawk(message.Channel, reference, sayFuckYou);
        }
    }

    private async Task SendSquawk(ISocketMessageChannel channel, MessageReference? reference = null, bool fuckYou = false)
    {
        var message = fuckYou ? "fuk u" : "Squawk!";

        if (channel is ITextChannel)
        {
            var guidChannel = (ITextChannel) channel;
            var user = await guidChannel.Guild.GetCurrentUserAsync();
            if (!user.GetPermissions(guidChannel).SendMessages)
            {
                Console.WriteLine("aborting squawk because no permissions :(");
                return;
            }
        }

        await channel.SendMessageAsync(message, messageReference: reference);
    }

    private async void Run()
    {
        Console.WriteLine("enabling random squawk");
        while (true)
        {
            if ((Config.Instance!.Squawking.MinSquawkTime <= 0 && Config.Instance!.Squawking.MaxSquawkTime <= 0) || Config.Instance!.Squawking.MaxSquawkTime < Config.Instance!.Squawking.MinSquawkTime)
            {
                Logger.Warning("random squawk times are invalid!");
                Console.WriteLine("disabling random squawk");
                return;
            }

            var waitTime = Random.Shared.Next(Config.Instance!.Squawking.MinSquawkTime, Config.Instance!.Squawking.MaxSquawkTime);
            Console.WriteLine($"squawking in {waitTime/1000} seconds");
            await Task.Delay(waitTime);

            if (Config.Instance.Squawking.RandomSquawkChannels.Count <= 0) // man i really hope its not less than 0 '>'
            {
                Logger.Warning("no random squawk channels specified");
                Console.WriteLine("disabling random squawk");
                return;
            }

            Console.WriteLine("sending random squawk");
            
            var channelId = Config.Instance.Squawking.RandomSquawkChannels[Random.Shared.Next(0, Config.Instance.Squawking.RandomSquawkChannels.Count)];
            var channel = (ISocketMessageChannel) await client.GetChannelAsync(channelId);
            await SendSquawk(channel);
        }
    }
}
