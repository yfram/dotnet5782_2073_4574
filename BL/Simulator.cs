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

        double speed = 7;
        int msTimer = 500;
        double progress = 0;

        Drone d;

        public Simulator(BL _bl , int _DroneId, Action _update, Func<bool> _stop)
        {
            bl = _bl;
            id = _DroneId;
            update = _update;
            stop = _stop;

           

            while(!stop())
            {
                d = bl.DisplayDrone(id);

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
                bl.UpdateDroneName(d.Id, d.Model, d.Battery);

                Thread.Sleep(msTimer);
            }
        }

        private void DroneEmpty()
        {
            if( !StartNewDelivery() && d.Battery != 100) // if cant start a new delivery, and has not enouth battery - snd to charge
            {
                bl.SendDroneToCharge(id);
            }
            
        }

        private void DroneBusy()
        {
            Package p = bl.DisplayPackage(d.Package.Id);

            d.Battery -= speed * (1/bl.ElecOfDrone(d.Id));

            progress += speed;
            double distance = -1;
            if (p.TimeToDeliver is not null)
            {
                throw new Exception();
            }
            else if (p.TimeToPickup is not null)
            {
                distance = BL.DistanceTo(d.Package.DropOffLocation, d.Package.PickUpLocation);
                if (progress >= distance)
                {
                    bl.DeliverPackage(d.Id, true);
                    d.Battery += (progress - distance) * (1 / bl.ElecOfDrone(d.Id));
                    progress = 0;
                }
            }
            else if(p.TimeToPair is not null) // need deliver.
            {

                distance = BL.DistanceTo(d.CurrentLocation, d.Package.PickUpLocation);
                if (progress >= distance)
                {
                    bl.PickUpPackage(d.Id, true);
                    d.Battery += (progress - distance) * (1 / bl.ElecOfDrone(d.Id));
                    progress = 0;
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
    }
}
