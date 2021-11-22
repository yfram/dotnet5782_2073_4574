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

        private enum UpdateMenue
        {
            DroneName, Customer, Station, ChargeDrone, ReleaseDrone, GivePackageToDrone, PickUpPackge, DeliverPackage,
        }

        static void Main(string[] args)
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
                        PrintUpdateMenue(Bl);
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

        private static void PrintUpdateMenue(IBL.IBL bl)
        {
            string msg = "to update a drone model, press 1\n" +
                "to update a customers information, press 2\n" +
                "to update a stations information, press 3\n" +
                "to put a drone in charging, press 4\n" +
                "to release a drone from charging, press 5\n" +
                "to pair a package to a drone, press 6\n" +
                "to have a drone pick up a package, press 7\n" +
                "to have a drone deliver a package, press 8\n";
            UpdateMenue menue = (UpdateMenue)(GetIntInputInRange(msg, 0, 8, "No such option") - 1);
            switch (menue)
            {
                case UpdateMenue.DroneName:
                    bl.UpdateDroneName(GetIntInput("Enter drone id:"), GetStringInput("Enter new name:"));
                    break;
                case UpdateMenue.Station:
                    bl.UpdateStation(GetIntInput("Enter station ID:"), GetStringInput("Enter new name(enter if no update):"),
                        GetIntInput("Enter new number of charging slots(enter if no update):", true));
                    break;
                case UpdateMenue.Customer:
                    bl.UpdateCustomer(GetIntInput("Enter customer ID number:"),
                        GetStringInput("Enter new name(enter if no update):"),
                        GetStringInput("Enter new phone number(enter if no update):"));
                    break;
                case UpdateMenue.ChargeDrone:
                    bl.SendDroneToCharge(GetIntInput("Enter drone ID:"));
                    break;
                case UpdateMenue.ReleaseDrone:
                    bl.ReleaseDrone(GetIntInput("Enter drone ID:"), GetDoubleInput("Enter amount of time in charging:"));
                    break;
                case UpdateMenue.GivePackageToDrone:
                    bl.AssignPackage(GetIntInput("Enter drone ID:"));
                    break;
                case UpdateMenue.PickUpPackge:
                    bl.PickUpPackage(GetIntInput("Enter drone ID"));
                    break;
                case UpdateMenue.DeliverPackage:
                    bl.DeliverPackage(GetIntInput("Enter drone ID:"));
                    break;
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

        private static int GetIntInput(string print, bool allowEmpty = false)
        {
            Console.WriteLine(print);
            int ret;
            string inp = Console.ReadLine();
            return
                inp == "" && allowEmpty ?
                -1 :
                (int.TryParse(inp, out ret) ? ret : GetIntInput(print));
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
