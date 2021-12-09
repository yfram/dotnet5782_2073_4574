using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using IBL.BO;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DroneViewTab.xaml
    /// </summary>
    public partial class DroneViewTab : UserControl
    {
        private Drone BLdrone { get => (Drone)Resources["drone"]; set => Resources["drone"] = value; }
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

        private void DroneNextOp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BLdrone.State == IBL.BO.DroneState.Empty)
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
            Charge.IsEnabled = BLdrone.State switch
            {
                IBL.BO.DroneState.Empty => (Charge.Content = "send to charge").Equals("send to charge"),
                IBL.BO.DroneState.Maitenance => (Charge.Content = "release from charge").Equals("release from charge"),
                IBL.BO.DroneState.Busy => (Charge.Content = "cannot charge now").Equals(""),
            };
        }

        private void UpdateDroneNextOp()
        {
            if (BLdrone.State == IBL.BO.DroneState.Maitenance)
            {
                DroneNextOp.Visibility = Visibility.Hidden;
                return;
            }
            if (BLdrone.State == IBL.BO.DroneState.Empty)
            {
                DroneNextOp.Content = "pair a package";
                return;
            }
            Package p = MainWindow.BL.DisplayPackage(BLdrone.Package.Id);
            if (p.TimeToDeliver.HasValue)
                throw new Exception("cannot deliver package that delivered");
            if (p.TimeToPickup.HasValue)
            {
                DroneNextOp.Content = "deliver the package";
            }
            else if (p.TimeToPair.HasValue)
            {
                DroneNextOp.Content = "Pick up the package";
            }
        }

        private void Exit(object? sender = null, RoutedEventArgs? e = null)
        {
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focusable = true;
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focus();
        }
    }
}
