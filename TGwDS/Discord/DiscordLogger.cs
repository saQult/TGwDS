using System;


namespace TGwDS
{
    static class DiscordLogger
    {
        public static void ConsoleLog(string logMessage, bool writeDate=true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if(writeDate)
                Console.WriteLine($"[DISCORD] {DateTime.Now} {logMessage}");
            else
                Console.WriteLine($"[DISCORD] {logMessage}");

            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
