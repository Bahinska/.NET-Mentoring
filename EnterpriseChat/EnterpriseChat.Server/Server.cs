using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
public class Server
{
    private ConcurrentDictionary<Socket, byte> clients = new ConcurrentDictionary<Socket, byte>();
    private Socket serverSocket;

    public void Start()
    {
        try
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8080));
            serverSocket.Listen(100);

            while (true)
            {
                var clientSocket = serverSocket.Accept();
                clients.TryAdd(clientSocket, 0);

                ThreadPool.QueueUserWorkItem(arg =>
                {
                    try
                    {
                        var buffer = new byte[1024];
                        while (true)
                        {
                            var received = clientSocket.Receive(buffer);
                            var data = Encoding.ASCII.GetString(buffer, 0, received);
                            Console.WriteLine("Server get data: " + data);
                            foreach (var client in clients.Keys)
                            {
                                client.Send(buffer.Take(received).ToArray());
                            }
                        }
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Client disconnected unexpectedly.");
                        clients.TryRemove(clientSocket, out _);
                        clientSocket.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occurred: {e.Message}");
                    }
                });
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine($"SocketException: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}