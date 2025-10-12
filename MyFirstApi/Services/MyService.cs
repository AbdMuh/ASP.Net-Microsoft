public class MyService : IMyService
{
    private readonly int _requestId;
    public void LogCreation(string message)
    {
        Console.WriteLine($"MyService instance created: {message} , RequestId: {_requestId}");
    }

    public MyService()
    {
        _requestId = new Random().Next(1, 1000);
    }
}