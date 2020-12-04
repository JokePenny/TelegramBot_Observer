using System;
using System.Threading;
using TelegramBot_Observer.src;

namespace TelegramBot_Observer
{
	class TelegramBot
	{
        static Bot bot; // сервер
        static Thread listenThread; // потока для прослушивания
        static void Main(string[] args)
		{
            try
            {
                bot = new Bot();
                bot.Listen();
                string command = Console.ReadLine();
            }
            catch (Exception ex)
            {
                bot.Disconnect();
                Console.WriteLine(ex.Message);
            }
        }
	}
}
