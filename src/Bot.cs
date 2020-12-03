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
        // прослушивание входящих подключений
        protected internal async void Listen()
        {
            try
            {
                ConsoleHelper.WriteSuccess("Бот запущен. Ожидание подключений...\n");
                string key = ReadDataPasswordsBot();
                TelegramBotClient botClient = new TelegramBotClient(key);
                Thread inputThread = new Thread(new ThreadStart(InputCommandBotThread));
                inputThread.Start();
                ConsoleHelper.WriteSuccess(key);
                int offset = 0; // отступ по сообщениям
                while (true)
                {
                    var updates = await botClient.GetUpdatesAsync(offset); // получаем массив обновлений

                    foreach (var update in updates) // Перебираем все обновления
                    {
                        var message = update.Message;
                        if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                        {
                            if (message.Text == "/saysomething")
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, "тест",
                                       replyToMessageId: message.MessageId);
                            }
                        }
                        offset = update.Id + 1;
                    }

                }
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message); // если ключ не подошел - пишем об этом в консоль отладки
            }
        }

        private void InputCommandBotThread()
        {
            while (true)
            {
                string command = Console.ReadLine();
                ConsoleHelper.WriteBotCommand(CommandBotParser.ApplyCommand(command));
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
            FileStream fstream = File.OpenRead("~/secret_keys/password_bot.txt"); 
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }
    }
}
