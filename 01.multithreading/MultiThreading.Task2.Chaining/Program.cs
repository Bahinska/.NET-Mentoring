/*
 * 2.	Write a program, which creates a chain of four Tasks.
 * First Task – creates an array of 10 random integer.
 * Second Task – multiplies this array with another random integer.
 * Third Task – sorts this array by ascending.
 * Fourth Task – calculates the average value. All this tasks should print the values to console.
 */
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MultiThreading.Task2.Chaining
{
    class Program
    {
        static readonly Random Random = new Random();
        static void Main(string[] args)
        {
            Console.WriteLine(".Net Mentoring Program. MultiThreading V1 ");
            Console.WriteLine("2.	Write a program, which creates a chain of four Tasks.");
            Console.WriteLine("First Task – creates an array of 10 random integer.");
            Console.WriteLine("Second Task – multiplies this array with another random integer.");
            Console.WriteLine("Third Task – sorts this array by ascending.");
            Console.WriteLine("Fourth Task – calculates the average value. All this tasks should print the values to console");
            Console.WriteLine();


            var t1 = Task.Run(() =>
            {
                int[] array = new int[10];
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = Random.Next(1, 100);
                }
                Console.WriteLine("Array of 10 random integers: " + string.Join(", ", array));
                return array;
            });

            var t2 = t1.ContinueWith(t =>
            {
                int randomInt = Random.Next(1, 100);
                Console.WriteLine($"Random integer to multiply: {randomInt}");
                return t.Result.Select(x => x * randomInt).ToArray();
            });

            var t3 = t2.ContinueWith(t =>
            {
                Array.Sort(t.Result);
                Console.WriteLine("Sorted array: " + string.Join(", ", t.Result));
                return t.Result;
            });

            var t4 = t3.ContinueWith(t =>
            {
                double average = t.Result.Average();
                Console.WriteLine($"Average value: {average}");
                return average;
            });

            t4.Wait();
            Console.ReadLine();
        }
    }
}
