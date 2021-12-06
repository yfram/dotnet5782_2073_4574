﻿using System;
using System.Windows.Controls;
using IBL.BO;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DroneViewTab.xaml
    /// </summary>
    public partial class DroneViewTab : UserControl
    {
        private Drone BLdrone;
        public DroneViewTab(Drone d)
        {
            BLdrone = d;
            InitializeComponent();
            DroneId.Text += d.Id.ToString();
            DroneName.Text = d.Model;
            DroneLocation.Text += d.CurrentLocation.ToString();
            DroneBattery.Text += System.Math.Round(d.Battery,2).ToString();
            DroneState.Text += d.State.ToString();
            DroneWeight.Text += d.Weight.ToString();

            UpdateChargeButton();
            UpdateDroneNextOp();

        }

        private void UpdateButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.BL.UpdateDroneName(BLdrone.Id,DroneName.Text);
            BLdrone = MainWindow.BL.DisplayDrone(BLdrone.Id);

            DroneName.Text = BLdrone.Model.ToString();
            UpdateState.Visibility = System.Windows.Visibility.Visible;
            
        }

        private void Charge_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if(Charge.Content.ToString() == "send to charge")
            {
                MainWindow.BL.SendDroneToCharge(BLdrone.Id);
                BLdrone = MainWindow.BL.DisplayDrone(BLdrone.Id);
                UpdateChargeButton();

            }
            else if(Charge.Content.ToString() == "release from charge")
            {
                MainWindow.BL.ReleaseDrone(BLdrone.Id, double.Parse(ChargeTime.Text));
                BLdrone = MainWindow.BL.DisplayDrone(BLdrone.Id);
                UpdateChargeButton();
            }
        }

        private void UpdateChargeButton()
        {
            if (BLdrone.State == IBL.BO.DroneState.Empty)
            {
                Charge.IsEnabled = true;
                Charge.Content = "send to charge";
                
            }
            else if (BLdrone.State == IBL.BO.DroneState.Maitenance)
            {
                Charge.IsEnabled = true;
                Charge.Content = "release from charge";
            }
            else
            {
                Charge.IsEnabled = false;
                Charge.Content = "cannot charge now";
            }
        }
        private void UpdateDroneNextOp()
        {
            if(BLdrone.State == IBL.BO.DroneState.Maitenance)
            {
                DroneNextOp.Visibility = System.Windows.Visibility.Hidden;
                return;
            }
            if(BLdrone.State == IBL.BO.DroneState.Empty)
            {
                DroneNextOp.Content = "pair a package";
                return;
            }
            Package p= MainWindow.BL.DisplayPackage(BLdrone.Package.Id);
            if (p.TimeToDeliver.HasValue)
                throw new Exception("cannot deliver package that delivered");
            if(p.TimeToPickup.HasValue)
            {
                DroneNextOp.Content = "deliver the package";
            }
            else if(p.TimeToPair.HasValue)
            {
                DroneNextOp.Content = "Pick up the package";
            }
        }

        private void DroneNextOp_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}
