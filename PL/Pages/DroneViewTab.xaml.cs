using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DroneViewTab.xaml
    /// </summary>
    public partial class DroneViewTab : UserControl
    {
        private Drone BLdrone { get => Resources["drone"] as Drone; set => Resources["drone"] = value; }
        public DroneViewTab(Drone d)
        {
            InitializeComponent();
            BLdrone = d;

            UpdateChargeButton();
            UpdateDroneNextOp();

            UpdateButton.GotFocus += UpdateButton_Click;
            DroneNextOp.GotFocus += DroneNextOp_Click;
            Charge.GotFocus += Charge_Click;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.BL.UpdateDroneName(BLdrone.Id, DroneName.Text);
                BLdrone = MainWindow.BL.DisplayDrone(BLdrone.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focusable = true;
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focus();
        }

        private void Charge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Charge.Content.ToString() == "send to charge")
                {
                    MainWindow.BL.SendDroneToCharge(BLdrone.Id);
                }
                else if (Charge.Content.ToString() == "release from charge")
                {
                    MainWindow.BL.ReleaseDrone(BLdrone.Id, double.Parse(ChargeTime.Text));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            BLdrone = MainWindow.BL.DisplayDrone(BLdrone.Id);
            UpdateDroneNextOp();
            UpdateChargeButton();

            Exit();
        }

        private void UpdateChargeButton()
        {
            if (BLdrone.State == BO.DroneState.Empty)
            {
                Charge.IsEnabled = true;
                Charge.Content = "send to charge";

            }
            else if (BLdrone.State == BO.DroneState.Maitenance)
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
            if (BLdrone.State == BO.DroneState.Maitenance)
            {
                DroneNextOp.Visibility = Visibility.Hidden;
                return;
            }
            DroneNextOp.Visibility = Visibility.Visible;
            if (BLdrone.State == BO.DroneState.Empty)
            {
                DroneNextOp.Content = "pair a package";
                return;
            }
            Package p = MainWindow.BL.DisplayPackage(BLdrone.Package.Id);
            if (p.TimeToDeliver.HasValue)
                throw new InvalidOperationException("cannot deliver package that has been delivered");
            if (p.TimeToPickup.HasValue)
            {
                DroneNextOp.Content = "deliver the package";
            }
            else if (p.TimeToPair.HasValue)
            {
                DroneNextOp.Content = "Pick up the package";
            }
        }

        private void DroneNextOp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BLdrone.State == BO.DroneState.Empty)
                {
                    MainWindow.BL.AssignPackage(BLdrone.Id);
                }
                else
                {
                    Package p = MainWindow.BL.DisplayPackage(BLdrone.Package.Id);
                    if (p.TimeToPickup.HasValue)
                    {
                        MainWindow.BL.DeliverPackage(BLdrone.Id);
                    }
                    else if (p.TimeToPair.HasValue)
                    {
                        MainWindow.BL.PickUpPackage(BLdrone.Id);
                    }
                }
                UpdateView();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            BLdrone = MainWindow.BL.DisplayDrone(BLdrone.Id);
            UpdateDroneNextOp();
            UpdateChargeButton();
            Exit();
        }

        private void UpdateView()
        {
            BLdrone = MainWindow.BL.DisplayDrone(BLdrone.Id);
        }
    }
}
