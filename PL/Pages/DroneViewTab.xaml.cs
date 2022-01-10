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
        private Window PackageView = null;

        BackgroundWorker bw;
        bool Stop = false;

        public DroneViewTab(int id)
        {
            InitializeComponent();
            BLdrone = MainWindow.BL.GetDroneById(id);
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
                BLdrone = MainWindow.BL.GetDroneById(BLdrone.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    MainWindow.BL.SendDroneToCharge(BLdrone.Id);
                }
                else if (Charge.Content.ToString() == "release from charge")
                {
                    MainWindow.BL.ReleaseDrone(BLdrone.Id, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            BLdrone = MainWindow.BL.GetDroneById(BLdrone.Id);
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
            Package p = MainWindow.BL.GetPackageById(BLdrone.Package.Id);
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
                {
                    MainWindow.BL.AssignPackage(BLdrone.Id);
                }
                else
                {
                    Package p = MainWindow.BL.GetPackageById(BLdrone.Package.Id);
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

            BLdrone = MainWindow.BL.GetDroneById(BLdrone.Id);
            UpdateDroneNextOp();
            UpdateChargeButton();
            RefreshParentBl();
        }

        private void UpdateView()
        {
            BLdrone = MainWindow.BL.GetDroneById(BLdrone.Id);
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
                MainWindow.BL.StartSimulator(
                    BLdrone.Id,
                     () => { ((BackgroundWorker)sender).ReportProgress(0); },

                     () => { return Stop; }

                     );
            };
            bw.ProgressChanged += (object? sender, ProgressChangedEventArgs args) =>
            {
                BLdrone = MainWindow.BL.GetDroneById(BLdrone.Id);
                UpdatePackage();
            };

            bw.RunWorkerAsync();

        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            Stop = true;
            UpdateDroneNextOp();
            UpdateChargeButton();

            //bw.CancelAsync();
        }

        private void OpenPackage(object sender, RoutedEventArgs e)
        {
            if (PackageView is null)
            {
                PackageView = new Window
                {
                    Content = new PackageForDroneViewTab(BLdrone.Package),
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
                ((PackageForDroneViewTab)PackageView.Content).update(MainWindow.BL.GetDroneById(BLdrone.Id).Package);
        }
    }
}
