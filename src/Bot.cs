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
        public static Bot Instance { get; private set; }
        public static TelegramBotClient botClient;
        public static string ChatId;

        private Thread threadProcessObserver;

        public Bot()
		{
            if(Instance == null)
			{
                Instance = this;

            }
			else
			{
                
			}
		}

        protected internal async void Listen()
        {
            try
            {
                ConsoleHelper.WriteSuccess("Bot started");

                string key = ReadDataPasswordsBot();
                ProcessObserve processObserver = new ProcessObserve();
                botClient = new TelegramBotClient(key);
                await botClient.SetWebhookAsync("");
                int offset = 0;

                threadProcessObserver = new Thread(processObserver.LookAtProcess);
                threadProcessObserver.Start();

                while (true)
                {
                    var updates = await botClient.GetUpdatesAsync(offset);

                    foreach (var update in updates)
                    {
                        var message = update.Message;
                        if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                        {
                            string messageReturn = CommandBotParser.ApplyCommand(message.Text);
                            Console.WriteLine(message.Chat.Id);
                            await botClient.SendTextMessageAsync(message.Chat.Id, messageReturn, replyToMessageId: message.MessageId);
                        }
                        offset = update.Id + 1;
                    }

                }
            }
            catch (Telegram.Bot.Exceptions.ApiRequestException ex)
            {
                Console.WriteLine(ex.Message);
            }
            if(threadProcessObserver != null) threadProcessObserver.Join();
        }

        public void Disconnect()
        {
            ConsoleHelper.WriteError("Bot disconnect");
            Environment.Exit(0);
        }

        private string ReadDataPasswordsBot()
        {
            return SystemGeneral.ReadFile(SystemGeneral.GetPathToToken());
        }
    }
}
