using RabbitMQ.Client;
using Newtonsoft.Json;
using System.Text;

class DataCaptureService
{
    private const string MonitorFolderPath = @"C:\MonitorFolder\";
    private const string QueueName = "processing_queue";
    private const int ChunkSize = 256 * 1024; // 256 KB per chunk
    private const int MaxRetryCount = 3; // Maximum number of retries to read a file
    private const int RetryDelayMilliseconds = 1000; // 1 second delay between retries

    public static async Task StartAsync()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        await MonitorFolderAsync(channel);
    }

    private static async Task MonitorFolderAsync(IModel channel)
    {
        while (true)
        {
            var files = Directory.GetFiles(MonitorFolderPath, "*.pdf");

            var fileTasks = files.Select(filePath => HandleFileAsync(channel, filePath)).ToArray();
            await Task.WhenAll(fileTasks);
        }
    }

    private static async Task HandleFileAsync(IModel channel, string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        byte[] fileContent = null;

        for (int retry = 0; retry < MaxRetryCount; retry++)
        {
            try
            {
                fileContent = await File.ReadAllBytesAsync(filePath);
                break;
            }
            catch (IOException)
            {
                Console.WriteLine($"File {filePath} is in use, retrying...");
                await Task.Delay(RetryDelayMilliseconds);
            }
        }

        if (fileContent == null)
        {
            Console.WriteLine($"Failed to read {filePath} after {MaxRetryCount} retries, skipping...");
            return;
        }

        var totalChunks = (int)Math.Ceiling((double)fileContent.Length / ChunkSize);

        var chunkTasks = Enumerable.Range(0, totalChunks)
            .Select(i => SendChunkAsync(channel, fileName, fileContent, i, totalChunks))
            .ToArray();

        await Task.WhenAll(chunkTasks);

        Console.WriteLine($"Sent {fileName} in {totalChunks} chunks");

        File.Delete(filePath);
    }

    private static async Task SendChunkAsync(IModel channel, string fileName, byte[] fileContent, int chunkIndex, int totalChunks)
    {
        var chunkContent = fileContent.Skip(chunkIndex * ChunkSize).Take(ChunkSize).ToArray();

        var chunkedMessage = new ChunkedMessage
        {
            FileName = fileName,
            TotalChunks = totalChunks,
            ChunkIndex = chunkIndex,
            FileChunk = chunkContent
        };

        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(chunkedMessage));

        await Task.Run(() => channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body));
    }
}

public class ChunkedMessage
{
    public string FileName { get; set; }
    public int TotalChunks { get; set; }
    public int ChunkIndex { get; set; }
    public byte[] FileChunk { get; set; }
}