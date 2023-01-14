namespace FileGenerator
{
    public class FileHandler
    {
        private static readonly string _folder = "Output";
        private readonly string _dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), _folder);

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
                    streamWriter.WriteLine("{0}||{1}", i, RandomData.GetFormattedRandomDataLine());
                }

                streamWriter.Dispose();
            });
        }
    }
}
