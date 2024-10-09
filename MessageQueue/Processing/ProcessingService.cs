using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;

class ProcessingService
{
    private const string QueueName = "processing_queue";
    private const string StoragePath = @"C:\ProcessedFiles\";
    private static readonly ConcurrentDictionary<string, List<byte[]>> FileChunks = new();

    public static async Task StartAsync()
    {
        Directory.CreateDirectory(StoragePath);

        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += async (model, ea) =>
        {       
            await ProcessChunkedMessageAsync(ea);
        };
        channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

        Console.ReadLine();
    }

    private static ChunkedMessage DeserializeMessage(byte[] body)
    {
        var message = Encoding.UTF8.GetString(body);
        return JsonConvert.DeserializeObject<ChunkedMessage>(message);
    }

    private static async Task ProcessChunkedMessageAsync(BasicDeliverEventArgs eventArgs)
    {
        var chunkedMessage = DeserializeMessage(eventArgs.Body.ToArray());

        if (!FileChunks.ContainsKey(chunkedMessage.FileName))
        {
            FileChunks[chunkedMessage.FileName] = new List<byte[]>(new byte[chunkedMessage.TotalChunks][]);
        }

        FileChunks[chunkedMessage.FileName][chunkedMessage.ChunkIndex] = chunkedMessage.FileChunk;

        if (FileChunks[chunkedMessage.FileName].All(chunk => chunk != null))
        {
            var fullFileContent = FileChunks[chunkedMessage.FileName].SelectMany(chunk => chunk).ToArray();
            await SaveFile(chunkedMessage.FileName, fullFileContent);
            FileChunks.TryRemove(chunkedMessage.FileName, out _);
        }
    }

    private static async Task SaveFile(string fileName, byte[] content)
    {
        var filePath = Path.Combine(StoragePath, fileName);
        await File.WriteAllBytesAsync(filePath, content);
        Console.WriteLine($"Received and stored {fileName}");
    }
}

public class ChunkedMessage
{
    public string FileName { get; set; }
    public int TotalChunks { get; set; }
    public int ChunkIndex { get; set; }
    public byte[] FileChunk { get; set; }
}