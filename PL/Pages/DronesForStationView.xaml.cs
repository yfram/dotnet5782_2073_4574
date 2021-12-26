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
