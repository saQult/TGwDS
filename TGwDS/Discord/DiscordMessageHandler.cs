using Discord.WebSocket;
using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;
using System.Linq;

namespace TGwDS
{
    static class DiscordMessageHandler
    {
        public static async Task SendMessageToTelegram(SocketMessage message)
        {
            /*
             * Sender's logic:
             * 1st step: check for attachments and send it
             * 2nd step: if there are no attachments check stickers
             * 3rd step: if there are no attachments and stickers than send normal message
             */
            if (message.Author.IsBot || message.Channel.Id != Startup.Config.DiscordChannelId)
                return;

            string content = message.Content;
            foreach (var user in message.MentionedUsers)
                content = content.Replace($"<@{user.Id}>", $"@{user.Username}");
            foreach (var channel in message.MentionedChannels)
                content = content.Replace($"<#{channel.Id}>", $"#{channel.Name}");
            foreach (var role in message.MentionedRoles)
                content = content.Replace($"<@&{role.Id}>", $"@&{role.Name}");

            if (message.Attachments != null)
            {
                var attachments = message.Attachments;
                using (WebClient wb = new WebClient())
                {
                    foreach (var att in attachments)
                    {
                        wb.DownloadFile(att.Url, $"{Environment.CurrentDirectory}\\cache\\{att.Filename}");
                        using (FileStream fs = new FileStream($"{Environment.CurrentDirectory}\\cache\\{att.Filename}", FileMode.Open))
                        {
                            InputOnlineFile iof = new InputOnlineFile(fs);
                            iof.FileName = att.Filename;

                            DiscordLogger.ConsoleLog($"{message.Author.Username} отправил файл {att.Filename}: {content}");
                            if (att.Filename.Contains(".mp4") || att.Filename.Contains("webm") || att.Filename.Contains(".avi"))
                            {
                                await TelegramBot.Client.SendVideoAsync(Startup.Config.TelegramChatId, iof, null, att.Width, att.Height, null, $"[DISCORD]\n{message.Author.Username} отправил видео: {content}");
                            }
                            else if (att.Filename.Contains(".png") || att.Filename.Contains(".jpg") || att.Filename.Contains(".jpeg"))
                            {
                                await TelegramBot.Client.SendPhotoAsync(Startup.Config.TelegramChatId, iof, $"[DISCORD]\n{message.Author.Username} отправил картинку: {content}");
                            }
                            else if (att.Filename.Contains(".mp3") || att.Filename.Contains(".wav") || att.Filename.Contains(".oga"))
                            {
                                await TelegramBot.Client.SendVoiceAsync(Startup.Config.TelegramChatId, iof, $"[DISCORD]\n{message.Author.Username} отправил аудио: {content}");
                            }
                            else
                            {
                                await TelegramBot.Client.SendDocumentAsync(Startup.Config.TelegramChatId, iof, null, $"[DISCORD]\n{message.Author.Username} отправил файл: {content}");
                            }
                        }
                        File.Delete($"{Environment.CurrentDirectory}\\cache\\{att.Filename}");
                        return;
                    }
                }
            }
            else if (message.Stickers != null)
            {
                var sticker = message.Stickers.FirstOrDefault();
                if (sticker == null)
                    return;
                DiscordLogger.ConsoleLog($"{message.Author.Username}: отправил стикер {sticker.Name}");
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile($"https://media.discordapp.net/stickers/{sticker.Id}.webp?size=300x300", $"{Environment.CurrentDirectory}\\cache\\{sticker.Name}.webp");
                    using (FileStream fs = new FileStream($"{Environment.CurrentDirectory}\\cache\\{sticker.Name}.webp", FileMode.Open))
                    {
                        InputOnlineFile iof = new InputOnlineFile(fs);
                        iof.FileName = sticker.Name + ".webp";
                        await TelegramBot.Client.SendTextMessageAsync(Startup.Config.TelegramChatId, $"[DISCORD]\n{message.Author.Username} отправил стикер:");
                        await TelegramBot.Client.SendStickerAsync(Startup.Config.TelegramChatId, iof);
                    }
                    File.Delete($"{Environment.CurrentDirectory}\\cache\\{sticker.Name}.webp");
                }
                return;
            }
            else
            {
                DiscordLogger.ConsoleLog($"{message.Author.Username}: {content}");
                string messageToSend = $"[DISCORD]\n{message.Author.Username}: {content}";
                await TelegramBot.Client.SendTextMessageAsync(Startup.Config.TelegramChatId, messageToSend);
            }
        }
    }
}
