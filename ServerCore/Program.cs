using System;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        // 휘발성 데이터 : 언제 바뀔지 모르니 괜히 최적화 하지 말아라
        volatile static bool _stop = false; // 전역 변수들은 모든 thread가 공통으로 사용
        
        static void ThreadMain()
        {
            Console.WriteLine("쓰레드 시작");

            // Release 시 코드 최적화로 인해 다음과 같이 변경됨
            /*
            if(!_stop)
            {
                while(true) { };
            }
            */

            while (_stop == false) { };

            Console.WriteLine("쓰레드 종료");
        }

        static void Main(string[] args)
        {
            Task t = new Task(ThreadMain);
            t.Start();

            Thread.Sleep(1000); // 메인 쓰레드는 1초동안 잠시 대기

            _stop = true;

            Console.WriteLine("Stop 호출");
            Console.WriteLine("종료 대기중");
            
            t.Wait();

            Console.WriteLine("종료 성공");
        }
    }
}