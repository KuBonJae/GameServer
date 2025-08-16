using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    // 재귀적 Lock 허용 YES { WriteLock->WriteLock O WriteLock->ReadLock O }
    // 스핀락 정책 ( 5000번 -> Yield ) 
    class Lock
    {
        const int EMPTY_FLAG = 0x00000000;
        const int WRITE_MASK = 0x7FFF0000; // 16진수 = 4비트 -> F는 모든 4 비트가 1로 변환, 7은 Unused 위치인 맨 앞의 8 하나가 빠진 값 
        const int READ_MASK = 0x0000FFFF;
        const int MAX_SPIN_COUNT = 5000;

        // [unused(1)] [WriteThreadID(15)] {ReadCount(16)]
        int _flag = EMPTY_FLAG;
        int _writeCount = 0;

        public void WriteLock()
        {
            // 동일 Thread가 이미 WriteLock을 가지고 있는지 확인
            int lockThreadID = (_flag & WRITE_MASK) >> 16;
            if(Thread.CurrentThread.ManagedThreadId == lockThreadID)
            {
                _writeCount++;
                return;
            }
            // 아무도 Write/Read Lock을 가지고 있지 않다면, 소유권을 얻기 위해 노력한다.
            int desired = (Thread.CurrentThread.ManagedThreadId << 16) & WRITE_MASK;
            while(true)
            {
                for(int i=0;i<MAX_SPIN_COUNT;i++)
                {
                    // 시도 성공하면 return
                    if (Interlocked.CompareExchange(ref _flag, desired, EMPTY_FLAG) == EMPTY_FLAG)
                    {
                        _writeCount = 1;
                        return;
                    }
                }

                Thread.Yield(); // 너무 오래 반복하면 나와서 잠시 쉬어감
            }
        }

        public void WriteUnlock()
        {
            int lockCount = --_writeCount;
            if(lockCount == 0)
                Interlocked.Exchange(ref _flag, EMPTY_FLAG);
        }

        public void ReadLock()
        {
            int lockThreadID = (_flag & WRITE_MASK) >> 16;
            if (Thread.CurrentThread.ManagedThreadId == lockThreadID)
            {
                Interlocked.Increment(ref _flag);
                return;
            }

            // 아무도 Write하고 있지 않다면, ReadCount를 1 증가시킴
            while (true)
            {
                for(int i=0;i<MAX_SPIN_COUNT;i++)
                {
                    int expected = (_flag & READ_MASK); // _flag 값이 예상 값과 바뀐다면 다시 재도전 -> 경합에서 패배해 누군가 먼저 접근했다는 의미
                    if (Interlocked.CompareExchange(ref _flag, expected + 1, expected) == expected)
                        return;
                }

                Thread.Yield();
            }
        }

        public void ReadUnlock()
        {
            Interlocked.Decrement(ref _flag);
        }
    }
}
