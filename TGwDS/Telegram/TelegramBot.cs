using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace TGwDS
{
    static class TelegramBot
    {
        private static string _token = Startup.Config.TelegramToken;
        public static TelegramBotClient Client = new TelegramBotClient(_token);
        public static void Run()
        {
            TelegramLogger.ConsoleLog("Запущен бот " + Client.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { },
            };
            Client.StartReceiving(
                TelegramHandler.HandleUpdateAsync,
                TelegramHandler.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
        }

        public static string GetFile(string fileId)
        {
            string endpoint = $"https://api.telegram.org/bot{_token}";
            string fileEndpoint = $"https://api.telegram.org/file/bot{_token}";
            string sourceFile = "";

            using (var webClient = new WebClient())
            {
                try
                {
                    string req = webClient.DownloadString($"{endpoint}/getFile?file_id={fileId}");
                    TelegramLogger.ConsoleLog(req);
                    var data = JObject.Parse(req);
                    string filePath = data["result"]["file_path"].ToString();
                    /*
                     * Некоторые адекватные люди спросят: "Нахуй ты сюда впихнул .Remove() ?
                     * Меме в том, что имя файла - это формат directory/filename.
                     * Чтобы не создавать под каждый тип файла папочку, проще обрезать
                     */
                    sourceFile = $"{Environment.CurrentDirectory}\\cache\\{filePath.Remove(0,11)}";
                    webClient.DownloadFile($"{fileEndpoint}/{filePath}", sourceFile);
                }
                catch (Exception ex) { TelegramLogger.ConsoleLog(ex.ToString()); }
            }
            System.IO.FileInfo fi = new System.IO.FileInfo(sourceFile);
            //Чтобы голосовые сообщения нормально работали
            fi.MoveTo(sourceFile.Replace(".oga", ".mp3"));
            return sourceFile.Replace(".oga", ".mp3");
        }
    }
}