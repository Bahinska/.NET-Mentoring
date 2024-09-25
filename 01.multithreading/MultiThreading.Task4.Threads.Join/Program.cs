/*
 * 4.	Write a program which recursively creates 10 threads.
 * Each thread should be with the same body and receive a state with integer number, decrement it,
 * print and pass as a state into the newly created thread.
 * Use Thread class for this task and Join for waiting threads.
 * 
 * Implement all of the following options:
 * - a) Use Thread class for this task and Join for waiting threads.
 * - b) ThreadPool class for this task and Semaphore for waiting threads.
 */

using System;
using System.Threading;

namespace MultiThreading.Task4.Threads.Join
{
    class Program
    {
        static Semaphore semaphore = new Semaphore(0, 1);
        static void Main(string[] args)
        {
            Console.WriteLine("4.	Write a program which recursively creates 10 threads.");
            Console.WriteLine("Each thread should be with the same body and receive a state with integer number, decrement it, print and pass as a state into the newly created thread.");
            Console.WriteLine("Implement all of the following options:");
            Console.WriteLine();
            Console.WriteLine("- a) Use Thread class for this task and Join for waiting threads.");
            Console.WriteLine("- b) ThreadPool class for this task and Semaphore for waiting threads.");

            Console.WriteLine();

            Option1();
            //Option2();

            Console.ReadLine();
        }

        static void Option1()
        {
            Thread thread = new Thread(new ParameterizedThreadStart(ThreadJoin));
            thread.Start(10);

            thread.Join();
        }

        static void ThreadJoin(object state)
        {
            int val = (int)state;
            Console.WriteLine(val);

            val--;

            if (val > 0)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(ThreadJoin));
                thread.Start(val);
                thread.Join();
            }
        }

        static void Option2()
        {
            ThreadPool.QueueUserWorkItem(ThreadPoolSemaphore, 10);

            semaphore.WaitOne();
        }

        static void ThreadPoolSemaphore(object state)
        {
            int val = (int)state;
            Console.WriteLine(val);

            val--;

            if (val > 0)
            {
                ThreadPool.QueueUserWorkItem(ThreadPoolSemaphore, val);
            }
            else
            {
                semaphore.Release();
            }
        }
    }
}
