// File PackageViewTab.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using BO;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for PackageViewTab.xaml
    /// </summary>
    public partial class PackageViewTab : UserControl
    {
        private Package BLpackage { get => Resources["package"] as Package; set => Resources["package"] = value; }
        private static IBL Bl => BlFactory.GetBl();

        public PackageViewTab(int id)
        {
            InitializeComponent();
            BLpackage = Bl.GetPackageById(id);
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((PackagesViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void OpenDrone(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var d = button.Tag as DroneForPackage;

            new Window
            {
                Content = new DroneForPackageWindow(d),
                Title = $"drone {d.Id}",
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.CanResize

            }.Show();
        }

        private void OpenCustomer(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            var d = button.Tag as CustomerForPackage;

            new Window
            {
                Content = new CustomerForPackageWindow(d),
                Title = $"customer {d.Id}",
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.CanResize

            }.Show();
        }

        private void DeletePackage(object sender, RoutedEventArgs e)
        {
            Bl.DeletePackage(BLpackage.Id);
            Exit();
        }
    }

}
