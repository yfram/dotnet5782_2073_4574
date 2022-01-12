// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using BO;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for AddDroneTab.xaml
    /// </summary>
    public partial class AddDroneTab : UserControl
    {
        private static IBL Bl => BlFactory.GetBl();

        public AddDroneTab()
        {
            InitializeComponent();
            Bl.GetStationsWithCharges().ToList().ForEach(s => Stations.Items.Add(s.Id));
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((DronesViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void AddDrone(object sender, RoutedEventArgs e)
        {
            try
            {
                Bl.AddDrone(new(int.Parse(NewId.Text), NewModel.Text,
                                NewWeight.Text switch { "Light" => WeightGroup.Light, "Mid" => WeightGroup.Mid, "Heavy" => WeightGroup.Heavy, _ => throw new InvalidOperationException() },
                                new Random().NextDouble() * 100, DroneState.Maitenance, new(),
                                Bl.GetStationById(int.Parse(Stations.Text)).LocationOfStation));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Exit();
        }
    }
}
