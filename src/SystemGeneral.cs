using System;
using System.IO;
using System.Text;

namespace TelegramBot_Observer.src
{
	public static class SystemGeneral
	{
        private static string pathFileTokenBotLinux = "/root/secret_keys/password_bot.txt";
        private static string pathFileTokenBotWindows = "\\password_bot.txt";
        private static string pathFileObserveProcessLinux = "/root/secret_keys/name_process_observer.txt";
        private static string pathFileObserveProcessWindows = "\\name_process_observer.txt";
        private static string pathFileChatIdLinux = "/root/secret_keys/name_process_observer.txt";
        private static string pathFileChatIdWindows = "\\name_process_observer.txt";
        private static string pathFilePasswordPostgresLinux = "/root/secret_keys/password_postgresql_bot.txt";
        private static string pathFilePasswordPostgresWindows = "\\password_postgresql_bot.txt";

        private static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static string GetTokenSeparationString()
        {
            return IsLinux ? "\n" : "\r\n";
        }

        public static string GetPathPasswordPostgres()
        {
            return IsLinux ? pathFilePasswordPostgresLinux
                : Environment.CurrentDirectory + pathFilePasswordPostgresWindows;
        }

        public static string GetPathToToken()
		{
            return IsLinux ? pathFileTokenBotLinux
                : Environment.CurrentDirectory + pathFileTokenBotWindows;
        }

        public static string GetPathToProcessObserve()
        {
            return IsLinux ? pathFileObserveProcessLinux
                : Environment.CurrentDirectory + pathFileObserveProcessWindows;
        }

        public static string GetPathToChatId()
        {
            return IsLinux ? pathFileChatIdLinux
                : Environment.CurrentDirectory + pathFileChatIdWindows;
        }

        public static string ReadFile(string path)
        {
            FileStream fstream = File.OpenRead(path);
            long lengthFstream = IsLinux ? fstream.Length - 1 : fstream.Length;
            byte[] array = new byte[lengthFstream];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array);
        }

        public static void WriteAppendInfoFile(string path, string info)
        {
            using (StreamWriter sw = new StreamWriter(path, true, Encoding.Default))
            {
                sw.WriteLine(info);
            }
        }

        public static void RemoveInfoInFile(string path, string info)
        {
            FileStream fstream = File.OpenRead(path);
            long lengthFstream = IsLinux ? fstream.Length - 1 : fstream.Length;
            byte[] array = new byte[lengthFstream];
            fstream.Read(array, 0, array.Length);
            string[] listInfo = Encoding.Default.GetString(array).Split(GetTokenSeparationString());
            fstream.Close();

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < listInfo.Length - 1; i++)
			{
                if(!string.IsNullOrEmpty(listInfo[i]) && listInfo[i] != info)
				{
                    stringBuilder.Append(listInfo[i] + GetTokenSeparationString());
                }
			}

            if (!string.IsNullOrEmpty(listInfo[listInfo.Length - 1]) && listInfo[listInfo.Length - 1] != info)
            {
                stringBuilder.Append(listInfo[listInfo.Length - 1] + GetTokenSeparationString());
            }

            byte[] byteConvert = Encoding.Default.GetBytes(stringBuilder.ToString());

            fstream = File.Create(path);
            fstream.Write(byteConvert, 0, byteConvert.Length);
            fstream.Close();
        }

        public static void ClearInfoFile(string path)
        {
            FileStream fstream = File.Create(path);
            byte[] byteConvert = Encoding.Default.GetBytes("");
            fstream.Write(byteConvert, 0, byteConvert.Length);
            fstream.Close();
        }
    }
}
