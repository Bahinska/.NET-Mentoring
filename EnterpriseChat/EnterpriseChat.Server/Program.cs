using System.Net.Sockets;
using System.Net;

public class Program
{
    static void Main(string[] args)
    {
        Server server = new Server();
        server.Start();
        Console.ReadKey();
    }
}