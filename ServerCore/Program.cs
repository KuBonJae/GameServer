using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                // 데이터 받기
                byte[] recvBuff = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Client] : {recvData}");

                // 데이터 보내기
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                clientSocket.Send(sendBuff);

                // 쫒아내기
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Main(string[] args)
        {
            // DNS (Domain Name System) -> 서버의 주소 (172.1.2.3) 를 도메인 이름으로 치환 (www.domain.com)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            
            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");

            while (true)
            {

            }
            
        }
    }
}