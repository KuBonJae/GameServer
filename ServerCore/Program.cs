using System;
using System.Runtime.Intrinsics.Arm;
using System.Threading;

namespace ServerCore
{
    // 메모리 배리어
    // 1. 코드 재배치 억제
    // 2. 가시성

    // 1. Full Memory Barrier = Store/Load 둘다 막기 (read / write )
    // 2. Store Memory Barrier = Store만 막기
    // 3. Load Memory Barrier = Load만 막기

    class Program
    {
        static int x = 0;
        static int y = 0;
        static int r1 = 0;
        static int r2 = 0;

        static void Thread_1()
        {
            y = 1;

            Thread.MemoryBarrier(); // 선을 그어버림. y=1; 이 아래로 내려갈 수 없고, r1=x; 가 위로 올라갈 수 없다.
                                    // 데이터에 내용 쓰기, 내용 불러오기를 해서 동기화 작업을 진행하는 것도 함

            r1 = x;
            // 순서 연관성이 없기 때문에 하드웨어적으로 다음과 같이 변경될 수도 있다
            // r1 = x; 
            // y = 1;
        }

        static void Thread_2()
        {
            x = 1;

            Thread.MemoryBarrier();


            r2 = y;
        }

        static void Main(string[] args)
        {
            int count = 0;
            while(true)
            {
                count++;
                x = y = r1 = r2 = 0;

                Task t1 = new Task(Thread_1);
                Task t2 = new Task(Thread_2);
                t1.Start(); 
                t2.Start();

                Task.WaitAll(t1, t2);

                if (r1 == 0 && r2 == 0)
                    break;
            }

            Console.WriteLine($"{count}번만에 빠져나옴");
        }
    }
}