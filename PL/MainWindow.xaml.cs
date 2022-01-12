// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using PL.Pages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal HomeViewTab home = new();
        internal DronesViewTab drones = new();
        internal StationsViewTab stations = new();
        internal CustomersViewTab customers = new();
        internal PackagesViewTab packages = new();
        public MainWindow()
        {
            InitializeComponent();
            GridMain.Children.Add(new HomeViewTab());
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            SettingsWindow.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        public void RefreshCurrent()
        {
            dynamic cur = GridMain.Children[0];

            cur.RefreshBl();
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridMain.Children.Clear();
            UserControl page = ((ListViewItem)((ListView)sender).SelectedItem).Name switch
            {
                "HomeView" => home.RefreshBl(),
                "DronesView" => drones.RefreshBl(),
                "StationsView" => stations.RefreshBl(),
                "CustomersView" => customers.RefreshBl(),
                "PackagesView" => packages.RefreshBl(),
                _ => throw new InvalidOperationException("Unreachable!"),
            };
            GridMain.Children.Add(page);
        }
    }
}
