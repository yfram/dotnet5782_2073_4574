using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome4574();
            Welcome2073();
            Console.ReadKey();
        }

        private static void Welcome4574()
        {
            Console.Write("Enter your name:");
            string name = Console.ReadLine();
            Console.WriteLine($"{name}, welcome to our first(more like 200th) console application");
        }
        
        static partial void Welcome2073()
        {
        }
    }
}
