using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory; // Session을 생성해줄 return 함수

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // TCP 소켓 생성
            _sessionFactory += sessionFactory; // 인자로 들어온 action을 실행할 것이다 명시

            // 문지기 교육 (가게의 입구 주소 입력)
            _listenSocket.Bind(endPoint);

            // 영업 시작 -> 입구를 열어 손님을 맞을 수 있도록 가게 상태를 전환
            _listenSocket.Listen(10); // backlog = 최대 대기 수

            // 손님 입장 대기
            SocketAsyncEventArgs args = new SocketAsyncEventArgs(); // 소켓 비동기 이벤트 관련 클래스
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted); // 비동기 함수의 완료 콜백 연결
            RegisterAccept(args); // 등록
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; // 재사용 전 깔끔하게 비우기

            bool pending = _listenSocket.AcceptAsync(args); // Accept에 성공하면, args.Completed 이벤트가 자동 실행됨
            if (!pending)
                OnAcceptCompleted(null, args); 
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args); // 다음 손님을 위한 재등록
        }
    }
}
