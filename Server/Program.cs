using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int port = 8080;
            TcpListener server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Console.WriteLine("Server started");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Клиент подключен.");
                NetworkStream stream = client.GetStream();

                // Чтение данных
                byte[] buffer = new byte[256];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine("Получено: " + dataReceived);

                // Отправка ответа
                string response = "Ответ от сервера";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("Ответ отправлен.");

                client.Close();
            }
        }
    }
}
