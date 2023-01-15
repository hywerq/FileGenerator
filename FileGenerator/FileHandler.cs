using System.Data.SqlClient;

namespace FileGenerator
{
    public class FileHandler
    {
        private static readonly string _folder = "Output";
        private static readonly string _dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), _folder);
        private readonly DirectoryInfo _directoryInfo = new DirectoryInfo(_dir);

        public FileHandler()
        {
            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }
        }

        public void GenerateFiles(int count)
        {
            List<StreamWriter> streamWriters = new List<StreamWriter>();

            Parallel.For(0, count, i => {
                streamWriters.Add(new StreamWriter(Path.Combine(_dir, $"file_{i + 1}.txt")));
            });

            Parallel.ForEach(streamWriters, streamWriter => {
                for (int i = 0; i < 100000; i++)
                {
                    streamWriter.WriteLine(RandomData.GetFormattedRandomDataLine());
                }

                streamWriter.Dispose();
            });
        }

        public string MergeFilesInto(string fileName, string removeSequence = " ")
        {
            IEnumerable<FileInfo> files = _directoryInfo.EnumerateFiles("file_*.*");
            FileInfo destination = new FileInfo(Path.Combine(_dir, $"{fileName}_merged.txt"));

            string removed = "";

            foreach (FileInfo file in files)
            {
                List<string> lines = File.ReadAllLines(file.FullName).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
                Console.WriteLine("\n- {0} removed {1} lines", file.Name, lines.RemoveAll(x => x.Contains(removeSequence)));                

                File.AppendAllLines(destination.FullName, lines);
            }

            return removed;
        }

        public void ImportFileDataToDB(string filePath, string connectionString)
        {
            try
            {
                FileInfo file = new FileInfo(filePath);
                List<string> lines = File.ReadAllLines(file.FullName).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "INSERT INTO [value] ([vl_date], [vl_line_eng], [vl_line_rus], [vl_int_num], [vl_dec_num]) " +
                        "VALUES (@date, @eng, @rus, @int, @dec)";

                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        int rowsAffected = 0;

                        lines.ForEach(async line => {
                            string[] args = line.Split("||");

                            command.Parameters.AddWithValue("@date", DateTime.ParseExact(args[0], "dd.MM.yyyy", null));
                            command.Parameters.AddWithValue("@eng", args[1]);
                            command.Parameters.AddWithValue("@rus", args[2]);
                            command.Parameters.AddWithValue("@int", int.Parse(args[3]));
                            command.Parameters.AddWithValue("@dec", double.Parse(args[4]));

                            rowsAffected += await command.ExecuteNonQueryAsync();
                            Console.WriteLine("Rows affected: {0}, rows left: {1}", rowsAffected, lines.Count - rowsAffected);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
