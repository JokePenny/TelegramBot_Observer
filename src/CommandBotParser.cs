using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot_Observer.src
{
	class CommandBotParser
	{
        private static List<string> listCommands = new List<string>();

        internal static string ApplyCommand(string command)
        {
            try
            {
                switch (command.ToLower())
                {
                    case "h":
                    case "help":
                        return CommandHelp();
                    default:
                        throw new Exception("[CS] Unknow command: " + command);
                }
            }
            catch (Exception error)
            {
                return error.Message;
            }
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
    }
}
