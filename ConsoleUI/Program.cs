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
                Console.Write(addMenu);
                menu = Console.Read();
                if (menu > 4 || menu < 1)
                    Console.WriteLine("Error! unrecognized op-code");
            } while (menu <= 4 && menu >= 1);
            Console.WriteLine("Enter ID:");
            int id = Console.Read();
            Console.WriteLine(id);
            string nameOrModel = "";
            if (menu != 4)
            {
                Console.WriteLine($"Enter {(menu == 2 ? "model" : "name")}");
                nameOrModel = Console.ReadLine();
            }
            switch (menu)
            {
                case 1:
                    Console.WriteLine("Enter drone position(longitude):");
                    int longitude = Console.Read();
                    Console.WriteLine("Enter drone position(lattitude):");
                    int lattitude = Console.Read();
                    Console.WriteLine("Enter amount of charge slots:");
                    int chargeSlots = Console.Read();
                    d.AddStation(id, nameOrModel, longitude, lattitude, chargeSlots);
                    break;
                case 2:
                    Console.WriteLine("Enter battery charge:");
                    int charge = Console.Read();
                    int weight = 0;
                    do
                    {
                        Console.WriteLine("Enter weight group(1 for light, 2 for mid, 3 for heavy)");
                        weight = Console.Read();
                    } while (weight > 3 || weight < 1);
                    int state = 0;
                    do
                    {
                        Console.WriteLine("Enter drone state(1 for free, 2 for Maintenance, 3 for Shipping)");
                        state = Console.Read();
                    } while (state > 3 || state < 1);
                    d.AddDrone(id, nameOrModel, charge, (WeightGroup)weight, (DroneStates)state);
                    break;
                case 3:
                    Console.WriteLine("Enter phone number:");
                    string phone = Console.ReadLine();
                    Console.WriteLine("Enter customer position(longitude):");
                    int longitudeC = Console.Read();
                    Console.WriteLine("Enter customer position(lattitude):");
                    int lattitudeC = Console.Read();
                    d.AddCustomer(id, nameOrModel, phone, lattitudeC, longitudeC);
                    break;
                case 4:
                    Console.WriteLine("Enter sender ID:");
                    int Sid = Console.Read();
                    Console.WriteLine("Enter reciver ID:");
                    int Rid = Console.Read();
                    int weightP = 0;
                    do
                    {
                        Console.WriteLine("Enter weight group(1 for light, 2 for mid, 3 for heavy)");
                        weightP = Console.Read();
                    } while (weightP > 3 || weightP < 1);
                    int priority = 0;
                    do
                    {
                        Console.WriteLine("Enter drone state(1 for free, 2 for Maintenance, 3 for Shipping)");
                        priority = Console.Read();
                    } while (priority > 3 || priority < 1);
                    Console.WriteLine("Enter time to package:");
                    int Ttp = Console.Read();
                    Console.WriteLine("Enter time to get drone:");
                    int Tgd = Console.Read();
                    Console.WriteLine("Enter time to get package:");
                    int Tgp = Console.Read();
                    Console.WriteLine("Enter time to recive:");
                    int Ttr = Console.Read();
                    d.AddPackage(id, Sid, Rid, (WeightGroup)weightP, (Priority)priority, -1, Ttp, Tgd, Tgp, Ttr);
                    break;
            }

        }
    }
}
