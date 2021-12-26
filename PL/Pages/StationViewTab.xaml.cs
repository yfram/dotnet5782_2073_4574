using BO;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

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
            BLstation = MainWindow.BL.DisplayStation(id);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.BL.UpdateStation(BLstation.Id,StationName.Text, int.Parse(EmptySlots.Text));
                BLstation = MainWindow.BL.DisplayStation(BLstation.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focusable = true;
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focus();
        }


        private void UpdateView()
        {
            BLstation = MainWindow.BL.DisplayStation(BLstation.Id);
        }

        private void View_Drones(object sender, RoutedEventArgs e)
        {
            GridMain.Children.Clear();
            UIElement menue = new DronesForStationView();

            GridMain.Children.Add(menue);
            
            /*
            BLstation.ChargingDrones.GetEnumerator().Current.
            Storyboard.SetTargetName(myDoubleAnimation, "UpdateMenue");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(HeightProperty));
            Storyboard storyboard = new Storyboard();

            UpdateMenue.Children.Clear();
            UIElement menue = new();
            menue = new StationViewTab(id ?? -1);
            storyboard.Children.Add(myDoubleAnimation);
            BeginStoryboard(storyboard);

            UpdateMenue.Children.Add(menue);
            */
        }
    }
}
