using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleUI
{
    class Program
    {
        enum MenueOptions { Add = 1, Update, GetById, GetList, Exit }


        static void Main(string[] args)
        {
            IDAL d = DalFactory.GetDal();

            string openMsg = "Welcome to John&G Drones administiration system!\n" +
                "for add menue, press 1\n" +
                "for update menue, press 2\n" +
                "for get by id menue, press 3\n" +
                "for get lists menue, press 4\n" +
                "to exit press 5";
            int menue;
            while (true)//you can exit on case MenueOptions.Exit
            {
                do
                {
                    menue = GetIntInput(openMsg);
                    if (menue > 5 || menue < 1)
                        Console.WriteLine("Error! unrecognized op-code");
                } while (menue > 5 || menue < 1);

                switch ((MenueOptions)menue)
                {
                    case MenueOptions.Add:
                        AddMenu(d);
                        break;
                    case MenueOptions.Update:
                        UpdateMenu(d);
                        break;
                    case MenueOptions.GetById:
                        GetByIdMenu(d);
                        break;
                    case MenueOptions.GetList:
                        GetListMenu(d);
                        break;
                    case MenueOptions.Exit:
                        Console.WriteLine("GoodBye!");
                        return;
                }
            }
        }

        private static void GetListMenu(DalApi.IDAL d)
        {
            string getListsMenu = "to get the station list press 1\n" +
                "to get the drones list press 2\n" +
                "to get the customers list press 3\n" +
                "to get the packages list press 4\n" +
                "to get a list of all packages wihtout drones press 5\n" +
                "to get a list of all the available stations press 6";
            int menue;
            do
            {
                menue = GetIntInput(getListsMenu);
                if (menue > 6 || menue < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menue > 6 || menue < 1);
            IEnumerable<object> list = null;
            switch (menue)
            {
                case 1:
                    list = d.GetAllStations().Cast<object>();
                    break;
                case 2:
                    list = d.GetAllDrones().Cast<object>();
                    break;
                case 3:
                    list = d.GetAllCustomers().Cast<object>();
                    break;
                case 4:
                    list = d.GetAllPackages().Cast<object>();
                    break;
                case 5:
                    list = d.GetAllPackagesWhere(p => p.Associated == null).Cast<object>();
                    break;
                case 6:
                    list = d.GetAllStationsWhere(s => s.ChargeSlots > 0).Cast<object>();
                    break;
            }
            foreach (Object obj in list)
            {
                Console.WriteLine(obj.ToString());
            }


        }

        private static void GetByIdMenu(DalApi.IDAL d)
        {
            string getByIdMenu = "to get a station by id press 1\n" +
                "to get a drone by id press 2\n" +
                "to get a customer by id press 3\n" +
                "to get a package by id press 4";

            int menue;
            do
            {
                menue = GetIntInput(getByIdMenu);
                if (menue > 4 || menue < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menue > 4 || menue < 1);

            int id = GetIntInput("Enter ID:");

            string str = "";
            switch (menue)
            {
                case 1:
                    str = d.GetStation(id).ToString();
                    break;
                case 2:
                    str = d.GetDrone(id).ToString();
                    break;
                case 3:
                    str = d.GetCustomer(id).ToString();
                    break;
                case 4:
                    str = d.GetPackage(id).ToString();
                    break;
            }

            Console.WriteLine(str);
        }

        private static void UpdateMenu(DalApi.IDAL d)
        {
            string updateMenu = "to associate a package to a drone, press 1\n" +
                "to pick up a package by a drone, press 2\n" +
                "to deliver a package to a customer, press 3\n" +
                "to send a drone to charge, press 4\n" +
                "to release a drone from a station, press 5";
            int menue;
            do
            {
                menue = GetIntInput(updateMenu);
                if (menue > 5 || menue < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menue > 5 || menue < 1);
            // all need a drone
            List<int> needPackage = new() { 1, 2, 3 };
            List<int> needStation = new() { 4, 5 };
            int droneId = 0, packageId = 0, stationId = 0;
            if (menue != 3)
            {
                droneId = GetIntInput("Enter drone ID:");
            }
            if (needPackage.Contains(menue))
            {
                packageId = GetIntInput("Enter package ID:");
            }
            else if (needStation.Contains(menue))
            {
                stationId = GetIntInput("Enter station ID:");
            }

            switch (menue)
            {
                case 1:
                    d.GivePackageDrone(packageId, droneId);
                    break;
                case 2:
                    d.PickUpPackage(packageId);
                    break;
                case 3:
                    d.DeliverPackage(packageId);
                    break;
                case 4:
                    d.SendDroneToCharge(droneId, stationId);
                    break;
                case 5:
                    d.ReleaseDroneFromCharge(droneId, stationId);
                    break;
                default:
                    break; ;
            }

        }

        private static void AddMenu(DalApi.IDAL d)
        {
            string addMenu = "to add a base station, press 1\n" +
                "to add a drone, press 2\n" +
                "to add a new customer, press 3\n" +
                "to add a new packeage for delivering, press 4";
            int menue;
            do
            {
                menue = GetIntInput(addMenu);
                if (menue > 4 || menue < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menue > 4 || menue < 1);
            switch (menue)
            {
                case 1:
                    d.AddStation(GetIntInput("Enter ID:"), GetStringInput("Enter name:"),
                        GetDoubleInput("Enter drone position(longitude):"), GetDoubleInput("Enter drone position(lattitude):"),
                        GetIntInput("Enter amount of charge slots:"));
                    break;
                case 2:
                    d.AddDrone(GetIntInput("Enter ID:"), GetStringInput("Enter model:"),
                        (WeightGroup)GetEnumInput("Enter weight group(1 for light, 2 for mid, 3 for heavy)", 1, 3));
                    break;
                case 3:
                    d.AddCustomer(GetIntInput("Enter ID"), GetStringInput("Enter name:"), GetStringInput("Enter phone number"),
                        GetDoubleInput("Enter customer position(longitude):"), GetDoubleInput("Enter customer position(lattitude):"));
                    break;
                case 4:
                    d.AddPackage(GetIntInput("Enter sender ID:"),
                        GetIntInput("Enter reciver ID:"), (WeightGroup)GetEnumInput("Enter weight group(1 for light, 2 for mid, 3 for heavy)", 1, 3),
                        (Priority)GetEnumInput("Enter prioraty(1 for low, 2 for mid, 3 for high)", 1, 3),
                        -1, GetDateTimeInput("Enter time to package:"), GetDateTimeInput("Enter time to get drone:"),
                        GetDateTimeInput("Enter time to get package:"), GetDateTimeInput("Enter time to recive:"));
                    break;
            }
        }

        private static int GetEnumInput(string print, int min, int max)
        {
            int ret;
            bool cont;
            do
            {
                Console.WriteLine(print);
                cont = int.TryParse(Console.ReadLine(), out ret);
            } while (ret > max || ret < min || !cont);
            return ret;
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
