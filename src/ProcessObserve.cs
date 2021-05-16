using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using TelegramBot_Observer.src.Database.PostgreSQL;
using TelegramBot_Observer.src.Database.Redis;

namespace TelegramBot_Observer.src
{
	class ProcessObserve
	{
		private const int timeSleep = 10000;
		public static List<string> ListNameProccessObserve { get; private set; }
		public static List<string> ListProcForPushMessageAttention { get; private set; }
		private RedisController redisController;
		private PostgreSqlController postgreSqlController;

		public static void AddProcess(string nameProcess)
		{
			ListNameProccessObserve.Add(nameProcess);
			ListProcForPushMessageAttention.Add(nameProcess);
		}

		public static void RemoveProcess(string nameProcess)
		{
			ListNameProccessObserve.Remove(nameProcess);
			ListProcForPushMessageAttention.Remove(nameProcess);
		}

		public static void Clear()
		{
			ListNameProccessObserve.Clear();
			ListProcForPushMessageAttention.Clear();
		}

		public void SetRedisController(RedisController redisController)
		{
			this.redisController = redisController;
		}

		public void SetPostgreSqlController(PostgreSqlController postgreSqlController)
		{
			this.postgreSqlController = postgreSqlController;
		}

		public void LookAtProcess()
		{
			ListNameProccessObserve = new List<string>();
			ListProcForPushMessageAttention = new List<string>();
			ReadProcessObserve();

			for(int i = 0; i < ListNameProccessObserve.Count; i++)
			{
				ListProcForPushMessageAttention.Add(ListNameProccessObserve[i]);
			}

			while (true)
			{
				Process[] allActivityProcesses = Process.GetProcesses();

				for(int i = 0; i < ListNameProccessObserve.Count; i++)
				{
					bool isFind = CheckIsProcInActivivity(ListNameProccessObserve[i], allActivityProcesses);

					if (!isFind)
					{
						if(ListProcForPushMessageAttention.Contains(ListNameProccessObserve[i]))
						{
							PushAttentionProcessFailed(ListNameProccessObserve[i]);
							ListProcForPushMessageAttention.Remove(ListNameProccessObserve[i]);
						}
					}
					else
					{
						if (!ListProcForPushMessageAttention.Contains(ListNameProccessObserve[i]))
						{
							ListProcForPushMessageAttention.Add(ListNameProccessObserve[i]);
						}
					}
				}

				Thread.Sleep(timeSleep);
			}
		}

		private bool CheckIsProcInActivivity(string nameProc, Process[] allActivityProcesses)
		{
			bool isFind = false;
			for (int indexProcess = 0; indexProcess < allActivityProcesses.Length; indexProcess++)
			{
				if (allActivityProcesses[indexProcess].ProcessName == nameProc)
				{
					isFind = true;
					break;
				}
			}
			return isFind;
		}

		private async void PushAttentionProcessFailed(string nameProcess)
		{
			long[] allChatId = redisController.GetAllChatId();
			for(int i = 0; i < allChatId.Length; i++)
			{
				await Bot.botClient.SendTextMessageAsync(allChatId[i], "Что-то случилось с процессом: " + nameProcess);
			}
		}

		private void ReadProcessObserve()
		{
			string[] nameArrayProcess = postgreSqlController.GetAllProcessObserve();
			for (int i = 0; i < nameArrayProcess.Length; i++)
			{
				if (!string.IsNullOrEmpty(nameArrayProcess[i]))
					ListNameProccessObserve.Add(nameArrayProcess[i]);
			}
		}
	}
}
