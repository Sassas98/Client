using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Client {
    public class Program {

        private TcpClient client;
        private Thread thread;
        private string name;

        //scelgo il nome, preparo il client tcp, mando il nome e poi inizio a chattare
        //con un thread ascolto gli altri messaggi
        public Program() {
            name = GetResponse("Inserire Nickname.");
            var ip = IPAddress.Loopback;
            Console.Clear();
            client = new();
            client.Connect(ip, 5000);
            thread = new Thread(ReceiveData);
            thread.Start();
            SendName();
            SendData();
        }

        //chiede e prende un nick
        private string GetResponse(string s) {
            Console.WriteLine(s);
            return Console.ReadLine();
        }

        //manda il nome
        private void SendName() {
            var stream = client.GetStream();
            byte[] buffer = Encoding.UTF8.GetBytes(name);
            stream.Write(buffer, 0, buffer.Length);
            name += ": ";
        }

        //manda i messaggi
        private void SendData() {
            var stream = client.GetStream();
            string s;
            while (!string.IsNullOrEmpty((s = Console.ReadLine()))) {
                byte[] buffer = Encoding.UTF8.GetBytes(name + s);
                stream.Write(buffer, 0, buffer.Length);
            }
            client.Client.Shutdown(SocketShutdown.Send);
            thread.Join();
            stream.Close();
            client.Close();
        }

        //riceve i dati
        private void ReceiveData() {
            var stream = client.GetStream();
            byte[] Buffer = new byte[1024];
            int byteCount;
            while ((byteCount = stream.Read(Buffer, 0, Buffer.Length)) > 0) {
                Console.WriteLine(Encoding.UTF8.GetString(Buffer, 0, byteCount));
            }
        }

        //avvia il client
        public static void Main(string[] args) {
            new Program();
        }
    }
}