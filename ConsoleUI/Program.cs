using System;
using IDAL;
using IDAL.DO;
using DalObject;
using System.Collections.Generic;


namespace ConsoleUI
{
    class Program
    {
        enum Menus { Add = 1, Update, GetById, GetList, Exit }


        static void Main(string[] args)
        {
            DalObject.DalObject d = new DalObject.DalObject();

            String openMsg = "Welcome to John&G Drones administiration system!\n" +
                "for add menu, press 1\n" +
                "for update menu, press 2\n" +
                "for get by id menu, press 3\n" +
                "for get lists menu, press 4\n" +
                "to exit press 5\n";

            int menu;
            do
            {
                Console.Write(openMsg);
                menu = Console.Read();
                if (menu > 5 || menu < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menu <= 5 && menu >= 1);

            switch ((Menus)menu)
            {
                case Menus.Add:
                    AddMenu(d);
                    break;
                case Menus.Update:
                    UpdateMenu(d);
                    break;
                case Menus.GetById:
                    GetByIdMenu(d);
                    break;
                case Menus.GetList:
                    GetListMenu(d);
                    break;
                case Menus.Exit:
                    Console.WriteLine("GoodBye!");
                    return;
            }
        }

        private static void GetListMenu(DalObject.DalObject d)
        {
            String getListsMenu = "to get the station list press 1\n" +
                "to get the drones list press 2\n" +
                "to get the customers list press 3\n" +
                "to get the packages list press 4\n" +
                "to get a list of all packages wihtout drones press 5\n" +
                "to get a list of all the available stations press 6\n";

            Console.Write(getListsMenu);

            int menu;
            do
            {
                Console.Write(getListsMenu);
                menu = Console.Read();
                if (menu > 6 || menu < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menu <= 6 && menu >= 1);
            String str = "";
            switch (menu)
            {
                case 1:
                    str = d.GetAllStationsString();
                    break;
                case 2:
                    str = d.GetAllDronesString();
                    break;
                case 3:
                    str = d.GetAllCustomersString();
                    break;
                case 4:
                    str = d.GetAllPackagesString();
                    break;
                case 5:
                    str = d.GetAllUndronedPackagesString();
                    break;
                case 6:
                    str = d.GetAllAvailableStationsString();
                    break;
            }

            Console.WriteLine(str);

        }

        private static void GetByIdMenu(DalObject.DalObject d)
        {
            String getByIdMenu = "to get a station by id press 1\n" +
                "to get a drone by id press 2\n" +
                "to get a customer by id press 3\n" +
                "to get a package by id press 4\n";

            Console.Write(getByIdMenu);

            int menu;
            do
            {
                Console.Write(getByIdMenu);
                menu = Console.Read();
                if (menu > 4 || menu < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menu <= 4 && menu >= 1);

            Console.Write("enter ID:\n");
            int id = Console.Read();

            String str = "";
            switch (menu)
            {
                case 1:
                    str = d.GetStationString(id);
                    break;
                case 2:
                    str = d.GetDroneString(id);
                    break;
                case 3:
                    str = d.GetCustomerString(id);
                    break;
                case 4:
                    str = d.GetPackageString(id);
                    break;
            }

            Console.WriteLine(str);
        }

        private static void UpdateMenu(DalObject.DalObject d)
        {
            String updateMenu = "to associate a package to a drone, press 1\n" +
                "to pick up a package by a drone, press 2\n" +
                "to deliver a package to a customer, press 3\n" +
                "to send a drone to charge, press 4\n" +
                "to release a drone from a station, press 5\n";

            Console.Write(updateMenu);

            int menu;
            do
            {
                Console.Write(updateMenu);
                menu = Console.Read();
                if (menu > 5 || menu < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menu <= 5 && menu >= 1);
            // all need a drone
            List<int> needPackage = new() { 1, 2, 3 };
            List<int> needStation = new() { 4, 5 };
            int droneId = 0, packageId = 0, stationId = 0;
            if (menu != 3)
            {
                Console.WriteLine("enter drone Id:");
                droneId = Console.Read();
            }
            if (needPackage.Contains(menu))
            {
                Console.WriteLine("enter package id");
                packageId = Console.Read();
            }
            else if (needStation.Contains(menu))
            {
                Console.WriteLine("enter station id");
                stationId = Console.Read();
            }

            switch (menu)
            {
                case 1:
                    d.GivePackageDrone(packageId, droneId);
                    break;
                case 2:
                    d.PickUpPackage(packageId, droneId);
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
                    break;
            }

        }

        private static void AddMenu(DalObject.DalObject d)
        {
            String addMenu = "to add a base station, press 1\n" +
                "to add a drone, press 2\n" +
                "to add a new customer, press 3\n" +
                "to add a new packeage for delivering, press 4\n";

            Console.Write(addMenu);
            int menu;
            do
            {
                menu = GetIntInput(addMenu);
                if (menu > 4 || menu < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menu <= 4 && menu >= 1);
            switch (menu)
            {
                case 1:
                    d.AddStation(GetIntInput("Enter ID:"), GetStringInput("Enter name:"),
                        GetDoubleInput("Enter drone position(longitude):"), GetDoubleInput("Enter drone position(lattitude):"),
                        GetIntInput("Enter amount of charge slots:"));
                    break;
                case 2:
                    d.AddDrone(GetIntInput("Enter ID:"), GetStringInput("Enter model:"),
                        GetIntInput("Enter battery charge:"), (WeightGroup)GetEnumInput("Enter weight group(1 for light, 2 for mid, 3 for heavy)", 1, 3),
                        (DroneStates)GetEnumInput("Enter drone state(1 for free, 2 for Maintenance, 3 for Shipping)", 1, 3));
                    break;
                case 3:
                    d.AddCustomer(GetIntInput("Enter ID"), GetStringInput("Enter name:"), GetStringInput("Enter phone number"),
                        GetDoubleInput("Enter customer position(longitude):"), GetDoubleInput("Enter customer position(lattitude):"));
                    break;
                case 4:
                    d.AddPackage(GetIntInput("Enter ID:"), GetIntInput("Enter sender ID:"),
                        GetIntInput("Enter reciver ID:"), (WeightGroup)GetEnumInput("Enter weight group(1 for light, 2 for mid, 3 for heavy)", 1, 3),
                        (Priority)GetEnumInput("Enter prioraty(1 for low, 2 for mid, 3 for high)", 1, 3),
                        -1, GetIntInput("Enter time to package:"), GetIntInput("Enter time to get drone:"),
                        GetIntInput("Enter time to get package:"), GetIntInput("Enter time to recive:"));
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
            } while (ret > 3 || ret < 1 || !cont);
            return ret;
        }

        private static int GetIntInput(string print)
        {
            Console.WriteLine(print);
            int ret;
            return int.TryParse(Console.ReadLine(), out ret) ? ret : GetIntInput(print);
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
