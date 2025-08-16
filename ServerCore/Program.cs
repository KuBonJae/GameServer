using System;
using System.Runtime.Intrinsics.Arm;
using System.Threading;

namespace ServerCore
{
    class SpinLock
    {
        volatile int _locked = 0;
        public void Acquire()
        {
            while(true)
            {
                //int original = Interlocked.Exchange(ref _locked, 1);
                //if (original == 0)
                //    break;

                if (Interlocked.CompareExchange(ref _locked, 1, 0) == 0)
                    break;
            }
            // _lock 확인 -> 반복문 탈출 -> 변수 변경 => 작업이 하나로 이루어지지 않음 (원자성 X)
            //while(_locked)
            //{
            //    // 잠김 해제 대기
            //}
            //
            //_locked = true;
        }

        public void Release()
        {
            // 본인 스스로만 자물쇠를 쥐고 있는게 확실한 상황이므로, 직접적으로 데이터를 건드려도 문제가 전혀 없다.
            _locked = 0;
        }
    }

    class Program
    {
        static int _num = 0;
        static SpinLock _lock = new SpinLock();

        static void Thread1()
        {
            for(int i=0; i < 100000;i++)
            {
                _lock.Acquire();
                _num++;
                _lock.Release();
            }
        }

        static void Thread2()
        {
            for (int i = 0; i < 100000; i++)
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