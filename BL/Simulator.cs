using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BO;
using System.Threading;
using static BlApi.BL;
using BlApi.Exceptions;

namespace BlApi
{
    class Simulator
    {
        BL bl;
        int id;
        Action update;
        Func<bool> stop;

        double speed = 3;
        int msTimer = 1000;

        bool wayToMaitenance = false;

        Drone d;

        public Simulator(BL _bl , int _DroneId, Action _update, Func<bool> _stop)
        {
            bl = _bl;
            id = _DroneId;
            update = _update;
            stop = _stop;

           

            while(!stop())
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
                bl.UpdateDroneName(d.Id, d.Model, d.Battery , d.CurrentLocation);

                Thread.Sleep(msTimer);
            }
        }

        private void DroneEmpty()
        {
            if (wayToMaitenance)
            {
                int? closestId = bl.GetClosetStation(d.CurrentLocation);

                if (closestId is not null)
                {
                    Station s = bl.GetStationById((int)closestId);
                    if (bl.ElecOfDrone(id) * d.Battery <= DistanceTo(d.CurrentLocation, s.LocationOfStation))
                    {
                        bool finish = makeProgress(s.LocationOfStation );
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

        private void DroneBusy()
        {
            Package p = bl.GetPackageById(d.Package.Id);

            if (p.TimeToDeliver is not null)
            {
                throw new Exception();
            }
            else if (p.TimeToPickup is not null)
            {
                bool finish = makeProgress(d.Package.DropOffLocation);
                if (finish)
                {
                    bl.DeliverPackage(d.Id, true);
                }
            }
            else if(p.TimeToPair is not null) // need deliver.
            {

                bool finish = makeProgress(d.Package.PickUpLocation);
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

        //https://stackoverflow.com/questions/7356629/how-to-move-x-distance-in-any-direction-from-geo-coordinates
        // https://www.igismap.com/formula-to-find-bearing-or-heading-angle-between-two-points-latitude-longitude/
        private bool makeProgress(Location destination)
        {
            Location source = new(0,0);
            source.Latitude = d.CurrentLocation.Latitude;
            source.Longitude = d.CurrentLocation.Longitude;

            double x = Math.Cos(destination.Latitude * Math.PI / 180) * Math.Sin((destination.Longitude * Math.PI / 180 - source.Longitude * Math.PI / 180));
            double y = Math.Cos(source.Latitude * Math.PI / 180) * Math.Sin(destination.Latitude * Math.PI / 180)
                - Math.Sin(source.Latitude * Math.PI / 180) *
                Math.Cos(destination.Latitude * Math.PI / 180)
                * Math.Cos(destination.Longitude * Math.PI / 180 - source.Longitude * Math.PI / 180);

            double bearing = Math.Atan2(x, y);

            bearing = ((bearing * 180 / Math.PI + 360) % 360);
            
            double distance = BL.DistanceTo(source, destination);

            double mySpeed = speed;

            if (speed >= distance)
            {
                mySpeed = speed - distance; // force progress == distance at end!
            }

            d.Battery -= mySpeed * (1 / bl.ElecOfDrone(d.Id));

            

            d.CurrentLocation.Latitude = Math.Asin(
                Math.Sin(source.Latitude * Math.PI / 180) * Math.Cos((mySpeed / 6371))
                +
                Math.Cos(source.Latitude * Math.PI / 180) * Math.Sin((mySpeed / 6371)) * Math.Cos(bearing * Math.PI / 180));

            d.CurrentLocation.Latitude = d.CurrentLocation.Latitude * 180 / Math.PI;

            d.CurrentLocation.Longitude = source.Longitude + Math.Atan2(Math.Sin(bearing * Math.PI / 180) * Math.Sin((mySpeed / 6371)) * Math.Cos(source.Latitude * Math.PI / 180)
                ,
                Math.Cos((mySpeed / 6371)) - Math.Sin(source.Latitude * Math.PI / 180) * Math.Sin(d.CurrentLocation.Latitude * Math.PI / 180)) * 180 / Math.PI;

            d.CurrentLocation.Longitude = (d.CurrentLocation.Longitude + 540) % 360 - 180;

            if (distance  > bl.ElecOfDrone(id) * d.Battery)
                throw new Exception();
            if (IsNear(d.CurrentLocation , destination))
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
