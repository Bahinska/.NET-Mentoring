using System.Net.Sockets;
public class Program
{
    static void Main(string[] args)
    {
        Client client = new Client("127.0.0.1", 8080);
        client.StartReceiving();
        Console.WriteLine("Client started");

        string[] messages = new string[] { "Hello", "How are you?", "Goodbye" };

        Random random = new Random();
        while (true)
        {
            foreach (var message in messages)
            {
                client.SendMessage(message);
                Thread.Sleep(random.Next(1000, 5000));
            }
        }
    }
}