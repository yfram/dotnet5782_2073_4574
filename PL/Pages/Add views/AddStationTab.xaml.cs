// File AddStationTab.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for AddStationTab.xaml
    /// </summary>
    public partial class AddStationTab : UserControl
    {
        private IBL Bl = BlFactory.GetBl();

        public AddStationTab()
        {
            InitializeComponent();
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((StationsViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void AddStation(object sender, RoutedEventArgs e)
        {
            try
            {
                Bl.AddStation(new(int.Parse(NewId.Text), NewName.Text, new(double.Parse(NewLong.Text), double.Parse(NewLat.Text)), int.Parse(NewSlots.Text)));

            }
            catch (BlApi.Exceptions.ObjectAllreadyExistsException)
            {
                MessageBox.Show($"Station {NewId.Text} cannot be added, as it already exists",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Exit();
        }
    }
}
