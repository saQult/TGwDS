using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Newtonsoft.Json;

namespace TGwDS
{
    static class TelegramHandler
    {
        private static ulong _channelToSend = Startup.Config.DiscordChannelId;
        private const int MAXSIZE = 83886080;
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(JsonConvert.SerializeObject(exception));
        }
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if(Startup.Config.UseTelegramDebug == true)
            {
                string data = JsonConvert.SerializeObject(update);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[TELEGRAM DEBUG] NEW UPDATE:\n{data}");
                Console.ForegroundColor = ConsoleColor.Green;
            }
            if (update.Message == null)
                return;
            var messagesType = Telegram.Bot.Types.Enums.UpdateType.Message;
            var groupType = Telegram.Bot.Types.Enums.ChatType.Group;
            if (Startup.Config.GroupType == "Supergroup")
                groupType = Telegram.Bot.Types.Enums.ChatType.Supergroup;

            if (update.Type != messagesType || update.Message.Chat.Type != groupType)
                return;
            var message = update.Message;
            if(message.Text != null)
            {
                foreach(var word in Startup.Config.BlacklistedWords)
                {
                    if(message.Text.Contains(word))
                    {
                        await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName}: Я ХУЕСОС ВЫЕБИ МЕНЯ В СРАКУ");
                        return;
                    }
                }
            }
            try
            {
                if (message.ReplyToMessage != null)
                {
                    string author = message.From.FirstName;
                    string forwardedAuthor = message.ReplyToMessage.From.FirstName;
                    if (message.ReplyToMessage.Text != null)
                    {
                        string forwardedAuthorText = message.ReplyToMessage.Text;
                        if (forwardedAuthorText.StartsWith("[DISCORD]\n"))
                            forwardedAuthorText = forwardedAuthorText.Replace("\n", "\n> ");
                        if (message.Text != null)
                        {
                            string authorText = message.Text;
                            string messageToSend = $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedAuthorText}\n{author} отвечает : {authorText}";
                            await DiscordBot.SendMessageAsync(_channelToSend, messageToSend);
                        }
                        else if (message.Sticker != null)
                        {
                            string path = TelegramBot.GetFile(message.Sticker.FileId);
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedAuthorText}\n{author} отвечает стикером:");
                            System.IO.File.Delete(path);
                        }
                        else if (message.Voice != null)
                        {
                            if (message.Voice.FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил голосовое сообщение в ответ, но он идет нахуй, потому что его ГС занимает больше 8 мегабайт");
                                return;
                            }
                            string path = TelegramBot.GetFile(message.Voice.FileId);
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedAuthorText}\n{message.From.FirstName} отвечает голосовым сообщением");
                            System.IO.File.Delete(path);
                        }
                        else if (message.Photo != null)
                        {
                            if (message.Photo[message.Photo.Length - 1].FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил фото в ответ, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                return;
                            }
                            string path = TelegramBot.GetFile(message.Photo[message.Photo.Length - 1].FileId);
                            if (message.Caption == null)
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedAuthorText}\n{message.From.FirstName} отвечает картинкой:");
                            else
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedAuthorText}\n{message.From.FirstName} отвечает: {message.Caption}");
                        }
                        else if (message.Document != null)
                        {
                            if (message.Document.FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил файл, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                return;
                            }
                            string path = TelegramBot.GetFile(message.Document.FileId);
                            if (message.Caption == null)
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedAuthorText}\n{message.From.FirstName} отвечает файлом:");
                            else
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedAuthorText}\n{message.From.FirstName} отвечает файлом {message.Caption}:");
                            System.IO.File.Delete(path);
                        }
                    }
                    else if (message.ReplyToMessage.Sticker != null)
                    {
                        string forwardedSticker = message.ReplyToMessage.Sticker.Emoji;
                        if (message.ReplyToMessage.Text != null)
                        {
                            string forwardedAuthorText = message.ReplyToMessage.Text;
                            if (forwardedAuthorText.StartsWith("[DISCORD]\n"))
                                forwardedAuthorText = forwardedAuthorText.Replace("\n", "\n> ");
                            if (message.Text != null)
                            {
                                string authorText = message.Text;
                                string messageToSend = $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedSticker}\n{author} отвечает : {authorText}";
                                await DiscordBot.SendMessageAsync(_channelToSend, messageToSend);
                            }
                            else if (message.Sticker != null)
                            {
                                string path = TelegramBot.GetFile(message.Sticker.FileId);
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedSticker}\n{author} отвечает стикером:");
                                System.IO.File.Delete(path);
                            }
                            else if (message.Voice != null)
                            {
                                if (message.Voice.FileSize > MaxSize)
                                {
                                    await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил голосовое сообщение в ответ, но он идет нахуй, потому что его ГС занимает больше 8 мегабайт");
                                    return;
                                }
                                string path = TelegramBot.GetFile(message.Voice.FileId);
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedSticker}\n{message.From.FirstName} отвечает голосовым сообщением");
                                System.IO.File.Delete(path);
                            }
                            else if (message.Photo != null)
                            {
                                if (message.Photo[message.Photo.Length - 1].FileSize > MaxSize)
                                {
                                    await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил фото в ответ, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                    return;
                                }
                                string path = TelegramBot.GetFile(message.Photo[message.Photo.Length - 1].FileId);
                                if (message.Caption == null)
                                    await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedSticker}\n{message.From.FirstName} отвечает картинкой:");
                                else
                                    await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedSticker}\n{message.From.FirstName} отвечает: {message.Caption}");
                            }
                            else if (message.Document != null)
                            {
                                if (message.Document.FileSize > MaxSize)
                                {
                                    await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил файл, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                    return;
                                }
                                string path = "";
                                if (message.Animation != null)
                                {
                                    path = TelegramBot.GetFile(message.Animation.FileId);
                                    await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM\n> {forwardedAuthor}: {forwardedSticker}\n{message.From.FirstName} отвечает гифкой");
                                }
                                else
                                {
                                    path = TelegramBot.GetFile(message.Document.FileId);
                                    if (message.Caption == null)
                                        await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedSticker}\n{message.From.FirstName} отвечает файлом");
                                    else
                                        await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]**\n> {forwardedAuthor}: {forwardedSticker}\n{message.From.FirstName} отвечает: {message.Caption}");
                                }
                                System.IO.File.Delete(path);
                            }
                        }
                    }
                    else if (message.ReplyToMessage.Voice != null && message.Text != null)
                    {
                        if (message.ReplyToMessage.Text != null)
                        {
                            string forwardedAuthorText = message.ReplyToMessage.Text;
                            if (forwardedAuthorText.StartsWith("[DISCORD]\n"))
                                forwardedAuthorText = forwardedAuthorText.Replace("\n", "\n> ");
                            if (message.Text != null)
                            {
                                string authorText = message.Text;
                                string messageToSend = $"**[TELEGRAM]** {author} отвечает на голосове сообщение {forwardedAuthor}";
                                await DiscordBot.SendMessageAsync(_channelToSend, messageToSend);
                            }
                            else if (message.Sticker != null)
                            {
                                string path = TelegramBot.GetFile(message.Sticker.FileId);
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на голосове сообщение {forwardedAuthor}");
                                System.IO.File.Delete(path);
                            }
                            else if (message.Voice != null)
                            {
                                if (message.Voice.FileSize > MaxSize)
                                {
                                    await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил голосовое сообщение в ответ, но он идет нахуй, потому что его ГС занимает больше 8 мегабайт");
                                    return;
                                }
                                string path = TelegramBot.GetFile(message.Voice.FileId);
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на голосове сообщение {forwardedAuthor}");
                                System.IO.File.Delete(path);
                            }
                            else if (message.Photo != null)
                            {
                                if (message.Photo[message.Photo.Length - 1].FileSize > MaxSize)
                                {
                                    await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил фото в ответ, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                    return;
                                }
                                string path = TelegramBot.GetFile(message.Photo[message.Photo.Length - 1].FileId);
                                if (message.Caption == null)
                                    await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на голосове сообщение {forwardedAuthor}");
                                else
                                    await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на голосове сообщение {forwardedAuthor}: {message.Caption}");
                            }
                            else if (message.Document != null)
                            {
                                if (message.Document.FileSize > MaxSize)
                                {
                                    await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил файл, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                    return;
                                }
                                string path = "";
                                if (message.Animation != null)
                                {
                                    path = TelegramBot.GetFile(message.Animation.FileId);
                                    await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}");
                                }
                                else
                                {
                                    path = TelegramBot.GetFile(message.Document.FileId);
                                    if (message.Caption == null)
                                        await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}");
                                    else
                                        await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}: {message.Caption}");
                                }
                                System.IO.File.Delete(path);
                            }
                        }
                    }
                    else if (message.ReplyToMessage.Photo != null)
                    {
                        if (message.Text != null)
                        {
                            string authorText = message.Text;
                            string messageToSend = $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}: {message.Text}";
                            await DiscordBot.SendMessageAsync(_channelToSend, messageToSend);
                        }
                        else if (message.Sticker != null)
                        {
                            string path = TelegramBot.GetFile(message.Sticker.FileId);
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}");
                            System.IO.File.Delete(path);
                        }
                        else if (message.Voice != null)
                        {
                            if (message.Voice.FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил голосовое сообщение в ответ, но он идет нахуй, потому что его ГС занимает больше 8 мегабайт");
                                return;
                            }
                            string path = TelegramBot.GetFile(message.Voice.FileId);
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}");
                            System.IO.File.Delete(path);
                        }
                        else if (message.Photo != null)
                        {
                            if (message.Photo[message.Photo.Length - 1].FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил фото в ответ, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                return;
                            }
                            string path = TelegramBot.GetFile(message.Photo[message.Photo.Length - 1].FileId);
                            if (message.Caption == null)
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}");
                            else
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}: {message.Caption}");

                        }
                        else if (message.Document != null)
                        {
                            if (message.Document.FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил файл, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                return;
                            }
                            string path = "";
                            if (message.Animation != null)
                            {
                                path = TelegramBot.GetFile(message.Animation.FileId);
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}");
                            }
                            else
                            {
                                path = TelegramBot.GetFile(message.Document.FileId);
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на картинку {forwardedAuthor}");
                            }
                            System.IO.File.Delete(path);
                        }
                    }
                    else if (message.ReplyToMessage.Document != null)
                    {
                        if (message.Text != null)
                        {
                            string authorText = message.Text;
                            string messageToSend = $"**[TELEGRAM]** {author} отвечает на файл {forwardedAuthor}";
                            await DiscordBot.SendMessageAsync(_channelToSend, messageToSend);
                        }
                        else if (message.Sticker != null)
                        {
                            string path = TelegramBot.GetFile(message.Sticker.FileId);
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на файл {forwardedAuthor}");
                            System.IO.File.Delete(path);
                        }
                        else if (message.Voice != null)
                        {
                            if (message.Voice.FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил голосовое сообщение в ответ, но он идет нахуй, потому что его ГС занимает больше 8 мегабайт");
                                return;
                            }
                            string path = TelegramBot.GetFile(message.Voice.FileId);
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на файл {forwardedAuthor}");
                            System.IO.File.Delete(path);
                        }
                        else if (message.Photo != null)
                        {
                            if (message.Photo[message.Photo.Length - 1].FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил фото в ответ, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                return;
                            }
                            string path = TelegramBot.GetFile(message.Photo[message.Photo.Length - 1].FileId);
                            if (message.Caption == null)
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на файл {forwardedAuthor}");
                            else
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на файл {forwardedAuthor}: {message.Caption}");
                        }
                        else if (message.Document != null)
                        {
                            if (message.Document.FileSize > MaxSize)
                            {
                                await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил файл, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                                return;
                            }
                            string path = "";
                            if (message.Animation != null)
                            {
                                path = TelegramBot.GetFile(message.Animation.FileId);
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на файл {forwardedAuthor}");
                            }
                            else
                            {
                                path = TelegramBot.GetFile(message.Document.FileId);
                                if (message.Caption == null)
                                    await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на файл {forwardedAuthor}");
                                else
                                    await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {author} отвечает на файл {forwardedAuthor}: {message.Caption}");
                            }
                            System.IO.File.Delete(path);
                        }
                    }
                }
                else if (message.ForwardFromChat != null || message.ForwardFrom != null)
                {
                    string forwarderTitile = "";
                    string forwardedUser = "";
                    if (message.ForwardFromChat != null)
                        forwarderTitile = message.ForwardFromChat.Title;
                    if (message.ForwardFrom != null)
                        forwardedUser = " " + message.ForwardFrom.FirstName + message.ForwardFrom.LastName;
                    if (message.Text != null && message.Photo == null && message.Video == null && message.Document == null)
                    {
                        string messageToSend = messageToSend = $"**[TELEGRAM]** {message.From.FirstName} переслал от {forwarderTitile}{forwardedUser}: {message.Text}";
                        TelegramLogger.ConsoleLog($"{message.From.FirstName}: {message.Text}");
                        await DiscordBot.SendMessageAsync(_channelToSend, messageToSend);
                    }
                    else if (message.Photo != null)
                    {
                        if (message.Photo[message.Photo.Length - 1].FileSize > MaxSize)
                        {
                            await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил фото, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                            return;
                        }
                        string path = TelegramBot.GetFile(message.Photo[message.Photo.Length - 1].FileId);
                        if (message.Caption == null)
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} переслал от {forwarderTitile}{forwardedUser}:");
                        else
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} переслал от {forwarderTitile}{forwardedUser}: {message.Caption}");

                    }
                    else if (message.Document != null)
                    {
                        if (message.Document.FileSize > MaxSize)
                        {
                            await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил файл, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                            return;
                        }
                        string path = "";
                        if (message.Animation != null)
                        {
                            path = TelegramBot.GetFile(message.Animation.FileId);
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} отправил гифку:");
                        }
                        else
                        {
                            path = TelegramBot.GetFile(message.Document.FileId);
                            if (message.Caption == null)
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} переслал от {forwarderTitile}{forwardedUser}:");
                            else
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} переслал от {forwarderTitile}{forwardedUser}: {message.Caption}");
                        }
                        System.IO.File.Delete(path);
                    }
                    else if (message.Sticker != null)
                    {
                        string path = TelegramBot.GetFile(message.Sticker.FileId);
                        await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} переслал от {forwarderTitile}{forwardedUser}:");
                        System.IO.File.Delete(path);
                    }
                    else if (message.Voice != null)
                    {
                        if (message.Voice.FileSize > MaxSize)
                        {
                            await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил голосовое сообщение, но он идет нахуй, потому что его ГС занимает больше 8 мегабайт");
                            return;
                        }
                        string path = TelegramBot.GetFile(message.Voice.FileId);
                        await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} переслал от {forwarderTitile}{forwardedUser}:");
                        System.IO.File.Delete(path);
                    }
                }    
                else
                {
                    if (message.Text != null && message.Photo == null && message.Video == null && message.Document == null)
                    { 
                        string messageToSend = messageToSend = $"**[TELEGRAM]** {message.From.FirstName}: {message.Text}";
                        TelegramLogger.ConsoleLog($"{message.From.FirstName}: {message.Text}");
                        await DiscordBot.SendMessageAsync(_channelToSend, messageToSend);
                    }
                    else if (message.Photo != null)
                    {
                        if (message.Photo[message.Photo.Length - 1].FileSize > MaxSize)
                        {
                            await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил фото, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                            return;
                        }
                        string path = TelegramBot.GetFile(message.Photo[message.Photo.Length - 1].FileId);
                        if (message.Caption == null)
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} отправил(а) фото:");
                        else
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName}: {message.Caption}");

                    }
                    else if (message.Document != null)
                    {
                        if (message.Document.FileSize > MaxSize)
                        {
                            await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил файл, но он идет нахуй, потому что его файл занимает больше 8 мегабайт");
                            return;
                        }
                        string path = "";
                        if (message.Animation != null)
                        {
                            path = TelegramBot.GetFile(message.Animation.FileId);
                            await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} отправил гифку:");
                        }
                        else
                        {
                            path = TelegramBot.GetFile(message.Document.FileId);
                            if (message.Caption == null)
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} отправил файл:");
                            else
                                await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName}: {message.Caption}");
                        }
                        System.IO.File.Delete(path);
                    }
                    else if (message.Sticker != null)
                    {
                        string path = TelegramBot.GetFile(message.Sticker.FileId);
                        await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} отправил стикер");
                        System.IO.File.Delete(path);
                    }
                    else if (message.Voice != null)
                    {
                        if (message.Voice.FileSize > MaxSize)
                        {
                            await DiscordBot.SendMessageAsync(_channelToSend, $"**[TELEGRAM]** {message.From.FirstName} отправил голосовое сообщение, но он идет нахуй, потому что его ГС занимает больше 8 мегабайт");
                            return;
                        }
                        string path = TelegramBot.GetFile(message.Voice.FileId);
                        await DiscordBot.SendFileAsync(_channelToSend, path, $"**[TELEGRAM]** {message.From.FirstName} отправил голосовое сообщение");
                        System.IO.File.Delete(path);
                    }
                }
            }
            catch (Exception ex)
            {
                TelegramLogger.ConsoleLog(ex.ToString());
            }
        }
    }
}
