using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WhaleApp.GameClient
{
    class Program
    {
        private static TcpClient _client;

        static async Task Main(string[] args)
        {
            while (true)
            {
                const string ip = "localhost";
                const int port = 13000;
                _client = new TcpClient();
                try
                {
                    await _client.ConnectAsync(ip, port);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error : " + ex.Message);
                }

                if (!_client.Connected) continue;

                Task.Factory.StartNew(LoopListener);
                var stream = _client.GetStream();
                Console.WriteLine("Connected !");
                var data = Encoding.ASCII.GetBytes($"Name: {TypeYourName()}");
                _client.Client.Send(data);
                while (_client.Connected)
                {
                    Thread.Sleep(1000);
                    var message = Console.ReadLine();
                    _client.Client.Send(Encoding.ASCII.GetBytes(message));
                }

                stream.Close();
                _client.Close();
                Thread.Sleep(10000);
            }
        }

        private static string TypeYourName()
        {
            var name = "";
            var nameIsValid = false;

            while (!nameIsValid)
            {
                Console.Write("Your name: ");
                name = Console.ReadLine();
                
                if (name.Length < 3 || name.Length > 8)
                {
                    Console.WriteLine("Sorry! Your name should in diapason between 2 and 8 letters");
                    continue;
                }


                if (!name.All(t => (char.IsDigit(t) || t >= 'a' && t <= 'z' || t >= 'A' && t <= 'Z')))
                {
                    Console.WriteLine("Sorry! Your name can only contain Latin letters and numbers");
                    continue;
                }

                nameIsValid = true;
            }

            return name;
        }

        private static void LoopListener()
        {
            while (true)
            {
                var bytes = new byte[256];
                int i;
                while (_client.Client != null && (i = _client.Client.Receive(bytes)) != 0)
                {
                    var data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine(data);
                }
                _client.Close();
            }
        }
    }
}