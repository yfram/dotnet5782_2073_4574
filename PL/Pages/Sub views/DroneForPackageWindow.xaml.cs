using BO;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for PackageForCustomer.xaml
    /// </summary>
    public partial class DroneForPackageWindow : UserControl
    {
        private DroneForPackage BLDrone { get => Resources["drone"] as DroneForPackage; set => Resources["drone"] = value; }

        public DroneForPackageWindow(DroneForPackage p)
        {
            InitializeComponent();
            BLDrone = p;
        }
    }
}
