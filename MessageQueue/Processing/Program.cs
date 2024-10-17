namespace Processing
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await ProcessingService.StartAsync();
        }
    }
}
