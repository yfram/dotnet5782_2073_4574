using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DroneViewTab.xaml
    /// </summary>
    public partial class PackageViewTab : UserControl
    {
        private Package BLpackage { get => Resources["package"] as Package; set => Resources["package"] = value; }

        public PackageViewTab(int id)
        {
            InitializeComponent();
            BLpackage = MainWindow.BL.DisplayPackage(id);
        }
    }
}
