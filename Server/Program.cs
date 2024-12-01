using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    public class Server
    {
        private TcpListener _server;
        private int _port;
        private bool _isRunning;

        public Server(int port)
        {
            _port = port;
            _server = new TcpListener(IPAddress.Any, _port);
            _isRunning = false;
        }

        public void Start()
        {
            _server.Start();
            _isRunning = true;
            Console.WriteLine("Server started on port " + _port);

            while (_isRunning)
            {
                TcpClient client = _server.AcceptTcpClient();
                Console.WriteLine("Клиент подключен.");
                HandleClient(client);
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[256];
                
                while (true)
                {
                    // Чтение данных
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // Клиент отключился
                    
                    string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("Получено: " + dataReceived);

                    // Отправка ответа
                    string response = "Ответ от сервера";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseBytes, 0, responseBytes.Length);
                    Console.WriteLine("Ответ отправлен.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при обработке клиента: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Клиент отключен.");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _server.Stop();
            Console.WriteLine("Server stopped");
        }
    }

    public class Client
    {
        private TcpClient _client;
        private string _serverIp;
        private int _serverPort;

        public Client(string serverIp, int serverPort)
        {
            _serverIp = serverIp;
            _serverPort = serverPort;
            _client = new TcpClient();
        }

        public void Connect()
        {
            try
            {
                _client.Connect(_serverIp, _serverPort);
                Console.WriteLine("Connected to server");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to server: " + ex.Message);
            }
        }

        public void SendMessage(string message)
        {
            if (!_client.Connected)
            {
                Console.WriteLine("Not connected to server");
                return;
            }

            try
            {
                NetworkStream stream = _client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                Console.WriteLine("Message sent: " + message);

                // Получение ответа
                byte[] buffer = new byte[256];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Received response: " + response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }

        public void Disconnect()
        {
            if (_client.Connected)
            {
                _client.Close();
                Console.WriteLine("Disconnected from server");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 8080;
            Server server = new Server(port);
            server.Start();
        }
    }
}
