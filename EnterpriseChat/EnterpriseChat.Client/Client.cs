using System.Net.Sockets;
using System.Text;

public class Client
{
    private Socket clientSocket;

    public Client(string serverAddress, int port)
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            clientSocket.Connect(serverAddress, port);
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Unable to connect to server: {e.Message}");
            return;
        }
    }

    public void SendMessage(string message)
    {
        try
        {
            Console.WriteLine("Client send: " + message);
            clientSocket.Send(Encoding.ASCII.GetBytes(message));
        }
        catch (SocketException e)
        {
            Console.WriteLine($"Unable to send message: {e.Message}");
        }
    }

    public void StartReceiving()
    {
        ThreadPool.QueueUserWorkItem(arg =>
        {
            try
            {
                var buffer = new byte[1024];
                while (true)
                {
                    var received = clientSocket.Receive(buffer);
                    var data = Encoding.ASCII.GetString(buffer, 0, received);
                    Console.WriteLine("Client receive: " + data);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Connection to server lost: {e.Message}");
                clientSocket.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
            }
        }, null);
    }
}