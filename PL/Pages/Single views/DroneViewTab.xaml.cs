// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using BlApi.Exceptions;
using BO;
using System;
using System.ComponentModel;
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
        private static IBL Bl => BlFactory.GetBl();
        private Window PackageView = null;
        private BackgroundWorker bw;
        private bool Stop = false;

        public DroneViewTab(int _id)
        {
            InitializeComponent();
            BLdrone = Bl.GetDroneById(_id);
            UpdateChargeButton();
            UpdateDroneNextOp();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

            //error cannot be thrown here, because the drone definitely exists 
            Bl.UpdateDrone(BLdrone.Id, DroneName.Text);
            BLdrone = Bl.GetDroneById(BLdrone.Id);
            RefreshParentBl();
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((DronesViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void RefreshParentBl()
        {
            ((DronesViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).RefreshBl();
        }

        private void Charge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Charge.Content.ToString() == "send to charge")
                    Bl.SendDroneToCharge(BLdrone.Id);
                else if (Charge.Content.ToString() == "release from charge")
                    Bl.ReleaseDrone(BLdrone.Id, DateTime.Now);
            }
            catch (BlException)
            {
                MessageBox.Show($"No available stations can charge drone {BLdrone.Id}", "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DroneStateException ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            BLdrone = Bl.GetDroneById(BLdrone.Id);
            UpdateDroneNextOp();
            UpdateChargeButton();

            RefreshParentBl();
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
            Package p = Bl.GetPackageById(BLdrone.Package.Id);
            if (p.TimeDeliverd.HasValue)
                throw new InvalidOperationException("cannot deliver package that has been delivered");

            if (p.TimePickedUp.HasValue)
                DroneNextOp.Content = "deliver the package";
            else if (p.TimePaired.HasValue)
                DroneNextOp.Content = "Pick up the package";
        }

        private void DroneNextOp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BLdrone.State == BO.DroneState.Empty)
                    Bl.AssignPackage(BLdrone.Id);
                else
                {
                    Package p = Bl.GetPackageById(BLdrone.Package.Id);
                    if (p.TimePickedUp.HasValue)
                        Bl.DeliverPackage(BLdrone.Id);
                    else if (p.TimePaired.HasValue)
                        Bl.PickUpPackage(BLdrone.Id);
                }
                UpdateView();

            }
            catch (BlException ex)
            {
                MessageBox.Show($"The package {ex.ObjectId} cannot be delivered, as it is not associated or picked up",
                    "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (ObjectDoesntExistException ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (DroneStateException ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            BLdrone = Bl.GetDroneById(BLdrone.Id);
            UpdateDroneNextOp();
            UpdateChargeButton();
            RefreshParentBl();
        }

        private void UpdateView()
        {
            BLdrone = Bl.GetDroneById(BLdrone.Id);
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            bw = new();

            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            Stop = false;
            //bw.RunWorkerCompleted = 
            bw.DoWork += (object? sender, DoWorkEventArgs args) =>
            {
                Bl.StartSimulator(
                    BLdrone.Id,
                     () => { ((BackgroundWorker)sender).ReportProgress(0); },

                     () => { return Stop; }

                     );
            };
            bw.ProgressChanged += (object? sender, ProgressChangedEventArgs args) =>
            {
                UpdateView();
                UpdatePackage();
                RefreshParentBl();
            };

            bw.RunWorkerAsync();

        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            Stop = true;
            UpdateDroneNextOp();
            UpdateChargeButton();
        }

        private void OpenPackage(object sender, RoutedEventArgs e)
        {
            if (PackageView is null)
            {
                PackageView = new Window
                {
                    Content = new PackageForDroneViewTab(BLdrone.Package.Id),
                    Title = $"package {BLdrone.Package.Id}",
                    SizeToContent = SizeToContent.WidthAndHeight,
                    ResizeMode = ResizeMode.CanResize
                };
                PackageView.Closed += (sender, args) => this.PackageView = null;
                PackageView.Show();
            }
        }

        private void UpdatePackage()
        {
            if (PackageView is not null)
                ((PackageForDroneViewTab)PackageView.Content).update(Bl.GetDroneById(BLdrone.Id).Package);
        }
    }
}
