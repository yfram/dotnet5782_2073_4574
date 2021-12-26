using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class StationsViewTab : UserControl
    {

        enum SelectdStates {No, Number , Has};

        public ObservableCollection<StationForList> StationsView { get; set; } = new();

        public List<StationForList> stations;

        private bool gridOpen = false;
        private bool packageView = false;

        public StationsViewTab()
        {
            MainWindow.BL.DisplayStations().ToList().ForEach(d => StationsView.Add(d));

            stations = new (StationsView);

            InitializeComponent();

            var StationsGroup = (CollectionViewSource)Resources["StationsGroup"];
            StationsGroup.GroupDescriptions.Clear();

            GotFocus += StationsViewTab_GotFocus;
        }

        private void StationsViewTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!gridOpen || e.OriginalSource is not Button button) return;
            StationsView.Clear();
            stations.Clear();
            MainWindow.BL.DisplayStations().ToList().ForEach(d => StationsView.Add(d));
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

        /*
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            
            if (sender is not CheckBox senderAsCheckBox) return;
            
            List<Func<Drone, bool>> weightFuncs = new();
            List<Func<Drone, bool>> statusFuncs = new();
            
            foreach (CheckBox checkBox in FilterGrid.Children.OfType<CheckBox>())
                switch (checkBox.Content)
                {
                    
                    case "Empty":
                    case "Maitenance":
                    case "Busy":
                        if ((checkBox.IsChecked ?? false))//false is unreacable
                            statusFuncs.Add((Drone d) => d.State.ToString() == (string)checkBox.Content);
                        break;
                    case "Light":
                    case "Mid":
                    case "Heavy":
                        if ((checkBox.IsChecked ?? false))//false is unreacable
                            weightFuncs.Add((Drone d) => d.Weight.ToString() == (string)checkBox.Content);
                        break;
                    
                    case "Collected_View":
                        break;
                    default:
                        throw new AccessViolationException();
                }
            
            Drones.Clear();

            drones.Where(d => (weightFuncs.Any(f => f(d)) || weightFuncs.Count == 0) && (statusFuncs.Any(f => f(d)) || statusFuncs.Count == 0)).ToList().ForEach(elem => Drones.Add(elem));
            
        }
        */

        private void Collected_View_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var StationsGroup = (CollectionViewSource)Resources["StationsGroup"];
            StationsGroup.GroupDescriptions.Clear();

            if (Collected_View.SelectedIndex == (int)SelectdStates.No)
                return;


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
            UIElement menue = new();
            switch (typeOfMenue)
            {
                /*
                case "drone add":
                    break;
                */
                case "station view":
                    menue = new StationViewTab(id ?? -1);
                    break;
                    /*
                case "package view":
                    menue = new PackageViewTab(id ?? -1);
                    break;
                    */
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
                Station s;

                MainWindow.BL.AddStation(new(int.Parse(NewId.Text), NewName.Text, new(0,0), int.Parse(NewSlots.Text)));
                
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
            if (packageView) return;
            ShowMenue(StationsView[((DataGridRow)sender).GetIndex()].Id, "station view");
        }
        /*
        private void PackageId_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            packageView = true;
            ShowMenue(Convert.ToInt32(((TextBlock)((DataGridCell)sender).Content).Text), "package view");
        }
        */
            }
}
