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
        protected static List<string> listNameProccessObserve = new List<string>();
        protected static TelegramBotClient botClient;
        private Thread inputThread;
        // прослушивание входящих подключений
        protected internal async void Listen()
        {
            try
            {
                ConsoleHelper.WriteSuccess("Бот запущен.\n");
                //ReadProcessObserve();
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
                            string messageReturn = CommandBotParser.ApplyCommand(message.Text);
                            await botClient.SendTextMessageAsync(message.Chat.Id, messageReturn,
                           replyToMessageId: message.MessageId);
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
            return ReadFile(Environment.CurrentDirectory + "\\password_bot.txt");
            //return ReadFile("/root/secret_keys/password_bot.txt");
        }

        private void ReadProcessObserve()
        {
            string nameProcess = ReadFile(Environment.CurrentDirectory + "\\name_process_observer.txt");
            //string nameProcess = ReadFile("/root/secret_keys/name_process_observer.txt");
            if (string.IsNullOrEmpty(nameProcess)) return;

            string[] nameArrayProcess = nameProcess.Split(':');
            for(int i = 0; i < nameArrayProcess.Length; i++)
			{
                listNameProccessObserve.Add(nameArrayProcess[i]);
            }
        }

        private string ReadFile(string path)
		{
            FileStream fstream = File.OpenRead(path);
            //Test on Windows 
            //int lengthFstream = array.Length;
            //For server on Ubuntu
            long lengthFstream = fstream.Length;
            byte[] array = new byte[lengthFstream];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }
    }
}
