using BlApi;
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
        private IBL Bl { get => BlFactory.GetBl(); }
        private Window PackageView = null;

        BackgroundWorker bw;
        bool Stop = false;

        public DroneViewTab(int id)
        {
            InitializeComponent();
            BLdrone = Bl.GetDroneById(id);
            UpdateChargeButton();
            UpdateDroneNextOp();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Bl.UpdateDroneName(BLdrone.Id, DroneName.Text);
                BLdrone = Bl.GetDroneById(BLdrone.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                {
                    Bl.SendDroneToCharge(BLdrone.Id);
                }
                else if (Charge.Content.ToString() == "release from charge")
                {
                    Bl.ReleaseDrone(BLdrone.Id, DateTime.Now);
                }
            }
            catch (Exception ex)
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
            if (p.TimeToDeliver.HasValue)
            {
                throw new InvalidOperationException("cannot deliver package that has been delivered");
            }

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
                    Bl.AssignPackage(BLdrone.Id);
                else
                {
                    Package p = Bl.GetPackageById(BLdrone.Package.Id);
                    if (p.TimeToPickup.HasValue)
                        Bl.DeliverPackage(BLdrone.Id);
                    else if (p.TimeToPair.HasValue)
                        Bl.PickUpPackage(BLdrone.Id);
                }
                UpdateView();

            }
            catch (Exception ex)
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
                BLdrone = Bl.GetDroneById(BLdrone.Id);
                UpdatePackage();
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
