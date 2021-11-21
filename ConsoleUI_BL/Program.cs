using System;
using IBL;

namespace ConsoleUI_BL
{
    class Program
    {
        IBL.IBL @Bl = new BL();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private static int GetIntInput(string print)
        {
            Console.WriteLine(print);
            int ret;
            return int.TryParse(Console.ReadLine(), out ret) ? ret : GetIntInput(print);
        }
        private static int GetIntInputInRange(string print, int min, int max)
        {
            int res;
            do
            {
                res = GetIntInput(print);
            } while (res < min || res >= max);

            return res;

        }

        private static DateTime GetDateTimeInput(string print)
        {
            Console.WriteLine(print);
            int year = GetIntInput("enter a year");
            int month = GetIntInputInRange("enter a month", 1, 13);
            int day = GetIntInputInRange("enter a day", 1, 32);
            int hour = GetIntInputInRange("enter a hour", 1, 25);
            int minute = GetIntInputInRange("enter a minute", 1, 60);
            int second = GetIntInputInRange("enter a second", 1, 60);

            return new DateTime(year, month, day, hour, minute, second);

        }

        private static double GetDoubleInput(string print)
        {
            Console.WriteLine(print);
            double ret;
            return double.TryParse(Console.ReadLine(), out ret) ? ret : GetDoubleInput(print);
        }

        private static string GetStringInput(string print)
        {
            Console.WriteLine(print);
            return Console.ReadLine();
        }
    }
}
