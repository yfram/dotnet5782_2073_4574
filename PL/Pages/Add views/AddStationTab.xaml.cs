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
                Bl.AddStation(new(int.Parse(NewId.Text), NewName.Text, new(0, 0), int.Parse(NewSlots.Text)));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Exit();
        }
    }
}
