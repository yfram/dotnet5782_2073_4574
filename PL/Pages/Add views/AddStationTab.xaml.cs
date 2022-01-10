using BlApi;
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
