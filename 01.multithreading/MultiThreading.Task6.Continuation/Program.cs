/*
*  Create a Task and attach continuations to it according to the following criteria:
   a.    Continuation task should be executed regardless of the result of the parent task.
   b.    Continuation task should be executed when the parent task finished without success.
   c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
   d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled
   Demonstrate the work of the each case with console utility.
*/
using System;
using System.Threading.Tasks;
using System.Threading;

namespace MultiThreading.Task6.Continuation
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Create a Task and attach continuations to it according to the following criteria:");
            Console.WriteLine("a.    Continuation task should be executed regardless of the result of the parent task.");
            Console.WriteLine("b.    Continuation task should be executed when the parent task finished without success.");
            Console.WriteLine("c.    Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation.");
            Console.WriteLine("d.    Continuation task should be executed outside of the thread pool when the parent task would be cancelled.");
            Console.WriteLine("Demonstrate the work of the each case with console utility.");
            Console.WriteLine();

            Task parentTask = Task.Run(() =>
            {
                Console.WriteLine("Running parent task...");
            });

            parentTask.ContinueWith(t =>
            {
                Console.WriteLine("Continuation regardless of the result of parent task...");
            });

            Thread.Sleep(1000);
            Console.WriteLine();


            // Continuation task should be executed when the parent task was completed without success.
            parentTask = Task.Run(() =>
            {
                Console.WriteLine("Running faulty parent task...");
                throw new Exception("Parent task exception.");
            });

            parentTask.ContinueWith(t =>
            {
                Console.WriteLine("Continuation on unsuccessful parent task: " + t.Exception.GetBaseException().Message);
            }, TaskContinuationOptions.OnlyOnFaulted);

            Thread.Sleep(1000);
            Console.WriteLine();


            // Continuation task should be executed when the parent task failed and parent task thread should be reused for continuation
            parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Running faulty parent task with reused thread...");
                throw new Exception("Faulty parent task exception");
            }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);

            parentTask.ContinueWith(t =>
            {
                Console.WriteLine("Continuation on same task: " + t.Exception.GetBaseException().Message);
            }, CancellationToken.None, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Current);

            Thread.Sleep(1000);
            Console.WriteLine();


            // Continuation task should be executed outside of the thread pool when the parent task is cancelled
            CancellationTokenSource cts = new CancellationTokenSource();
            parentTask = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("Running cancellable parent task...");
                while (true)
                {
                    cts.Token.ThrowIfCancellationRequested();
                }
            }, cts.Token);

            parentTask.ContinueWith(t =>
            {
                Console.WriteLine("Continuation outside of the Thread pool after task cancellation: " + t.Exception.GetBaseException().Message);
            }, TaskContinuationOptions.NotOnRanToCompletion | TaskContinuationOptions.LongRunning);

            Thread.Sleep(1000);
            cts.Cancel();

            Console.ReadLine();
        }
    }
}
