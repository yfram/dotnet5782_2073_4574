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
        private IBL Bl { get => BlFactory.GetBl(); }

        public StationViewTab(int id)
        {
            InitializeComponent();
            BLstation = Bl.GetStationById(id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Bl.UpdateStation(BLstation.Id, StationName.Text, int.Parse(EmptySlots.Text));
                BLstation = Bl.GetStationById(BLstation.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
