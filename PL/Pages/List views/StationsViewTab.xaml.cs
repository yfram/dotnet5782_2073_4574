using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for StationsViewTab.xaml
    /// </summary>
    public partial class StationsViewTab : UserControl
    {
        private enum SelectdStates { No, Number, Has };

        public ObservableCollection<StationForList> StationsView { get; set; } = new();

        public List<StationForList> stations;

        private bool gridOpen = false;
        private bool packageView = false;

        public StationsViewTab()
        {
            MainWindow.BL.GetAllStations().ToList().ForEach(d => StationsView.Add(d));

            stations = new(StationsView);

            InitializeComponent();

            var StationsGroup = (CollectionViewSource)Resources["StationsGroup"];
            StationsGroup.GroupDescriptions.Clear();

            GotFocus += StationsViewTab_GotFocus;
        }

        private void StationsViewTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!gridOpen || e.OriginalSource is not Button button)
            {
                return;
            }

            StationsView.Clear();
            stations.Clear();
            MainWindow.BL.GetAllStations().ToList().ForEach(d => StationsView.Add(d));
            stations = new(StationsView.Where(d => true).ToArray());

            //this way only the exit button acctualy closes the update view
            if (button.Name == "ExitButton" && UpdateMenue.Height != 0)
            {
                DoubleAnimation myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 150;
                myDoubleAnimation.To = 0;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
                Storyboard.SetTargetName(myDoubleAnimation, "UpdateMenue");
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(HeightProperty));
                Storyboard storyboard = new Storyboard();

                storyboard.Children.Add(myDoubleAnimation);
                BeginStoryboard(storyboard);

                UpdateMenue.Children.Clear();

                gridOpen = false;
            }
            if (AddMenue.Height != 0)
            {
                ButtonOpenMenu.Visibility = Visibility.Visible;
                DoubleAnimation myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 150;
                myDoubleAnimation.To = 0;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
                Storyboard.SetTargetName(myDoubleAnimation, "AddMenue");
                Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(HeightProperty));
                Storyboard storyboard = new Storyboard();

                storyboard.Children.Add(myDoubleAnimation);
                BeginStoryboard(storyboard);
                gridOpen = false;
            }
        }

        private void Collected_View_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var StationsGroup = (CollectionViewSource)Resources["StationsGroup"];
            StationsGroup.GroupDescriptions.Clear();

            if (Collected_View.SelectedIndex == (int)SelectdStates.No)
            {
                return;
            }

            String prop = Collected_View.SelectedIndex == (int)SelectdStates.Number ? "AmountOfEmptyPorts" : "HasEmptyPorts";
            StationsGroup.GroupDescriptions.Add(new PropertyGroupDescription(prop));

        }

        private void ShowMenue(int? id, string typeOfMenue)
        {
            gridOpen = true;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            Storyboard.SetTargetName(myDoubleAnimation, "UpdateMenue");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(HeightProperty));
            Storyboard storyboard = new Storyboard();

            UpdateMenue.Children.Clear();
            UserControl menue = new();
            switch (typeOfMenue)
            {
                case "station view":
                    menue = new StationViewTab(id ?? -1);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            storyboard.Children.Add(myDoubleAnimation);
            BeginStoryboard(storyboard);

            UpdateMenue.Children.Add(menue);
        }

        private void AddStation(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.BL.AddStation(new(int.Parse(NewId.Text), NewName.Text, new(0, 0), int.Parse(NewSlots.Text)));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            NewId.Text = "";
            NewName.Text = "";
            NewSlots.Text = "-1";
            Focus();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = Visibility.Collapsed;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            Storyboard.SetTargetName(myDoubleAnimation, "AddMenue");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(HeightProperty));
            Storyboard storyboard = new Storyboard();

            storyboard.Children.Add(myDoubleAnimation);
            BeginStoryboard(storyboard);

            gridOpen = true;

            //MainWindow.BL.DisplayStations().ToList().ForEach(s => Stations.Items.Add(s.Id));
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (packageView)
            {
                return;
            }

            ShowMenue(StationsView[((DataGridRow)sender).GetIndex()].Id, "station view");
        }
    }
}
