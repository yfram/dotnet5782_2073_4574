using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DroneViewTab.xaml
    /// </summary>
    public partial class PackageForDroneViewTab : UserControl
    {
        private PackageInTransfer BLpackage { get => Resources["package"] as PackageInTransfer; set => Resources["package"] = value; }

        public PackageForDroneViewTab(PackageInTransfer p)
        {
            InitializeComponent();
            BLpackage = p;
        }
    }
}
