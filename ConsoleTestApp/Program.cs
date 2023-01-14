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

FileHandler fileHandler = new FileHandler();
CancellationTokenSource tokenSource = new();

Thread progressThread = new(() => OperationCompleteWaiting(tokenSource.Token));
progressThread.Start();

try
{
    fileHandler.GenerateFiles(100);
    Console.WriteLine("\nCompleted successfully.");
}
catch(Exception ex)
{
    Console.WriteLine("Error: " + ex.Message);
}
finally
{
    tokenSource.Cancel();
    progressThread.Join();
}

tokenSource.Dispose();