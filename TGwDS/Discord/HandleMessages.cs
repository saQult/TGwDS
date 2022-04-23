using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;

namespace TGwDS
{
    static class DiscordMessageHandler
    {
        public static async Task SendMessageToTelegram(SocketMessage message)
        {
            if (message.Author.IsBot || String.IsNullOrEmpty(message.Content) || message.Channel.Id != 965599901201363004)
                return;
            DiscordLogger.ConsoleLog($"{message.Author.Username}: {message.Content}");
            string messageToSend = $"[DISCORD]\n{message.Author.Username}: {message.Content}";
            await TelegramBot.Client.SendTextMessageAsync(-1001377314351, messageToSend);
        }
    }
}