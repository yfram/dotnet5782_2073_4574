using BlApi;
using System;
using System.Collections.Generic;

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

        private enum SingleViewMenue
        {
            Station, Drone, Customer, Package
        }

        private enum ListViewMenue
        {
            Stations, Drones, Customers, Packages, UnpairedPackages, OpenStations
        }

        static void Main(string[] args)
        {
            BlApi.IBL Bl =BlFactory.GetBl();
            string openMsg = "Welcome to John&G Drones administiration system!";

            string mainMsg = "\nfor add menue, press 1\n" +
                "for update menue, press 2\n" +
                "for get by id menue, press 3\n" +
                "for get lists menue, press 4\n" +
                "to exit press 5";
            Console.Write(openMsg);
            while (true)
            {
                Console.Write(mainMsg);
                MainMenue menue = (MainMenue)(GetIntInputInRange("", 0, 5, "Error! unrecognized op-code") - 1);
                switch (menue)
                {
                    case MainMenue.Add:
                        PrintAddMenue(Bl);
                        break;
                    case MainMenue.Update:
                        PrintUpdateMenue(Bl);
                        break;
                    case MainMenue.ViewById:
                        PrintSingleViewMenue(Bl);
                        break;
                    case MainMenue.ListView:
                        PrintListViewMenue(Bl);
                        break;
                    case MainMenue.Exit:
                        Console.WriteLine("GoodBye!");
                        return;
                }
            }
        }

        private static void PrintListViewMenue(IBL bl)
        {
            string msg = "to see a list of all stations, press 1\n" +
                "to see a list of all drones, press 2\n" +
                "to see a list of all customres, press 3\n" +
                "to see a list of all packages, press 4\n" +
                "to see a list of all unpaired packages, press 5\n" +
                "to see a list of all stations with open slots, press 6";
            try
            {
                ListViewMenue menue = (ListViewMenue)(GetIntInputInRange(msg, 1, 8, "No such option!") - 1);
                switch (menue)
                {
                    case ListViewMenue.Stations:
                        Console.WriteLine(string.Join('\n', bl.DisplayStations()));
                        break;
                    case ListViewMenue.Drones:
                        Console.WriteLine(string.Join('\n', bl.DisplayDrones()));
                        break;
                    case ListViewMenue.Customers:
                        Console.WriteLine(string.Join('\n', bl.DisplayCustomers()));
                        break;
                    case ListViewMenue.Packages:
                        Console.WriteLine(string.Join('\n', bl.DisplayPackages()));
                        break;
                    case ListViewMenue.UnpairedPackages:
                        Console.WriteLine(string.Join('\n', bl.DisplayPackagesWithoutDrone()));
                        break;
                    case ListViewMenue.OpenStations:
                        Console.WriteLine(string.Join('\n', bl.DisplayStationsWithCharges()));
                        break;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        private static void PrintSingleViewMenue(IBL bl)
        {
            string msg = "to view a station by ID, press 1\n" +
                "to view a drone by ID, press 2\n" +
                "to view a customer by ID, press 3\n" +
                "to view  Package by ID, press 4";
            try
            {
                SingleViewMenue menue = (SingleViewMenue)((GetIntInputInRange(msg, 1, 4, "No such option") - 1));
                switch (menue)
                {
                    case SingleViewMenue.Station:
                        Console.WriteLine(bl.DisplayStation(GetIntInput("Enter station ID:")));
                        break;
                    case SingleViewMenue.Drone:
                        Console.WriteLine(bl.DisplayDrone(GetIntInput("Enter drone ID:")));
                        break;
                    case SingleViewMenue.Customer:
                        Console.WriteLine(bl.DisplayCustomer(GetIntInput("Enter customer ID:")));
                        break;
                    case SingleViewMenue.Package:
                        Console.WriteLine(bl.DisplayPackage(GetIntInput("Enter package ID:")));
                        break;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        private static void PrintUpdateMenue(IBL bl)
        {
            string msg = "to update a drone model, press 1\n" +
                "to update a customers information, press 2\n" +
                "to update a stations information, press 3\n" +
                "to put a drone in charging, press 4\n" +
                "to release a drone from charging, press 5\n" +
                "to pair a package to a drone, press 6\n" +
                "to have a drone pick up a package, press 7\n" +
                "to have a drone deliver a package, press 8";
            try
            {
                UpdateMenue menue = (UpdateMenue)(GetIntInputInRange(msg, 1, 8, "No such option") - 1);
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
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        private static void PrintAddMenue(IBL Bl)
        {
            string msg = "to add a base station, press 1\n" +
                "to add a drone, press 2\n" +
                "to add a new customer, press 3\n" +
                "to add a new packeage for delivering, press 4";
            try
            {
                AddMenue menue = (AddMenue)(GetIntInputInRange(msg, 1, 4, "No such option") - 1);
                switch (menue)
                {
                    case AddMenue.Station:
                        Bl.AddStation(new(GetIntInput("Enter ID:"), GetStringInput("Enter name:"),
                            new(GetDoubleInput("Enter station position(longitude):"), GetDoubleInput("Enter station position(lattitude):")),
                            GetIntInput("Enter amount of charge slots:")));
                        break;
                    case AddMenue.Drone:
                        Bl.AddDrone(new(GetIntInput("Enter ID:"), GetStringInput("Enter model:"),
                            (BO.WeightGroup)GetIntInputInRange("Enter weight group(1 for light, 2 for mid, 3 for heavy)", 1, 3),
                            new Random().Next(20, 40) / 100, BO.DroneState.Maitenance, new(),
                            GetStationLocation(GetIntInput("Enter starting station id:"), Bl)));

                        break;
                    case AddMenue.Customer:
                        Bl.AddCustomer(new(GetIntInput("Enter ID number:"), GetStringInput("Enter name:"),
                            GetStringInput("Enter phone number:"),
                            new(GetDoubleInput("Enter custumer position(longitude):"), GetDoubleInput("Enter customer position(lattitude):")),
                            new List<BO.PackageForCustomer>(), new List<BO.PackageForCustomer>()));
                        break;
                    case AddMenue.Package:
                        Bl.AddPackage(new(new Random().Next(), new(Bl.DisplayCustomer(GetIntInput("Enter sender ID:"))),
                            new(Bl.DisplayCustomer(GetIntInput("Enter reciver ID:"))),
                            (BO.WeightGroup)(GetIntInputInRange("Enter weight group(1 for light, 2 for mid, 3 for heavy):", 1, 3) + 1),
                            (BO.PriorityGroup)(GetIntInputInRange("Enter prioriyt group(1 for low, 2 for mid, 3 for high):", 1, 3) + 1),
                            null, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue));
                        break;
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static BO.Location GetStationLocation(int id, IBL bL)
        {
            
            /*if (bL is not BL Bl)
                throw new("Unreachable!");*/
            try
            {

                return bL.DisplayStation(id).LocationOfStation;
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

        private static int GetIntInputInRange(string print, int min, int max, string errorMsg = null)
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