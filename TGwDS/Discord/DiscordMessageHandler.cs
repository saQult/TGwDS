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
            if (message.Author.IsBot || message.Channel.Id != Startup.Config.DiscordChannelId)
                return;
            if(String.IsNullOrEmpty(message.Content) == false)
            {
                DiscordLogger.ConsoleLog($"{message.Author.Username}: {message.Content}");
                string messageToSend = $"[DISCORD]\n{message.Author.Username}: {message.Content}";
                await TelegramBot.Client.SendTextMessageAsync(Startup.Config.TelegramChatId, messageToSend);
            }
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

                            DiscordLogger.ConsoleLog($"{message.Author.Username}: отправил файл {att.Filename}");
                            if (att.Filename.Contains(".mp4") || att.Filename.Contains("webm") || att.Filename.Contains(".avi"))
                            {
                                await TelegramBot.Client.SendVideoAsync(Startup.Config.TelegramChatId, iof, null, att.Width, att.Height, null, att.Description);
                            }
                            else if (att.Filename.Contains(".png") || att.Filename.Contains(".jpg") || att.Filename.Contains(".jpeg"))
                            {
                                await TelegramBot.Client.SendPhotoAsync(Startup.Config.TelegramChatId, iof, att.Description);
                            }
                            else if (att.Filename.Contains(".mp3") || att.Filename.Contains(".wav") || att.Filename.Contains(".oga"))
                            {
                                await TelegramBot.Client.SendVoiceAsync(Startup.Config.TelegramChatId, iof);
                            }
                            else
                            {
                                await TelegramBot.Client.SendDocumentAsync(Startup.Config.TelegramChatId, iof);
                            }
                        }
                        File.Delete($"{Environment.CurrentDirectory}\\cache\\{att.Filename}");
                    }
                }
            }
            if (message.Stickers != null)
            {
                var sticker = message.Stickers.FirstOrDefault();
                if (sticker == null)
                    return;
                DiscordLogger.ConsoleLog($"{message.Author.Username}: отправил стикер {sticker.Name}");
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFile($"https://media.discordapp.net/stickers/{sticker.Id}.webp", $"{Environment.CurrentDirectory}\\cache\\{sticker.Name}.webp");
                    using (FileStream fs = new FileStream($"{Environment.CurrentDirectory}\\cache\\{sticker.Name}.webp", FileMode.Open))
                    {
                        InputOnlineFile iof = new InputOnlineFile(fs);
                        iof.FileName = sticker.Name + ".webp";
                        string messageToSend = $"[DISCORD]\n{message.Author.Username} отправил стикер:";
                        await TelegramBot.Client.SendTextMessageAsync(Startup.Config.TelegramChatId, messageToSend);
                        await TelegramBot.Client.SendStickerAsync(Startup.Config.TelegramChatId, iof);
                    }
                    File.Delete($"{Environment.CurrentDirectory}\\cache\\{sticker.Name}.webp");
                }
            }   
        }
    }
}
