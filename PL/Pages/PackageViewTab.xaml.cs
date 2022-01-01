﻿using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            BLpackage = MainWindow.BL.DisplayPackage(id);
            
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focusable = true;
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focus();
        }

        private void OpenDrone(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var d= button.Tag as DroneForPackage;

            var w = new Window
            {
                Content = new DroneForPackageWindow(d),
                Title = $"drone {d.Id}",
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.CanResize

            };
            w.Show();
        }

        private void OpenCustomer(object sender, RoutedEventArgs e)
        {

            var button = sender as Button;
            var d = button.Tag as CustomerForPackage;

            var w = new Window
            {
                Content = new CustomerForPackageWindow(d),
                Title = $"customer {d.Id}",
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.CanResize

            };
            w.Show();
        }
    }


}