using System;

namespace TelegramBot_Observer.src
{
	class ConsoleHelper
	{
        private const ConsoleColor Header = ConsoleColor.Yellow;
        private const ConsoleColor Error = ConsoleColor.Red;
        private const ConsoleColor Success = ConsoleColor.Green;
        private const ConsoleColor CommandServer = ConsoleColor.Cyan;
        private static bool isShowLog = true;

        public static void WriteError(string message)
        {
            Console.ForegroundColor = Error;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteSuccess(string message)
        {
            if (!isShowLog) return;
            Console.ForegroundColor = Success;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteBotCommand(string message)
        {
            Console.ForegroundColor = CommandServer;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void WriteDefault(string message)
        {
            if (!isShowLog) return;
            Console.WriteLine(message);
        }

        public static void WriteSeparator()
        {
            Console.ForegroundColor = Header;
            Console.WriteLine("-----------------------------------------");
            Console.ResetColor();
        }

        public static void SetShowLog(bool isShow)
        {
            isShowLog = isShow;
        }
    }
}
