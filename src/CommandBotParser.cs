using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace TelegramBot_Observer.src
{
    class CommandBotParser : Bot
    {
        private static List<string> listCommands = new List<string>()
        {
            "/help (показывает список всех доступных команд)",
            "/add_observe [имя процесса] (добавляет в список отслеживаний указанный процесс)",
            "/delete_observe [имя процесса] (удаляет из списка отслеживаний указанный процесс)",
            "/show_list_observe (выводит список отслеживаний)",
            "/clear_list_observe (очистка списка отслеживаний)",
            "/status (выводит состояние всех процессов в списке отслеживаний)",
            "/set_alaram [on/off] (включение оповещения об завершении процесса из списка отслеживаний)",
            "/set_period_diagnostic [1h1m, 1h, 1m] (установка времени, через которое бот будет посылать сообщение об состоянии сервера)",
            "/set_greeting [строка с приветствием] (устанавливает строку приветствия)"
        };

        internal static void ApplyCommand(Message message)
        {
            try
            {
                switch (message.Text.ToLower())
                {
                    case "/help":
                        CommandHelp(message);
                        break;
                    default:
                        throw new Exception("[CS] Unknow command: " + message.Text);
                }
            }
            catch (Exception error)
            {
                ConsoleHelper.WriteError(error.Message);
            }
        }

        private static async void CommandHelp(Message message)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < listCommands.Count; i++)
            {
                stringBuilder.Append(listCommands[i]);
                stringBuilder.Append("\n");
            }
            await botClient.SendTextMessageAsync(message.Chat.Id, stringBuilder.ToString(),
                                       replyToMessageId: message.MessageId);
        }
    }
}
