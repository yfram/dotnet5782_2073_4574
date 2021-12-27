using BO;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesForStationView.xaml
    /// </summary>
    public partial class DronesForStationView : UserControl
    {

        public DronesForStationView(int stationId)
        {
            InitializeComponent();

            ListOfDrones.ItemsSource = MainWindow.BL.DisplayStation(stationId).ChargingDrones;
        }
        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((StationViewTab)Parent).Focusable = true;
            ((StationViewTab)Parent).Focus();
        }
    }
}
