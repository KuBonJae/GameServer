using System;
using System.Runtime.Intrinsics.Arm;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        // Race Condition -> 공유 자원에 무질서하게 접근한다
        
        static int number = 0;

        static void Thread_1()
        {
            for(int i=0; i<10000; i++)
            {
                int now = number; // 이미 다른 곳에서 작업하고 있을 수 있어 원하는 값 받아오기 힘들 수 있음
                int after = Interlocked.Increment(ref number); // return 값으로 받아오면 원하는 값 받아오기 보장
                
                // 원자성이 없으면 문제 (한번에 일어나야하는 작업 뭉탱이)
                //number++;

                // number++은 어셈블리 코드 상 다음과 같이 풀어쓸 수 있다.
                //int temp = number;
                //temp += 1;
                //number = temp; // 여기에 대입하는 숫자를 누가 선점해서 넣냐에 따라 결과값이 달라지게 될 것
                
            }
        }

        static void Thread_2() 
        {
            for (int i = 0; i < 10000; i++)
            {
                Interlocked.Decrement(ref number);
                //number--;
            }
        }
        static void Main(string[] args)
        {
            number++;

            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}