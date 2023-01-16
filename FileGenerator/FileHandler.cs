namespace FileOperationsTask1
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

        public void MergeFilesInto(string fileName, string removeSequence = " ")
        {
            IEnumerable<FileInfo> files = _directoryInfo.EnumerateFiles("file_*.*");
            FileInfo destination = new FileInfo(Path.Combine(_dir, $"{fileName}_merged.txt"));

            foreach (FileInfo file in files)
            {
                List<string> lines = File.ReadAllLines(file.FullName).Where(arg => !string.IsNullOrWhiteSpace(arg)).ToList();
                Console.WriteLine("- {0} removed {1} lines", file.Name, lines.RemoveAll(x => x.Contains(removeSequence)));                

                File.AppendAllLines(destination.FullName, lines);
            }
        }
    }
}
