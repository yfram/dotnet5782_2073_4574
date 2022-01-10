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

        public PackageViewTab(int id)
        {
            InitializeComponent();
            BLpackage = MainWindow.BL.GetPackageById(id);
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
            MainWindow.BL.DeletePackage(BLpackage.Id);
            Exit();
        }
    }

}
