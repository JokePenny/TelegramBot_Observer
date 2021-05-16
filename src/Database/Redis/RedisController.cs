using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBot_Observer.src.Database.Redis
{
	class RedisController
	{
        private ConnectionMultiplexer redis;
        protected static IDatabase dbRedis;
        private List<long> allChatId = new List<long>();

        public void Connect()
        {
            redis = ConnectionMultiplexer.Connect("localhost");
            dbRedis = redis.GetDatabase();
        }

        public void UpdateStatusUsers(string[] allUsersChatId)
		{
            for(int i = 0; i < allUsersChatId.Length; i++)
			{
                string[] userInfo = allUsersChatId[i].Split(':');
                allChatId.Add(Convert.ToInt64(userInfo[0]));
                for (int j = 0; j < userInfo.Length; j++)
                {
                    dbRedis.StringSet("chat_id_" + userInfo[0], "0");
                    dbRedis.StringSet("access_level_" + userInfo[0], userInfo[1]);
                }
            }
		}

        public long[] GetAllChatId()
        {
            return allChatId.ToArray();
        }

        public bool CheckUserExist(long chatId)
        {
            return dbRedis.KeyExists("chat_id_" + chatId);
        }

        public bool CheckLogInUser(long chatId)
        {
            return dbRedis.StringGet("chat_id_" + chatId) == "1";
        }

        public bool SetLogInUser(long chatId)
        {
            return dbRedis.StringSet("chat_id_" + chatId, "1");
        }

        public int GetAccessLevelUser(long chatId)
        {
            return Convert.ToInt32(dbRedis.StringGet("access_level_" + chatId));
        }
    }
}
