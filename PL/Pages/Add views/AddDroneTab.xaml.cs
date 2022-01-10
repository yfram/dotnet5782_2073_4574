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
    /// Interaction logic for AddDroneTab.xaml
    /// </summary>
    public partial class AddDroneTab : UserControl
    {
        public AddDroneTab()
        {
            InitializeComponent();
            MainWindow.BL.GetStationsWithCharges().ToList().ForEach(s => Stations.Items.Add(s.Id));
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((DronesViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void AddDrone(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.BL.AddDrone(new(int.Parse(NewId.Text), NewModel.Text,
                                NewWeight.Text switch { "Light" => WeightGroup.Light, "Mid" => WeightGroup.Mid, "Heavy" => WeightGroup.Heavy, _ => throw new InvalidOperationException() },
                                new Random().NextDouble() * 100, DroneState.Maitenance, new(),
                                MainWindow.BL.GetStationById(int.Parse(Stations.Text)).LocationOfStation));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Exit();
        }
    }
}
