using System;
using System.Collections.Generic;
using System.Text;

namespace TGwDS
{
    class TelegramLogger
    {
        public static void ConsoleLog(string logMessage)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[TELEGRAM] {DateTime.Now} {logMessage}");
            Console.ForegroundColor = ConsoleColor.Gray;

        }
    }
}
