﻿using IDAL.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IDal
    {
        void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots);
        void AddDrone(int id, string model, WeightGroup weight);
        void AddCustomer(int id, string name, string phone, double lattitude, double longitude);
        void AddPackage(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int droneId,
            DateTime Created, DateTime Associated, DateTime PickUp, DateTime Delivered);
        void GivePackageDrone(int packageId, int droneId);
        void PickUpPackage(int packageId, int droneID);

        void DeliverPackage(int packageId);
        void SendDroneToCharge(int droneId, int stationId);
        void ReleaseDroneFromCharge(int droneId, int stationId);

        Station GetStation(int id);
        Drone GetDrone(int id);
        Customer GetCustomer(int id);
        Package GetPackage(int id);


        void DeleteStation(int id);
        void DeletePackage(int id);
        void DeleteCustomer(int id);
        void DeleteDrone(int id);

        void UpdateStation(Station s);
        void UpdatePackage(Package p);
        void UpdateCustomer(Customer c);
        void UpdateDrone(Drone d);

        IEnumerable<Station> GetAllStations();
        IEnumerable<Drone> GetAllDrones();
        IEnumerable<Customer> GetAllCustomers();
        IEnumerable<Package> GetAllPackages();
        IEnumerable<Package> GetAllUndronedPackages();
        IEnumerable<Station> GetAllAvailableStations();

        IEnumerable<double> GetElectricity(int droneId);
    }
}
