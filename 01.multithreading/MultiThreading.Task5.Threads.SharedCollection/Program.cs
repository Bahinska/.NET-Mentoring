/*
 * 5. Write a program which creates two threads and a shared collection:
 * the first one should add 10 elements into the collection and the second should print all elements
 * in the collection after each adding.
 * Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.
 */
using System;
using System.Collections.Generic;
using System.Threading;

namespace MultiThreading.Task5.Threads.SharedCollection
{
    class Program
    {
        private static List<int> sharedCollection = new List<int>();
        private static ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.WriteLine("5. Write a program which creates two threads and a shared collection:");
            Console.WriteLine("the first one should add 10 elements into the collection and the second should print all elements in the collection after each adding.");
            Console.WriteLine("Use Thread, ThreadPool or Task classes for thread creation and any kind of synchronization constructions.");
            Console.WriteLine();

            Thread thread1 = new Thread(AddElements);
            Thread thread2 = new Thread(PrintElements);

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Console.ReadLine();
        }

        private static void AddElements()
        {
            for (int i = 1; i <= 10; i++)
            {
                lock (sharedCollection)
                {
                    sharedCollection.Add(i);
                }

                manualResetEvent.Set();

                Thread.Sleep(100);
            }
        }

        private static void PrintElements()
        {
            for (int i = 1; i <= 10; i++)
            {
                manualResetEvent.WaitOne();

                lock (sharedCollection)
                {
                    Console.WriteLine(string.Join(",", sharedCollection));
                }

                manualResetEvent.Reset();
            }
        }
    }
}
