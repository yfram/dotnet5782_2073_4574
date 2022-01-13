// File StationViewTab.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for StationViewTab.xaml
    /// </summary>
    public partial class StationViewTab : UserControl
    {
        private Station BLstation { get => Resources["station"] as Station; set => Resources["station"] = value; }
        private static IBL Bl => BlFactory.GetBl();

        public StationViewTab(int id)
        {
            InitializeComponent();
            BLstation = Bl.GetStationById(id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            //we know the station exists
            Bl.UpdateStation(BLstation.Id, StationName.Text, int.Parse(EmptySlots.Text));
            BLstation = Bl.GetStationById(BLstation.Id);
        }
        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((StationsViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void View_Drones(object sender, RoutedEventArgs e)
        {
            GridMain.Children.Clear();
            UIElement menue = new DronesForStationView(BLstation.Id);
            GridMain.Children.Add(menue);
        }
    }
}
