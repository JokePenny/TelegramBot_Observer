using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot_Observer.src
{
    class CommandBotParser : Bot
    {
        private static List<string> commands = new List<string>
        {
            "/add_observe",
            "/delete_observe",
            "/show_list_observe",
            "/clear_list_observe",
            "/show_status",
            "/add_user",
            "/delete_user",
        };

        private static Dictionary<string, string> dictionaryCommandsDescription = new Dictionary<string, string>
        {
            {"/add_observe","/add_observe [имя процесса] (добавляет в список отслеживаний указанный процесс)"},
            {"/delete_observe","/delete_observe [имя процесса] (удаляет из списка отслеживаний указанный процесс)"},
            {"/show_list_observe","/show_list_observe (выводит список отслеживаний)"},
            {"/clear_list_observe","/clear_list_observe (очистка списка отслеживаний)"},
            {"/show_status","/show_status (выводит состояние всех процессов в списке отслеживаний)"},
            {"/add_user","/add_user [chat_id] [уровень доступа] (добавляет нового пользователя для бота)"},
            {"/delete_user","/delete_user [chat_id] (удаляет пользователя)"},
        };

        private static Dictionary<string, int> dictionaryCommands = new Dictionary<string, int>
        {
            {"/login", 0},
            {"/add_observe", 1},
            {"/delete_observe", 1},
            {"/show_list_observe", 0},
            {"/clear_list_observe", 1},
            {"/show_status", 0},
            {"/add_user", 2},
            {"/delete_user", 2},
        };

        public static string ApplyCommand(string message, int accessLevel)
        {
			try
			{
                string[] messages = SplitCommand(message);

                if(dictionaryCommands.ContainsKey(messages[0].ToLower()))
				{
                    if (dictionaryCommands[messages[0].ToLower()] > accessLevel)
                    {
                        return "Низкий уровень доступа для данной команды!";
                    }
                }
				else
				{
                    return "Такой команды я не знаю :(";
                }

                switch (messages[0].ToLower())
                {
                    case "/help":
                        return CommandHelp(accessLevel);
                    case "/add_observe":
                        return CommandAddObserve(messages[1]);
                    case "/delete_observe":
                        return CommandDeleteObserve(messages[1]);
                    case "/show_list_observe":
                        return CommandShowListProcessObserve();
                    case "/clear_list_observe":
                        return CommandClearListProcessObserve();
                    case "/show_status":
                        return CommandShowStatusProcess();
                    case "/add_user":
                        return "not work";
                    case "/delete_user":
                        return "not work";
                    case "/login":
                        return "Вы уже авторизовались";
                    default:
                        return "Такой команды я не знаю :(";
                }
			}
			catch (Exception error)
			{
				ConsoleHelper.WriteError(error.Message);
                return "Что-то мне плохо :(";
			}
		}

        public static string ApplyCommandLogIn(string message, long chatId)
        {
            try
            {
                string[] messages = SplitCommand(message);

                if(messages[0].ToLower() == "/login")
				{
                    string passwordUser = postgreSqlController.GetPasswordUser(chatId);
                    if(messages[1] == passwordUser)
					{
                        redisController.SetLogInUser(chatId);
                        return "Авторизация пройдена успешно!";
                    }
                    return "Пароль неверный :(";
                }
				else
				{
                    return "Для авторизации введите:\n/login [пароль]";
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

        private static string CommandHelp(int accessLevel)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < commands.Count; i++)
            {
                if(dictionaryCommands[commands[i]] <= accessLevel)
				{
                    stringBuilder.Append(dictionaryCommandsDescription[commands[i]]);
                    stringBuilder.Append("\n");
                }
            }
            return stringBuilder.ToString();
        }

        private static string CommandAddObserve(string message)
        {
            if (string.IsNullOrEmpty(message)) return "Ты забыл указать имя процесса для отслеживания!";
            if (ProcessObserve.ListNameProccessObserve.Contains(message)) return "Этот процесс уже отслеживается!";
            postgreSqlController.AddProcessObserve(message);
            ProcessObserve.AddProcess(message);
            return "Хорошо, я добавила. Буду теперь за ним следить :)";
        }

        private static string CommandDeleteObserve(string message)
        {
            if (string.IsNullOrEmpty(message)) return "Ты забыл указать имя процесса за которым больше не надо наблюдать!";
            if (!ProcessObserve.ListNameProccessObserve.Contains(message)) return "За таким процессом я не наблюдаю!";
            postgreSqlController.DeleteProcessObserve(message);
            ProcessObserve.RemoveProcess(message);
            return "Окей, больше не буду наблюдать за этим процессом :)";
        }

        private static string CommandShowListProcessObserve()
        {
            if (ProcessObserve.ListNameProccessObserve.Count == 0) return "Сейчас я ни за кем не наблюдаю!";
            StringBuilder stringBUilder = new StringBuilder();
            stringBUilder.Append("+------<[Process]>-----+\n");
            for (int i = 0; i < ProcessObserve.ListNameProccessObserve.Count; i++)
			{
                stringBUilder.Append(i + 1 + ") " + ProcessObserve.ListNameProccessObserve[i] + "\n");
			}
            stringBUilder.Append("+-----------------------------+\n");
            return stringBUilder.ToString();
        }

        private static string CommandClearListProcessObserve()
        {
            if (ProcessObserve.ListNameProccessObserve.Count == 0) return "Сейчас я ни за кем не наблюдаю!";
            postgreSqlController.ClearProcessObserve();
            ProcessObserve.Clear();
            return "Больше я ни за кем не наблюдаю, буду отдыхать :)";
        }

        private static string CommandShowStatusProcess()
        {
            if (ProcessObserve.ListNameProccessObserve.Count == 0) return "Сейчас я ни за кем не наблюдаю!";

            StringBuilder stringBUilder = new StringBuilder();
            bool isFindProcess = false;
            stringBUilder.Append("+------<[Process]>-----+\n");
            for (int i = 0; i < ProcessObserve.ListNameProccessObserve.Count; i++)
            {
                for (int j = 0; j < ProcessObserve.ListProcForPushMessageAttention.Count; j++)
                {
                    if(ProcessObserve.ListProcForPushMessageAttention[j] == ProcessObserve.ListNameProccessObserve[i])
					{
                        isFindProcess = true;
                        break;
                    }
                }

                stringBUilder.Append(i + 1 + ") " + ProcessObserve.ListNameProccessObserve[i] + ": " + (isFindProcess ? "work" : "failed") + "\n");
            }
            stringBUilder.Append("+-----------------------------+\n");
            return stringBUilder.ToString();
        }
    }
}
