using System;
using System.Runtime.Intrinsics.Arm;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static int number = 0;
        static object _obj = new object();

        static void Thread_1()
        {
            try { } finally { } // 이걸 사용해서 무조건 Exit 시킬 수 있음

            for(int i=0; i<10000; i++)
            {
                lock(_obj) // Monitor.Enter(_obj)과 같은 방식으로 내부 구현되어있다.
                {
                    number++;
                }

                //Monitor.Enter(_obj); // 문 잠구기
                //
                //number++;
                //
                //// return; // 이래버리면, _obj가 반환되지 않아 Thread_2가 무한 대기 (DeadLock) 상태가 되어버린다.
                //
                //Monitor.Exit(_obj); // 잠금 해제
            }
        }

        static void Thread_2() 
        {
            for (int i = 0; i < 10000; i++)
            {
                lock (_obj)
                {
                    number--;
                }

                //Monitor.Enter(_obj);
                //
                //number--;
                //
                //Monitor.Exit(_obj);
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread_1);
            Task t2 = new Task(Thread_2);

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(number);
        }
    }
}