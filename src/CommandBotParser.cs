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
            "/add_observe [имя процесса] (добавляет в список отслеживаний указанный процесс)",
            "/delete_observe [имя процесса] (удаляет из списка отслеживаний указанный процесс)",
            "/show_list_observe (выводит список отслеживаний)",
            "/clear_list_observe (очистка списка отслеживаний)",
            "/status (выводит состояние всех процессов в списке отслеживаний)",
            "/set_alaram [on/off] (включение оповещения об завершении процесса из списка отслеживаний)",
            "/set_period_diagnostic [1h1m, 1h, 1m] (установка времени, через которое бот будет посылать сообщение об состоянии сервера)",
            "/set_greeting [строка с приветствием] (устанавливает строку приветствия)"
        };

        internal static string ApplyCommand(string message)
        {
			try
			{
                string[] messages = SplitCommand(message);

                switch (messages[0].ToLower())
                {
                    case "/help":
                        return CommandHelp();
                    case "/add_observe":
                        return CommandAddObserve(messages[1]);
                    case "/delete_observe":
                        return CommandDeleteObserve(messages[1]);
                    case "/show_list_observe":
                        return CommandShowListProcessObserve();
                    case "/clear_list_observe":
                        return CommandClearListProcessObserve();
                    case "/status":
                        return "in work";
                    case "/set_alaram":
                        return "in work";
                    case "/set_period_diagnostic":
                        return "in work";
                    case "/set_greeting":
                        return "in work";
                    default:
                        ConsoleHelper.WriteError("[CS] Unknow command: " + messages[0]);
                        return "Такой команды я не знаю :(";
                }
			}
			catch (Exception error)
			{
				ConsoleHelper.WriteError(error.Message);
                return "Что-то мне плохо :(";
			}
		}

        private static string[] SplitCommand(string message)
		{
            string[] arrayMessages = new string[2];

            bool findCommand = false;
            int ignoreSpace = 0;
            for(int i = 0; i < message.Length; i++)
			{
                if (message[i] == ' ' && findCommand)
                {
                    arrayMessages[0] = message.Substring(ignoreSpace, i);
                    arrayMessages[1] = message.Substring(i + 1);
                    break;
                }
                else if (message[i] != ' ')
                {
                    findCommand = true;
                }
                else ignoreSpace++;
            }

            if(findCommand && string.IsNullOrEmpty(arrayMessages[1])) arrayMessages[0] = message;

            return arrayMessages;
        }

        private static string CommandHelp()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < listCommands.Count; i++)
            {
                stringBuilder.Append(listCommands[i]);
                stringBuilder.Append("\n");
            }
            return stringBuilder.ToString();
        }

        private static string CommandAddObserve(string message)
        {
            if (string.IsNullOrEmpty(message)) return "Ты забыл указать имя процесса для отслеживания!";
            if (listNameProccessObserve.Contains(message)) return "Этот процесс уже отслеживается!";
            listNameProccessObserve.Add(message);
            return "Хорошо, я добавила. Буду теперь за ним следить :)";
        }

        private static string CommandDeleteObserve(string message)
        {
            if (string.IsNullOrEmpty(message)) return "Ты забыл указать имя процесса за которым больше не надо наблюдать!";
            if (!listNameProccessObserve.Contains(message)) return "За таким процессом я не наблюдаю!";
            listNameProccessObserve.Remove(message);
            return "Окей, больше не буду наблюдать за этим процессом :)";
        }

        private static string CommandShowListProcessObserve()
        {
            if (listNameProccessObserve.Count == 0) return "Сейчас я ни за кем не наблюдаю!";
            StringBuilder stringBUilder = new StringBuilder();
            stringBUilder.Append("Вот список:\n-----------------\n");
            for (int i = 0; i < listNameProccessObserve.Count; i++)
			{
                stringBUilder.Append(i + 1 + ") " + listNameProccessObserve[i] + "\n");
			}
            stringBUilder.Append("-----------------\n");
            return stringBUilder.ToString();
        }

        private static string CommandClearListProcessObserve()
        {
            if (listNameProccessObserve.Count == 0) return "Сейчас я ни за кем не наблюдаю!";
            listNameProccessObserve.Clear();
            return "Больше я ни за кем не наблюдаю, буду отдыхать :)";
        }
    }
}
