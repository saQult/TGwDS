using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Telegram.Bot;

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
            await TelegramBot.Client.SendTextMessageAsync(Startup.Config.TelegramChatId, messageToSend);
        }
    }
}
