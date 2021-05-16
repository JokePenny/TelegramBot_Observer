using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Telegram.Bot;
using TelegramBot_Observer.src.Database.PostgreSQL;
using TelegramBot_Observer.src.Database.Redis;

namespace TelegramBot_Observer.src
{
	class Bot
	{
        public static Bot Instance { get; private set; }
        public static TelegramBotClient botClient;

        private Thread threadProcessObserver;

        protected static PostgreSqlController postgreSqlController;
        protected static RedisController redisController;

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

                postgreSqlController = new PostgreSqlController();
                postgreSqlController.Connect();

                redisController = new RedisController();
                redisController.Connect();

                string[] allUsers = postgreSqlController.GetAllUsersChatId();
                redisController.UpdateStatusUsers(allUsers);

                string key = postgreSqlController.GetPasswordsBot("emma");
                botClient = new TelegramBotClient(key);
                await botClient.SetWebhookAsync("");
                int offset = 0;

                ProcessObserve processObserver = new ProcessObserve();
                processObserver.SetRedisController(redisController);
                processObserver.SetPostgreSqlController(postgreSqlController);
                threadProcessObserver = new Thread(processObserver.LookAtProcess);
                threadProcessObserver.Start();

                PushAttentionBotStart();
                while (true)
                {
                    var updates = await botClient.GetUpdatesAsync(offset);

                    foreach (var update in updates)
                    {
                        var message = update.Message;

                        if(redisController.CheckUserExist(message.Chat.Id))
						{
                            if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                            {
                                string messageReturn;
                                if (redisController.CheckLogInUser(message.Chat.Id))
								{
                                    int accessLevelUSer = redisController.GetAccessLevelUser(message.Chat.Id);
                                    messageReturn = CommandBotParser.ApplyCommand(message.Text, accessLevelUSer);
                                }
								else
								{
                                    messageReturn = CommandBotParser.ApplyCommandLogIn(message.Text, message.Chat.Id);
                                }
                                await botClient.SendTextMessageAsync(message.Chat.Id, messageReturn, replyToMessageId: message.MessageId);
                            }
                            offset = update.Id + 1;
                        }
						else
						{
                            string messageForRegistration = "Передай данный айди администратору:" + message.Chat.Id;
                            await botClient.SendTextMessageAsync(message.Chat.Id, messageForRegistration, replyToMessageId: message.MessageId);
                            offset = update.Id + 1;
                        }
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

        private async void PushAttentionBotStart()
        {
            long[] allChatId = redisController.GetAllChatId();
            for (int i = 0; i < allChatId.Length; i++)
            {
                await Bot.botClient.SendTextMessageAsync(allChatId[i], "Привет, я снова в строю!");
            }
        }
    }
}
