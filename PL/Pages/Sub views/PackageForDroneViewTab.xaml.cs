﻿using BO;
using System.Linq;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DroneViewTab.xaml
    /// </summary>
    public partial class PackageForDroneViewTab : UserControl
    {
        private PackageInTransfer BLpackage { get => Resources["package"] as PackageInTransfer; set => Resources["package"] = value; }

        public PackageForDroneViewTab(int? id)
        {
            if (id is null) return;
            InitializeComponent();
            BLpackage = MainWindow.BL.GetDroneById(MainWindow.BL.GetAllDronesWhere(d =>
                d.PassingPckageId == (int)id).First().Id).Package;
        }

        public void update(PackageInTransfer newPkg)
        {
            BLpackage = newPkg;
        }
    }
}
