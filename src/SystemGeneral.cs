using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TelegramBot_Observer.src
{
	public static class SystemGeneral
	{
        private static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        public static string GetPathToToken()
		{
            return IsLinux ? "/root/secret_keys/password_bot.txt"
                : Environment.CurrentDirectory + "\\password_bot.txt";
        }

        public static string GetPathToProcessObserve()
        {
            return IsLinux ? "/root/secret_keys/name_process_observer.txt"
                : Environment.CurrentDirectory + "\\name_process_observer.txt";
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
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
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
            string[] listInfo = Encoding.Default.GetString(array).Split(IsLinux ? "\n" : "\n\r");

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < listInfo.Length; i++)
			{
                if(!string.IsNullOrEmpty(listInfo[i]) && listInfo[i] != info)
				{
                    stringBuilder.Append(listInfo[i] + "\n");
                }
			}

            byte[] byteConvert = Encoding.Default.GetBytes(stringBuilder.ToString());

            fstream.Write(byteConvert, 0, byteConvert.Length);
            fstream.Close();
        }

        public static void ClearInfoFile(string path)
        {
            string clear = "";
            FileStream fstream = File.OpenRead(path);
            byte[] byteConvert = Encoding.Default.GetBytes(clear.ToString());
            fstream.Write(byteConvert, 0, byteConvert.Length);
            fstream.Close();
        }
    }
}
