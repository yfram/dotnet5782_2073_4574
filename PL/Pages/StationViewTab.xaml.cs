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
        public StationViewTab(int id)
        {
            InitializeComponent();
            BLstation = MainWindow.BL.GetStationById(id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.BL.UpdateStation(BLstation.Id, StationName.Text, int.Parse(EmptySlots.Text));
                BLstation = MainWindow.BL.GetStationById(BLstation.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void Exit(object sender, RoutedEventArgs e)
        {
            ((StationsViewTab)((Grid)((Grid)Parent).Parent).Parent).Focusable = true;
            ((StationsViewTab)((Grid)((Grid)Parent).Parent).Parent).Focus();
        }

        private void View_Drones(object sender, RoutedEventArgs e)
        {
            GridMain.Children.Clear();
            UIElement menue = new DronesForStationView(BLstation.Id);

            GridMain.Children.Add(menue);
        }
    }
}
