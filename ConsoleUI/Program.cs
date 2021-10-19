using System;
using IDAL;
using IDAL.DO;
using DalObject;

namespace ConsoleUI
{
    class Program
    {
        enum Menus { Add = 1, Update, GetById, GetList, Exit }
        static void Main(string[] args)
        {
            String openMsg = "Welcome to John&G Drones administiration system!\n" +
                "for add menu, press 1\n" +
                "for update menu, press 2\n" +
                "for get by id menu, press 3\n" +
                "for get lists menu, press 4\n" +
                "to exit press 5\n";
            String addMenu = "to add a base station, press 1\n" +
                "to add a drone, press 2\n" +
                "to add a new customer, press 3\n" +
                "to add a new packeage for delivering, press 4\n";
            String updateMenu = "to associate a package to a drone, press 1\n" +
                "to pick up a package by a drone, press 2\n" +
                "to deliver a package to a customer, press 3\n" +
                "to send a drone to charge, press 4\n" +
                "to release a drone from a station, press 5\n";
            String getByIdMenu = "to get a station by id press 1\n" +
                "to get a drone by id press 2\n" +
                "to get a customer by id press 3\n" +
                "to get a package by id press 4\n";
            String getListsMenu = "to get the station list press 1\n" +
                "to get the drones list press 2\n" +
                "to get the customers list press 3\n" +
                "to get the packages list press 4\n" +
                "to get a list of all packages wihtout drones press 5\n" +
                "to get a list of all the available stations press 6\n";

            int menu;
            do
            {
                Console.Write(openMsg);
                menu = Console.Read();
            } while (menu <= 5 && menu >= 1);

            switch ((Menus)menu)
            {
                case Menus.Add:
                    AddMenu();
                    break;
                case Menus.Update:
                    UpdateMenu();
                    break;
                case Menus.GetById:
                    GetByIdMenu();
                    break;
                case Menus.GetList:
                    GetListMenu();
                    break;
                case Menus.Exit:
                    Console.WriteLine("GoodBye!");
                    return;

            }
        }

        private static void GetListMenu()
        {
            throw new NotImplementedException();
        }

        private static void GetByIdMenu()
        {
            throw new NotImplementedException();
        }

        private static void UpdateMenu()
        {
            throw new NotImplementedException();
        }

        private static void AddMenu()
        {
            throw new NotImplementedException();
        }
    }
}
