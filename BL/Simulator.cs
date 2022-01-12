// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi.Exceptions;
using BO;
using System;
using System.Threading;
using static BL.BL;
using BlApi;
using BL;

namespace Simulator
{
    internal class Simulator
    {
        private BL.BL bl => BlFactory.GetBl() as BL.BL;
        private int id;
        private Action update;
        private Func<bool> stop;
        private double speed = 3;
        private int msTimer = 700;
        private bool wayToMaitenance = false;
        private Drone d;

        private int steps = 0;
        private Location source = null;

        /// <summary>
        /// Starts up the simulator on the drone <paramref name="_DroneId"/>.
        /// When the simulator updates the drone it will call <paramref name="_update"/>.
        /// </summary>
        /// <param name="_bl">The bl instance to work off of</param>
        /// <param name="_DroneId">The drone the simulator runs on</param>
        /// <param name="_update">The action to be invoked when the drone is updated</param>
        /// <param name="_stop"></param>
        /// <exception cref="Exception"></exception>
        public Simulator(int _DroneId, Action _update, Func<bool> _stop)
        {
            id = _DroneId;
            update = _update;
            stop = _stop;
            while (!stop())
            {
                lock (bl)
                {
                    d = bl.GetDroneById(id);
                    switch (d.State)
                    {
                        case DroneState.Empty:
                            DroneEmpty();
                            break;
                        case DroneState.Busy:
                            DroneBusy();
                            break;
                        case DroneState.Maitenance:
                            DroneMaitenance();
                            break;
                    }
                    if (d.Battery < 0)
                    {
                        throw new Exception();
                    }

                    update.Invoke();
                }
                Thread.Sleep(msTimer);
            }
        }

        /// <summary>
        /// The operation to be done if <c>d</c> is empty
        /// </summary>
        private void DroneEmpty()
        {
            if (wayToMaitenance)
            {
                int? closestId = bl.GetClosetStation(d.CurrentLocation);

                if (closestId is not null)
                {
                    Station s = bl.GetStationById((int)closestId);
                    if ((bl.ElecOfDrone(id)) * d.Battery >= LocationUtil.DistanceTo(d.CurrentLocation, s.LocationOfStation))
                    {
                        bool finish = MakeProgress(s.LocationOfStation);
                        if (finish)
                        {
                            wayToMaitenance = false;
                            bl.SendDroneToCharge(id);
                        }
                    }
                }
            }
            else
            {
                if (!StartNewDelivery() && d.Battery < 99) // if cant start a new delivery, and has not enough battery - send to charge
                {
                    wayToMaitenance = true;
                }

            }

        }

        /// <summary>
        /// The operation to be done if <c>d</c> is busy
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        private void DroneBusy()
        {
            Package p = bl.GetPackageById(d.Package.Id);

            if (p.TimeDeliverd is not null)
                throw new Exception();
            else if (p.TimePickedUp is not null)
            {
                bool finish = MakeProgress(d.Package.DropOffLocation);
                if (finish)
                    bl.DeliverPackage(d.Id, true);
            }
            else if (p.TimePaired is not null) // need deliver.
            {

                bool finish = MakeProgress(d.Package.PickUpLocation);
                if (finish)
                    bl.PickUpPackage(id, true);
            }
            else
                throw new InvalidOperationException();
            if (d.Battery < 0)
                throw new InvalidOperationException();
        }

        /// <summary>
        /// The operation to be done if <c>d</c> is in maintenance
        /// </summary>
        private void DroneMaitenance()
        {
            if (d.Battery >= 100)
            {
                d.Battery = Math.Min(100, d.Battery);
                bl.ReleaseDrone(id, DateTime.Now); // don't care time, it's anyway has 100% battery.
            }
            else
            {
                d.Battery += bl.Idal.GetElectricity()[4] * (msTimer / 1000.0); // convert ms to second
                bl.UpdateDrone(d.Id, d.Model, d.Battery, d.CurrentLocation);
            }
        }

        /// <summary>
        /// Assigns a package to <c>d</c> 
        /// </summary>
        /// <returns><code>true</code>If the operation succeeded <code>false</code>If the operation failed</returns>
        private bool StartNewDelivery()
        {
            try
            {
                bl.AssignPackage(id);
                return true;
            }
            catch (BlException)
            {
                return false;
            }
        }

        /// <summary>
        /// Makes a step in the direction of <paramref name="destination"/>
        /// </summary>
        /// <param name="destination">Final target</param>
        /// <returns><code>true</code>If the drone is at <paramref name="destination"/><code>false</code>If the drone is not yet at <paramref name="destination"/></returns>
        /// <exception cref="BlException"></exception>
        private bool MakeProgress(Location destination)
        {
            if (source is null)
                source = d.CurrentLocation;


            double distance = LocationUtil.DistanceTo(d.CurrentLocation, destination);
            if (distance > bl.ElecOfDrone(id) * d.Battery)
            {
                bl.ElecOfDrone(id);
                throw new BlException("Not enough battery to complete operation", id, typeof(Drone));
            }

            double mySpeed = speed;

            if (speed >= distance)
                mySpeed = speed - distance; // force progress == distance at end!


            double bearing = LocationUtil.Bearing(source, destination);

            Location newLoc = LocationUtil.UpdateLocation(source, steps*speed+mySpeed, bearing);
            steps++;

            d.Battery -= LocationUtil.DistanceTo(newLoc , d.CurrentLocation) * (1 / bl.ElecOfDrone(d.Id));
            d.CurrentLocation = newLoc;

            bl.UpdateDrone(d.Id, d.Model, d.Battery, d.CurrentLocation);

            if (LocationUtil.IsNear(newLoc, destination))
            {
                steps = 0;
                source = null;
                return true;
            }
            return false;
        }
    }
}
