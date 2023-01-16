using System.Data;
using System.Data.SqlClient;

namespace FileOperationsTask1
{
    public static class DatabaseHandler
    {
        public static async Task ImportFileDataToDBAsync(string filePath, string connectionString)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);
                List<string> lines = File.ReadAllLines(file.FullName).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO [file_data] ([fd_date], [fd_line_eng], [fd_line_rus], [fd_int_num], [fd_dec_num]) " +
                        "VALUES (@date, @eng, @rus, @int, @dec)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.Add("@date", SqlDbType.Date);
                        command.Parameters.Add("@eng", SqlDbType.VarChar);
                        command.Parameters.Add("@rus", SqlDbType.VarChar);
                        command.Parameters.Add("@int", SqlDbType.Int);
                        command.Parameters.Add("@dec", SqlDbType.Float);

                        int rowsAffected = 0;

                        foreach(String line in lines)
                        {
                            string[] args = line.Split("||");

                            command.Parameters["@date"].Value = DateTime.ParseExact(args[0], "dd.MM.yyyy", null);
                            command.Parameters["@eng"].Value = args[1];
                            command.Parameters["@rus"].Value = args[2];
                            command.Parameters["@int"].Value = int.Parse(args[3]);
                            command.Parameters["@dec"].Value = double.Parse(args[4]);

                            rowsAffected += await command.ExecuteNonQueryAsync();
                            Console.WriteLine("Rows added: {0}, rows left: {1}", rowsAffected, lines.Count - rowsAffected);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static async Task<(long, double)> CcalculateSumAndMedian(string connectionString)
        {
            (long sum, double median) result = (0, 0);

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT SUM(CAST(fd_int_num AS bigint)), AVG(fd_dec_num) " +
                        "FROM file_data";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        var reader = await command.ExecuteReaderAsync();

                        if(reader.Read())
                        {
                            result.sum = reader.GetInt64(0);
                            result.median = reader.GetDouble(1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return result;
        }
    }
}
