using Npgsql;
using System;
using System.IO;
using System.Text;

namespace TelegramBot_Observer.src.Database.PostgreSQL
{
	class PostgreSqlController
	{
        protected static NpgsqlConnection postgreSqlConnection;

        private string users = "users";
        private string passwordBot = "password_bot";
        private string processObserve = "process_observe";

        public void Connect()
        {
            string[] data = ReadDataPasswordsPostgreSQL();
            string host = data[0];
            string username = data[1];
            string password = data[2];
            string nameDatabase = data[3];
            string stringConnectPostgreSql = $"Host={host};Username={username};Password={password};Database={nameDatabase}";

            postgreSqlConnection = new NpgsqlConnection(stringConnectPostgreSql);
            postgreSqlConnection.Open();

            string sql = "SELECT version()";
            NpgsqlCommand cmd = new NpgsqlCommand(sql, postgreSqlConnection);
            string version = cmd.ExecuteScalar().ToString();
            Console.WriteLine($"PostgreSQL version: {version}");
        }

        private string[] ReadDataPasswordsPostgreSQL()
        {
            FileStream fstream = File.OpenRead(SystemGeneral.GetPathPasswordPostgres());
            byte[] array = new byte[fstream.Length];
            fstream.Read(array, 0, array.Length);
            fstream.Close();
            return Encoding.Default.GetString(array).Split(':');
        }

        public void RegistrationUser(string chatId, int accessLevel)
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand
            (
                $"INSERT INTO {users} (chat_id, access_level)" +
                $"VALUES('{chatId}', '{accessLevel}'); ",
                postgreSqlConnection
            );
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            npgSqlDataReader.Close();
        }

        public void AddProcessObserve(string nameProcess)
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand
            (
                $"INSERT INTO {processObserve} (name)" +
                $"VALUES('{nameProcess}'); ",
                postgreSqlConnection
            );
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            npgSqlDataReader.Close();
        }

        public void DeleteProcessObserve(string nameProcess)
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand
            (
                $"DELETE FROM {processObserve} WHERE name = '{nameProcess}';",
                postgreSqlConnection
            );
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            npgSqlDataReader.Close();
        }

        public void ClearProcessObserve()
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand
            (
                $"DELETE FROM {processObserve};",
                postgreSqlConnection
            );
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            npgSqlDataReader.Close();
        }

        public string[] GetAllProcessObserve()
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT * FROM {processObserve};", postgreSqlConnection);
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            StringBuilder stringBuilder = new StringBuilder();
            if (npgSqlDataReader.HasRows)
            {
                while (npgSqlDataReader.Read())
                {
                    stringBuilder.Append(npgSqlDataReader[1].ToString());
                    stringBuilder.Append(",");
                }
            }

            npgSqlDataReader.Close();
            return stringBuilder.ToString().Split(',');
        }

        public bool CheckUserIsRegistred(string chatId)
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT * FROM {users} WHERE chat_id = '{chatId}';", postgreSqlConnection);
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            bool isPlayerRegistered = npgSqlDataReader.HasRows;
            npgSqlDataReader.Close();
            return isPlayerRegistered;
        }

        public bool CheckUserPassword(string password)
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT * FROM {users} WHERE password = '{password}';", postgreSqlConnection);
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            bool isPlayerRegistered = npgSqlDataReader.HasRows;
            npgSqlDataReader.Close();
            return isPlayerRegistered;
        }

        public bool CheckUserAccessLevel(int requiredLevel, string chatId)
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT * FROM {users} WHERE chat_id = '{chatId}';", postgreSqlConnection);
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            bool isHasRows = npgSqlDataReader.HasRows;
            bool isHasAccess = false;
            if (isHasRows && Int32.Parse(npgSqlDataReader[3].ToString()) <= requiredLevel)
			{
                isHasAccess = true;
			}
            npgSqlDataReader.Close();
            return isHasAccess;
        }

        public string GetPasswordsBot(string nameBot)
		{
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT * FROM {passwordBot} WHERE name = '{nameBot}';", postgreSqlConnection);
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            string passwordForBot = "";
            if (npgSqlDataReader.HasRows)
            {
                while (npgSqlDataReader.Read())
                {
                    passwordForBot = npgSqlDataReader[2].ToString();
                }
            }

            npgSqlDataReader.Close();
            return passwordForBot;
        }

        public string[] GetAllUsersChatId()
		{
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT * FROM {users};", postgreSqlConnection);
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            StringBuilder stringBuilder = new StringBuilder();
            if (npgSqlDataReader.HasRows)
            {
                while (npgSqlDataReader.Read())
                {
                    stringBuilder.Append(npgSqlDataReader[1].ToString());
                    stringBuilder.Append(":");
                    stringBuilder.Append(npgSqlDataReader[3].ToString());
                    stringBuilder.Append(",");
                }
            }

            npgSqlDataReader.Close();
            return stringBuilder.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        public string GetPasswordUser(long chatId)
        {
            NpgsqlCommand npgSqlCommand = new NpgsqlCommand($"SELECT password FROM {users} WHERE chat_id = '{chatId}';", postgreSqlConnection);
            NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
            string password = "";
            if (npgSqlDataReader.HasRows)
            {
                while (npgSqlDataReader.Read())
                {
                    password = npgSqlDataReader[0].ToString();
                }
            }

            npgSqlDataReader.Close();
            return password;
        }
    }
}
