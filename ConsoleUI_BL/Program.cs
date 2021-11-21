using System;
using IBL;

namespace ConsoleUI_BL
{
    internal class Program
    {
        IBL.IBL @Bl = new BL();

        private enum MainMenue
        {
            Add, Update, ViewById, ListView, Exit
        }

        static void Main(string[] args)
        {
            string openMsg = "Welcome to John&G Drones administiration system!\n" +
                "for add menue, press 1\n" +
                "for update menue, press 2\n" +
                "for get by id menue, press 3\n" +
                "for get lists menue, press 4\n" +
                "to exit press 5";
            while (true)
            {
                MainMenue menue = (MainMenue)(GetIntInputInRange(openMsg, 0, 5, "Error! unrecognized op-code") - 1);
                switch (menue)
                {
                    case MainMenue.Add:
                        break;
                    case MainMenue.Update:
                        break;
                    case MainMenue.ViewById:
                        break;
                    case MainMenue.ListView:
                        break;
                    case MainMenue.Exit:
                        Console.WriteLine("GoodBye!");
                        return;
                }
            }
        }

        private static int GetIntInput(string print)
        {
            Console.WriteLine(print);
            int ret;
            return int.TryParse(Console.ReadLine(), out ret) ? ret : GetIntInput(print);
        }

        private static int GetIntInputInRange(string print, int min, int max, string? errorMsg)
        {
            int res;
            do
            {
                res = GetIntInput(print);
                if (res < min || res > max)
                    Console.Write(errorMsg is null ? "" : $"{errorMsg}\n");
            } while (res < min || res > max);
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
