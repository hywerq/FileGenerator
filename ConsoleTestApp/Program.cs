using FileGenerator;

void OperationCompleteWaiting(CancellationToken token)
{
    Console.Write("Loading");

    while (!token.IsCancellationRequested)
    { 
        Console.Write(".");
        Thread.Sleep(500);
    }
}


CancellationTokenSource tokenSource = new();
Thread progressThread = new(() => OperationCompleteWaiting(tokenSource.Token));
FileHandler fileHandler = new FileHandler();

//task 1
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

//task 2
try
{
    string result = fileHandler.MergeFilesInto("result");
    Console.WriteLine("\nMerged successfully." + result);
}
catch (Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}