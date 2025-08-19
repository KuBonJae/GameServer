using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;

namespace DummyClient
{
    class Program
    {

        static void Main(string[] args)
        {
            // DNS (Domain Name System) -> 서버의 주소 (172.1.2.3) 를 도메인 이름으로 치환 (www.domain.com)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            while(true)
            {
                try
                {
                    // 고객 휴대폰 설정
                    Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                    // 가게 입장 문의
                    socket.Connect(endPoint); // 블로킹함수
                    Console.WriteLine($"Connected to {socket.RemoteEndPoint.ToString()}");

                    // 보낸다
                    byte[] sendBuffer = Encoding.UTF8.GetBytes("Hello Server!");
                    int sendBytes = socket.Send(sendBuffer);

                    // 받는다
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server] : {recvData}");

                    // 나간다
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(500);
            }
        }
    }
}