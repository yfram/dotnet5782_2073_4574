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

            ListOfDrones.ItemsSource = BlApi.BlFactory.GetBl().GetStationById(stationId).ChargingDrones;
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((DronesViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }
    }
}
