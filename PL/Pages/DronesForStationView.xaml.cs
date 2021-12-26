using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesForStationView.xaml
    /// </summary>
    public partial class DronesForStationView : UserControl
    {
        public DronesForStationView()
        {
            InitializeComponent();
        }
        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((StationViewTab)Parent).Focusable = true;
            ((StationViewTab)Parent).Focus();
        }
    }
}
