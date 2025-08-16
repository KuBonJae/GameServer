using System;
using System.Runtime.Intrinsics.Arm;
using System.Threading;

namespace ServerCore
{
    class Lock
    {
        // 커널(운영체제)쪽에서 체크하는 bool 값이라고 볼 수 있음
        AutoResetEvent _available = new AutoResetEvent(true);

        public void Acquire()
        {
            _available.WaitOne(); // 입장 시도
        }

        public void Release()
        {
            _available.Set();
        }
    }

    class Program
    {
        static int _num = 0;
        static Lock _lock = new Lock();

        static void Thread1()
        {
            for(int i=0; i < 10000;i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread2()
        {
            for (int i = 0; i < 10000; i++)
            {
                _lock.Acquire();
                _num--;
                _lock.Release();
            }
        }
        static void Main(string[] args)
        {
            Task t1 = new Task(Thread1);
            Task t2 = new Task(Thread2);
            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);

            Console.WriteLine(_num);
        }
    }
}