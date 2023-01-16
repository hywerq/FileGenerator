using FileOperationsTask1;

void OperationCompleteWaiting(CancellationToken token)
{
    Console.Write("Loading files");

    while (!token.IsCancellationRequested)
    { 
        Console.Write(".");
        Thread.Sleep(500);
    }
}

CancellationTokenSource tokenSource = new();
Thread progressThread = new(() => OperationCompleteWaiting(tokenSource.Token));

Console.WriteLine("\nPress any key to start task1:");
Console.ReadKey();

//task 1

FileHandler fileHandler = new FileHandler();

try
{
    progressThread.Start();

    fileHandler.GenerateFiles(100);

    Console.WriteLine("\nGenerated successfully.");
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}
finally
{
    tokenSource.Cancel();
    progressThread.Join();
    tokenSource.Dispose();
}

Console.WriteLine("\nPress any key to start task2:");
Console.ReadKey();

//task 2
try
{
    Console.WriteLine("Removing lines containing \'a\' symbol ");

    fileHandler.MergeFilesInto("result", "a");

    Console.WriteLine("\nMerged successfully.");
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}

Console.WriteLine("\nPress any key to start task3:");
Console.ReadKey();

//task3
await DatabaseHandler.ImportFileDataToDBAsync(
    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "import_file.txt"),
    "Data Source=.\\sqlexpress;Initial Catalog=FileDB;Integrated Security=True"
    );

Console.WriteLine("\nPress any key to start task4:");
Console.ReadKey();

//task4
var result = await DatabaseHandler.CcalculateSumAndMedian(
    "Data Source=.\\sqlexpress;Initial Catalog=FileDB;Integrated Security=True");

Console.WriteLine("Sum: {0}, median: {1}", result.Item1, result.Item2);

Console.ReadKey();