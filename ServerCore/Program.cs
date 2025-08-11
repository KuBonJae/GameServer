using System;
using System.Threading;

namespace ServerCore
{
    class Program
    {
        static void MainThread(object? state)
        {
            for (int i = 0; i < 5; i++) 
                Console.WriteLine("Hello Thread");
        }

        static void Main(string[] args)
        {
            // Task 자체도 ThreadPool에서 관리함
            Task t = new Task(() => { while (true) { } }, TaskCreationOptions.LongRunning); // 긴 스레드 시간 -> 별도의 스레드 따로 만들어냄
            t.Start();

            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(5, 5);

            for(int i=0; i<4; i++)
            {
                ThreadPool.QueueUserWorkItem((obj) => { while (true) { } });
            }

            ThreadPool.QueueUserWorkItem(MainThread);

            while(true)
            {

            }

            //Thread t = new Thread(MainThread);
            //t.Name = "Test Thread";
            //t.IsBackground = true; // Main쪽에서 멈추면, thread도 멈춘다.
            //t.Start();
            //Console.WriteLine("Waiting Thread");
            //t.Join(); // t Thread를 기다린다.

            //Console.WriteLine("Hello World");
        }
    }
}