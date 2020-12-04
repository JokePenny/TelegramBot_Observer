using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Telegram.Bot;

namespace TelegramBot_Observer.src
{
	class Bot
	{
        protected static TelegramBotClient botClient;
        private Thread inputThread;
        // прослушивание входящих подключений
        protected internal async void Listen()
        {
            try
            {
                ConsoleHelper.WriteSuccess("Бот запущен.\n");
                string key = ReadDataPasswordsBot();
                botClient = new TelegramBotClient(key);
                inputThread = new Thread(new ThreadStart(InputCommandBotThread));
                inputThread.Start();
                await botClient.SetWebhookAsync("");
                int offset = 0;
                while (true)
                {
                    var updates = await botClient.GetUpdatesAsync(offset);

                    foreach (var update in updates)
                    {
                        var message = update.Message;
                        if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                        {
                            CommandBotParser.ApplyCommand(message);
                        }
                        offset = update.Id + 1;
                    }

                }
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void InputCommandBotThread()
        {
            while (true)
            {
                string command = Console.ReadLine();
                if (command == "exit" || command == "quit" || command == "q") break;
                ConsoleHelper.WriteBotCommand(CommandConsoleBotParser.ApplyCommand(command));
            }
        }

        protected internal void Disconnect()
        {
            ConsoleHelper.WriteError("Бот завершил работу");
            inputThread.Join();
            Environment.Exit(0);
        }

        private string ReadDataPasswordsBot()
        {
            //Test on Windows 
            //FileStream fstream = File.OpenRead(Environment.CurrentDirectory + "\\password_bot.txt");
            //For server on Ubuntu
            FileStream fstream = File.OpenRead("/home/alex/Desktop/secret_keys/password_bot.txt"); 
            //Test on Windows 
            //int lengthFstream = array.Length;
            //For server on Ubuntu
            long lengthFstream = fstream.Length - 1;
            byte[] array = new byte[lengthFstream];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }
    }
}
