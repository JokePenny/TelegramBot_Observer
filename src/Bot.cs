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
        // прослушивание входящих подключений
        protected internal async void Listen()
        {
            try
            {
                ConsoleHelper.WriteSuccess("Бот запущен. Ожидание подключений...\n");
                string key = ReadDataPasswordsBot();
                botClient = new TelegramBotClient(key);
                await botClient.SetWebhookAsync("");
                Thread inputThread = new Thread(new ThreadStart(InputCommandBotThread));
                inputThread.Start();
                int offset = 0; // отступ по сообщениям
                while (true)
                {
                    var updates = await botClient.GetUpdatesAsync(offset); // получаем массив обновлений

                    foreach (var update in updates) // Перебираем все обновления
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
                ConsoleHelper.WriteBotCommand(CommandConsoleBotParser.ApplyCommand(command));
            }
        }

        protected internal void Disconnect()
        {
            ConsoleHelper.WriteError("Бот завершил работу");
            Environment.Exit(0); //завершение процесса
        }

        private string ReadDataPasswordsBot()
        {
            //Test on Windows 
            //FileStream fstream = File.OpenRead(Environment.CurrentDirectory + "\\password_bot.txt");
            //For server
            FileStream fstream = File.OpenRead("/home/lorne/secret_keys/password_bot.txt"); 
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }
    }
}
