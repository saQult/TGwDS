using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TGwDS
{
    static class Startup
    {
        public static Config Config;
        public static Task Main(string[] args) => MainAsync();

        public static async Task MainAsync()
        {
            Config = JsonConvert.DeserializeObject<Config>(System.IO.File.ReadAllText("config.json"));
            await DiscordBot.RunAsync();
            TelegramBot.Run();
            await Task.Delay(-1);
        }
    }
}



