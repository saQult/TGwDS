using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TGwDS
{

    static class DiscordBot
    {
        private static string _token = Startup.Config.DiscordToken;
        public static DiscordSocketClient Client = new DiscordSocketClient();

        public static async Task RunAsync()
        {
            await Client.LoginAsync(TokenType.Bot, _token);
            await Client.StartAsync();
            Client.Log += (LogMessage msg) =>
            {
                DiscordLogger.ConsoleLog(msg.ToString(), false);
                return Task.CompletedTask;
            };
            Client.MessageReceived += DiscordMessageHandler.SendMessageToTelegram;
        }
        public static async Task SendMessageAsync(ulong chatID, string message)
        {
            try
            {
                var channel = Client.GetChannel(chatID) as IMessageChannel;
                if (channel == null)
                    return;
                await channel.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                DiscordLogger.ConsoleLog(ex.ToString());
            }
        }
        public static async Task SendFileAsync(ulong chatID, string path, string text = null)
        {
            try
            {
                var channel = Client.GetChannel(chatID) as IMessageChannel;
                if (channel == null)
                    return;
                await channel.SendFileAsync(path, text);
            }
            catch(Exception ex)
            {
                DiscordLogger.ConsoleLog(ex.ToString());
            }
        }
        public static async Task SendFilesAsync(ulong chatID, List<FileAttachment> filesToSend, string text = null)
        {
            try
            {
                var channel = Client.GetChannel(chatID) as IMessageChannel;
                if (channel == null)
                    return;
                await channel.SendFilesAsync(filesToSend, text);
            }
            catch (Exception ex)
            {
                DiscordLogger.ConsoleLog(ex.ToString());
            }
        }
    }
}

