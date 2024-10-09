namespace DataCapture
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await DataCaptureService.StartAsync();
        }
    }
}
