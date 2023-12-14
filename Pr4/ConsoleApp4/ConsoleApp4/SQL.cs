using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Threading;
using System.Data.SqlClient;

namespace Bot
{
    class SQL
    {
        private static long chatId;

        public static async Task Insert(ITelegramBotClient arg1, Update arg2, CancellationToken arg3, string result)
        {
            try
            {
                string[] parts = result.Split(' ');
                chatId = arg2.Message.Chat.Id;
                string baseCurrency = parts[0];
                string targetCurrency = parts[2];
                string exchangeDate = parts[5].Replace(':', ' ').Trim();
                string exchangeRate = parts[8];

                using (var connection = new SqlConnection("Data Source=DESKTOP-I3330L2\\SQLEXPRESS;Initial Catalog=Exchange;Integrated Security=True;"))
                {
                    connection.Open();

                    string tableName = $"{baseCurrency}_{Math.Abs(chatId)}";

                    string checkTableQuery = $"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '{tableName}') CREATE TABLE {tableName} (BaseCurrency NVARCHAR(5) NOT NULL, TargetCurrency NVARCHAR(5) NOT NULL, ExchangeDate DATE NOT NULL, ExchangeRate FLOAT NOT NULL,)";
                    using (var command = new SqlCommand(checkTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                     string insertQuery = $@" IF NOT EXISTS (SELECT 1 FROM {tableName} WHERE TargetCurrency = @TargetCurrency AND ExchangeDate = @ExchangeDate) BEGIN INSERT INTO {tableName} (BaseCurrency, TargetCurrency, ExchangeDate, ExchangeRate) VALUES (@BaseCurrency, @TargetCurrency,  @ExchangeDate, @ExchangeRate); END";
                    using (var command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@BaseCurrency", baseCurrency);
                        command.Parameters.AddWithValue("@TargetCurrency", targetCurrency);
                        command.Parameters.AddWithValue("@ExchangeDate", DateTime.Parse(exchangeDate));
                        command.Parameters.AddWithValue("@ExchangeRate", Convert.ToDouble(exchangeRate));
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при внесении данных в базу: " + ex.Message);
            }
        }
        public static async Task ExtractData(ITelegramBotClient arg1, Update arg2, CancellationToken arg3)
        {
            try
            {
                chatId = arg2.Message.Chat.Id;
            string[] messageParts = arg2.Message.Text.Split(' ');
            string baseCurrency = messageParts[1];
            string targetCurrency = messageParts.Length > 2 ? messageParts[2] : "";
            string exchangeDate = messageParts.Length > 2 ? messageParts[2] : "";
            if (messageParts.Length == 4)
            {
                exchangeDate = messageParts[3];
            }
            string result = "";
            using (var connection = new SqlConnection("Data Source=DESKTOP-I3330L2\\SQLEXPRESS;Initial Catalog=Exchange;Integrated Security=True;"))
            {
                connection.Open();
                string tableName = $"{baseCurrency}_{Math.Abs(chatId)}";
                string selectQuery;
                if (messageParts.Length == 2)
                {
                    selectQuery = $"SELECT * FROM {tableName}";
                        result = await ExecuteSelectQuery(selectQuery, arg1, arg2);
                }
                else if (messageParts.Length == 3 && !DateTime.TryParse(targetCurrency, out _))
                {
                    selectQuery = $"SELECT * FROM {tableName} WHERE TargetCurrency = '{targetCurrency}'";
                        result = await ExecuteSelectQuery(selectQuery, arg1, arg2);
                    }
                else if (messageParts.Length == 3 && DateTime.TryParse(targetCurrency, out _))
                {
                    selectQuery = $"SELECT * FROM {tableName} WHERE  ExchangeDate = '{exchangeDate}'";
                        result = await ExecuteSelectQuery(selectQuery, arg1, arg2);
                    }
                else if (messageParts.Length == 4)
                {
                    selectQuery = $"SELECT * FROM {tableName} WHERE TargetCurrency = '{targetCurrency}' AND ExchangeDate = '{exchangeDate}'";
                        result = await ExecuteSelectQuery(selectQuery, arg1, arg2);
                    }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при чтении данных из базы: " + ex.Message);
                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Произошла ошибка при чтении данных из базы: " + ex.Message);
            }
        }


        public static async Task VietMy(ITelegramBotClient arg1, Update arg2)
        {
            try
            {
                chatId = arg2.Message.Chat.Id;
                string[] messageParts = arg2.Message.Text.Split(' ');
                string result = "";
                using (var connection = new SqlConnection("Data Source=DESKTOP-I3330L2\\SQLEXPRESS;Initial Catalog=Exchange;Integrated Security=True;"))
                {
                    connection.Open();
                    string tableName = $"My_{Math.Abs(chatId)}";
                    string selectQuery;
                        selectQuery = $"SELECT * FROM {tableName}";
                        result = await ExecuteSelectQuery(selectQuery, arg1, arg2);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при чтении данных из базы: " + ex.Message);
                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Произошла ошибка при чтении данных из базы: " + ex.Message);
            }
        }


        private static async Task<string> ExecuteSelectQuery(string selectQuery, ITelegramBotClient arg1, Update arg2)
    {
        string result = "";
            chatId = arg2.Message.Chat.Id;
            using (var connection = new SqlConnection("Data Source=DESKTOP-I3330L2\\SQLEXPRESS;Initial Catalog=Exchange;Integrated Security=True;"))
        {
            connection.Open();
            using (var command = new SqlCommand(selectQuery, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result += $"{reader.GetName(i)}: {reader.GetValue(i)}\n";
                            }
                            result += "\n";
                            if (result.Length > 4000)
                            {
                                await arg1.SendTextMessageAsync(chatId, result);
                                result = "Результат вашего запроса (продолжение):\n";
                            }
                        }
                    }
                    else
                    {
                        result = "Запрос вернул пустой результат.";
                    }
                    await arg1.SendTextMessageAsync(chatId, result);
                }
            }
        }
        return result;
    }

        public static async Task InsertMy(ITelegramBotClient arg1, Update arg2)
        {
            try
            {
                var chatId = arg2.Message.Chat.Id;
                string[] commands = arg2.Message.Text.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                using (var connection = new SqlConnection("Data Source=DESKTOP-I3330L2\\SQLEXPRESS;Initial Catalog=Exchange;Integrated Security=True;"))
                {
                    connection.Open();
                    string tableName = $"My_{Math.Abs(chatId)}";
                    string checkTableQuery = $@"IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = '{tableName}') CREATE TABLE {tableName} (Id INT IDENTITY(1,1) PRIMARY KEY, BaseCurrency NVARCHAR(5) NOT NULL, TargetCurrency NVARCHAR(5) NOT NULL)";
                    using (var command = new SqlCommand(checkTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    for (int i = 1; i < commands.Length - 1; i += 2)
                    {
                        string baseCurrency = commands[i];
                        string targetCurrency = commands[i + 1];

                        string insertQuery = $@"IF NOT EXISTS (SELECT 1 FROM {tableName} WHERE BaseCurrency = @BaseCurrency AND TargetCurrency = @TargetCurrency)
                                    BEGIN
                                        INSERT INTO {tableName} (BaseCurrency, TargetCurrency) VALUES (@BaseCurrency, @TargetCurrency);
                                    END";
                        using (var insertCommand = new SqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@BaseCurrency", baseCurrency);
                            insertCommand.Parameters.AddWithValue("@TargetCurrency", targetCurrency);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
                SQL.VietMy(arg1, arg2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при внесении данных в базу: " + ex.Message);
            }
        }

        public static string GetCurrencyById(ITelegramBotClient arg1, Update arg2)
        {
            var chatId = arg2.Message.Chat.Id;
            string[] messageParts = arg2.Message.Text.Split(' ');
            string id = messageParts[1];
            string result = "";
            try
            {
                using (var connection = new SqlConnection("Data Source=DESKTOP-I3330L2\\SQLEXPRESS;Initial Catalog=Exchange;Integrated Security=True;"))
                {
                    connection.Open();
                    string tableName = $"My_{chatId}";

                    string selectQuery = $@"SELECT BaseCurrency, TargetCurrency FROM {tableName} WHERE Id = @Id";
                    using (var command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                reader.Read();
                                result = $"BaseCurrency: {reader.GetString(0)} TargetCurrency: {reader.GetString(1)}";
                            }
                            else
                            {
                                result = "Запрос вернул пустой результат.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при чтении данных из базы: " + ex.Message);
            }
            return result;
        }

        public static async Task DelMy(ITelegramBotClient arg1, Update arg2)
        {
            try
            {
                chatId = arg2.Message.Chat.Id;
                string[] messageParts = arg2.Message.Text.Split(' ');
                string iD = messageParts[1];
                string result = "";
                using (var connection = new SqlConnection("Data Source=DESKTOP-I3330L2\\SQLEXPRESS;Initial Catalog=Exchange;Integrated Security=True;"))
                {
                    connection.Open();
                    string tableName = $"My_{Math.Abs(chatId)}";
                    string checkTableQuery = $"Delete FROM {tableName} where id= {iD}";
                    using (var command = new SqlCommand(checkTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                SQL.VietMy(arg1, arg2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при чтении данных из базы: " + ex.Message);
                await arg1.SendTextMessageAsync(arg2.Message.Chat.Id, "Произошла ошибка при удалении данных из базы: " + ex.Message);
            }
        }
        public static async Task ChangeMy(ITelegramBotClient arg1, Update arg2)
        {
            try
            {
                var chatId = arg2.Message.Chat.Id;
                string[] commands = arg2.Message.Text.Split();

                using (var connection = new SqlConnection("Data Source=DESKTOP-I3330L2\\SQLEXPRESS;Initial Catalog=Exchange;Integrated Security=True;"))
                {
                    connection.Open();
                    string tableName = $"My_{Math.Abs(chatId)}";

                    string baseCurrency = commands[2];
                    string targetCurrency = commands[3];
                    string Id = commands[1];

                    string updateQuery = $@"UPDATE {tableName} 
                                  SET BaseCurrency = @BaseCurrency, TargetCurrency = @TargetCurrency 
                                  WHERE Id = @Id";

                    using (var updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@BaseCurrency", baseCurrency);
                        updateCommand.Parameters.AddWithValue("@TargetCurrency", targetCurrency);
                        updateCommand.Parameters.AddWithValue("@Id", Id);

                        updateCommand.ExecuteNonQuery();
                    }
                }

                SQL.VietMy(arg1, arg2);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка при внесении данных в базу: " + ex.Message);
            }
        }
    }

}
