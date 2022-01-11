using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Threading;
using static BlApi.BL;
using BlApi.Exceptions;
using BLApi;

namespace BlApi
{
    class Simulator
    {
        BL bl;// why not use the singolton?
        int id;
        Action update;
        Func<bool> stop;//why?
        double speed = 3;
        int msTimer = 1000;
        bool wayToMaitenance = false;
        Drone d;

        /// <summary>
        /// Starts up the simulator on the drone <paramref name="_DroneId"/>.
        /// When the simulator updates the drone it will call <paramref name="_update"/>.
        /// </summary>
        /// <param name="_bl">The bl instance to work off of</param>
        /// <param name="_DroneId">The drone the simulator runs on</param>
        /// <param name="_update">The action to be invoked when the drone is updated</param>
        /// <param name="_stop"></param>
        /// <exception cref="Exception"></exception>
        public Simulator(BL _bl, int _DroneId, Action _update, Func<bool> _stop)
        {
            bl = _bl;
            id = _DroneId;
            update = _update;
            stop = _stop;
            while (!stop())
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
                    throw new Exception();
                update.Invoke();
                bl.UpdateDroneName(d.Id, d.Model, d.Battery, d.CurrentLocation);
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
                    if (bl.ElecOfDrone(id) * d.Battery <= LocationUtil.DistanceTo(d.CurrentLocation, s.LocationOfStation))
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
                if (!StartNewDelivery() && d.Battery != 100) // if cant start a new delivery, and has not enouth battery - snd to charge
                {
                    bl.SendDroneToCharge(id);
                    wayToMaitenance = true;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="Exception">Thrown </exception>
        private void DroneBusy()
        {
            Package p = bl.GetPackageById(d.Package.Id);

            if (p.TimeDeliverd is not null)
            {
                throw new Exception();
            }
            else if (p.TimePickedUp is not null)
            {
                bool finish = MakeProgress(d.Package.DropOffLocation);
                if (finish)
                {
                    bl.DeliverPackage(d.Id, true);
                }
            }
            else if (p.TimePaired is not null) // need deliver.
            {

                bool finish = MakeProgress(d.Package.PickUpLocation);
                if (finish)
                {
                    bl.PickUpPackage(id, true);
                }
            }
            else
            {
                throw new Exception();
            }

            if (d.Battery < 0)
                throw new Exception();

        }

        private void DroneMaitenance()
        {
            if (d.Battery >= 100)
            {
                d.Battery = Math.Min(100, d.Battery);
                bl.ReleaseDrone(id, DateTime.Now); // don't care time, it's enyway has 100% battery.
            }
            else
            {
                d.Battery += bl.Idal.GetElectricity()[4] * (msTimer / 1000.0); // convert ms to second
            }
        }

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


        private bool MakeProgress(Location destination)
        {
            double distance = LocationUtil.DistanceTo(d.CurrentLocation, destination);
            if (distance > bl.ElecOfDrone(id) * d.Battery)
                throw new Exception();

            double mySpeed = speed;

            if (speed >= distance)
                mySpeed = speed - distance; // force progress == distance at end!

            d.Battery -= mySpeed * (1 / bl.ElecOfDrone(d.Id));

            double bearing = LocationUtil.Bearing(d.CurrentLocation, destination);

            Location newLoc = LocationUtil.UpdateLocation(new Location(d.CurrentLocation.Longitude, d.CurrentLocation.Latitude), mySpeed, bearing);
            d.CurrentLocation = newLoc;
            if (LocationUtil.IsNear(newLoc, destination))
            {
                return true;
            }

            return false;
        }

        private bool IsNear(Location a, Location b)
        {
            return DistanceTo(a, b) < 2;
        }
    }
}
