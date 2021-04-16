using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TelegramBot_Observer.src
{
	class ProcessObserve
	{
		private const int timeSleep = 10000;
		public static List<string> ListNameProccessObserve { get; private set; }
		public static List<string> ListNameProccessPushMessageAttention { get; private set; }

		public static void AddProcess(string nameProcess)
		{
			ListNameProccessObserve.Add(nameProcess);
			ListNameProccessPushMessageAttention.Add(nameProcess);
		}

		public static void RemoveProcess(string nameProcess)
		{
			ListNameProccessObserve.Remove(nameProcess);
			ListNameProccessPushMessageAttention.Remove(nameProcess);
		}

		public static void Clear()
		{
			ListNameProccessObserve.Clear();
			ListNameProccessPushMessageAttention.Clear();
		}

		public void LookAtProcess()
		{
			ListNameProccessObserve = new List<string>();
			ListNameProccessPushMessageAttention = new List<string>();
			ReadProcessObserve();

			for(int i = 0; i < ListNameProccessObserve.Count; i++)
			{
				ListNameProccessPushMessageAttention.Add(ListNameProccessObserve[i]);
			}

			while (true)
			{
				Process[] allActivityProcesses = Process.GetProcesses();

				for(int i = 0; i < ListNameProccessObserve.Count; i++)
				{
					bool isFind = false;
					for (int indexProcess = 0; indexProcess < allActivityProcesses.Length; indexProcess++)
					{
						if(allActivityProcesses[indexProcess].ProcessName == ListNameProccessObserve[i])
						{
							isFind = true;
							break;
						}
					}

					if(!isFind)
					{
						if(ListNameProccessPushMessageAttention.Contains(ListNameProccessObserve[i]))
						{
							PushAttentionProcessFailed(ListNameProccessObserve[i]);
							ListNameProccessPushMessageAttention.Remove(ListNameProccessObserve[i]);
						}
					}
				}

				Thread.Sleep(timeSleep);
			}
		}

		private async void PushAttentionProcessFailed(string nameProcess)
		{
			await Bot.botClient.SendTextMessageAsync(Bot.ChatId, "Что-то случилось с процессом: " + nameProcess);
		}

		private void ReadProcessObserve()
		{
			string nameProcess = SystemGeneral.ReadFile(SystemGeneral.GetPathToProcessObserve());

			string[] nameArrayProcess = nameProcess.Split('\n');
			for (int i = 0; i < nameArrayProcess.Length; i++)
			{
				if (!string.IsNullOrEmpty(nameArrayProcess[i]))
					ListNameProccessObserve.Add(nameArrayProcess[i]);
			}
		}
	}
}
