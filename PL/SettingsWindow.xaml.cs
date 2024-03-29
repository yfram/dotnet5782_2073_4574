﻿// File SettingsWindow.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;

namespace PL
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : UserControl
    {
        private int curState;

        public SettingsWindow()
        {
            InitializeComponent();
            Directory.EnumerateFiles(@"Pl/Assets/Themes").ToList().ForEach(f =>
                ThemeOptions.Items.Add(f.Replace("Theme.xaml", "").Remove(0, f.LastIndexOf('\\') + 1)));
            ThemeOptions.SelectedIndex = ThemeOptions.Items.IndexOf(((App)Application.Current).CurrentTheme);

            XElement dalConfig = XElement.Load(@"xml\dal-config.xml");
            string current = dalConfig.Element("dal").Value;
            curState = DataOptions.SelectedIndex = current == "list" ? 0 : 1;
        }

        private void ChangeDal()
        {
            XElement dalConfig = XElement.Load(@"xml\dal-config.xml");
            dalConfig.Element("dal").Value = (DataOptions.SelectedIndex == 0 ? "list" : "xml");
            dalConfig.Save(@"xml\dal-config.xml");

            BlApi.BlFactory.Init();

            ((MainWindow)Application.Current.MainWindow).RefreshCurrent();
            ((MainWindow)Application.Current.MainWindow).RefreshAll();
        }

        private void ApplyChanges(object sender, RoutedEventArgs e)
        {
            ((Grid)Parent).Visibility = Visibility.Collapsed;

            ((App)Application.Current).ChangeTheme(ThemeOptions.SelectedItem as string ?? "Dark");

            if (MapGrid.Visibility == Visibility.Visible)// this means we are on the home tab
            {
                Pages.HomeViewTab.ShowDrones = DronesCheck.IsChecked ?? false;
                Pages.HomeViewTab.ShowStations = StationsCheck.IsChecked ?? false;
                Pages.HomeViewTab.ShowCustomers = CustomersCheck.IsChecked ?? false;
                ((MainWindow)Application.Current.MainWindow).RefreshCurrent();
            }

            if (DataOptions.SelectedIndex != curState)
            {
                curState = DataOptions.SelectedIndex;
                ChangeDal();
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((MainWindow)Application.Current.MainWindow).GridMain.Children[0] is Pages.HomeViewTab)
                MapGrid.Visibility = Visibility.Visible;
            else
                MapGrid.Visibility = Visibility.Collapsed;
        }
    }
}
