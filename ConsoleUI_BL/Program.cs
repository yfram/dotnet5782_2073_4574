using System;
using IBL;

namespace ConsoleUI_BL
{
    internal class Program
    {

        private enum MainMenue
        {
            Add, Update, ViewById, ListView, Exit
        }

        private enum AddMenue
        {
            Station, Drone, Customer, Package
        }

        internal static void Main(string[] args)
        {
            IBL.IBL Bl = new BL();
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
                        PrintAddMenue(Bl);
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

        private static void PrintAddMenue(IBL.IBL Bl)
        {
            string msg = "to add a base station, press 1\n" +
                "to add a drone, press 2\n" +
                "to add a new customer, press 3\n" +
                "to add a new packeage for delivering, press 4";
            AddMenue menue = (AddMenue)(GetIntInputInRange(msg, 0, 4, "No such option") - 1);
            switch (menue)
            {
                case AddMenue.Station:
                    Bl.AddStation(new(GetIntInput("Enter ID:"), GetStringInput("Enter name:"),
                        new(GetDoubleInput("Enter drone position(longitude):"), GetDoubleInput("Enter drone position(lattitude):")),
                        GetIntInput("Enter amount of charge slots:")));
                    break;
                case AddMenue.Drone:
                    try
                    {
                        Bl.AddDrone(new(GetIntInput("Enter ID:"), GetStringInput("Enter model:"),
                            (IBL.BO.WeightGroup)GetIntInputInRange("Enter weight group(1 for light, 2 for mid, 3 for heavy)", 1, 3),
                            new Random().Next(20, 40) / 100, IBL.BO.DroneState.Maitenance, new(),
                            GetStationLocation(GetIntInput("Enter starting station id:"), Bl)));
                    }
                    catch (ArgumentNullException)
                    {
                        //message allready written
                        return;
                    }
                    break;
                case AddMenue.Customer:
                    Bl.AddCustomer(new(GetIntInput("Enter ID number:"), GetStringInput("Enter name:"),
                        GetStringInput("Enter phone number:"),
                        new(GetDoubleInput("Enter drone position(longitude):"), GetDoubleInput("Enter drone position(lattitude):")),
                        new(), new()));
                    break;
                case AddMenue.Package:
                    Bl.AddPackage(new(new Random().Next(), new(Bl.DisplayCustomer(GetIntInput("Enter sender ID:"))),
                        new(Bl.DisplayCustomer(GetIntInput("Enter reciver ID:"))),
                        (IBL.BO.WeightGroup)(GetIntInputInRange("Enter weight group(1 for light, 2 for mid, 3 for heavy):", 1, 3) + 1),
                        (IBL.BO.PriorityGroup)(GetIntInputInRange("Enter prioriyt group(1 for low, 2 for mid, 3 for high):", 1, 3) + 1),
                        null, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue));
                    break;
            }
        }

        private static IBL.BO.Location? GetStationLocation(int id, IBL.IBL bL)
        {
            if (bL is not BL Bl)
                throw new("Unreachable!");
            try
            {

                return Bl.DisplayStation(id).LocationOfStation;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static int GetIntInput(string print)
        {
            Console.WriteLine(print);
            int ret;
            return int.TryParse(Console.ReadLine(), out ret) ? ret : GetIntInput(print);
        }

        private static void GetIntInput(string print, out int ret)
        {
            Console.WriteLine(print);
            ret = int.TryParse(Console.ReadLine(), out ret) ? ret : GetIntInput(print);
        }

        private static int GetIntInputInRange(string print, int min, int max, string? errorMsg = null)
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
