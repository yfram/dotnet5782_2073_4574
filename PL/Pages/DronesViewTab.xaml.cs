using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class DronesViewTab : UserControl
    {
        public ObservableCollection<Drone> Drones { get; set; } = new();

        public List<Drone> drones;

        private bool gridOpen = false;
        private bool packageViewInTheMiddle = false;

        public DronesViewTab()
        {
            MainWindow.BL.DisplayDrones().ToList().ForEach(d => Drones.Add(MainWindow.BL.DisplayDrone(d.Id)));
            drones = new(Drones.Where(d => true).ToArray());

            InitializeComponent();

            var droneView = (CollectionViewSource)Resources["DronesGroup"];
            droneView.GroupDescriptions.Clear();

            GotFocus += DronesViewTab_GotFocus;
        }

        private void DronesViewTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!gridOpen || e.OriginalSource is not Button button) return;
            Drones.Clear();
            drones.Clear();
            MainWindow.BL.DisplayDrones().ToList().ForEach(d => Drones.Add(MainWindow.BL.DisplayDrone(d.Id)));
            drones = new(Drones.Where(d => true).ToArray());

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

        private void Collected_view(object sender, RoutedEventArgs e)
        {
            var droneView = (CollectionViewSource)Resources["DronesGroup"];
            droneView.GroupDescriptions.Clear();
            if (((CheckBox)sender).IsChecked.HasValue && ((CheckBox)sender).IsChecked.Value)
            {
                droneView.GroupDescriptions.Add(new PropertyGroupDescription("State"));
            }

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
                case "drone add":
                    break;
                case "drone view":
                    menue = new DroneViewTab(id ?? -1);
                    break;
                case "package view":
                    menue = new PackageForDroneViewTab(drones.Where(d => d.Package?.Id == id).ElementAt(0).Package);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            storyboard.Children.Add(myDoubleAnimation);
            BeginStoryboard(storyboard);

            UpdateMenue.Children.Add(menue);
        }

        private void AddDrone(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.BL.AddDrone(new(int.Parse(NewId.Text), NewModel.Text,
                                NewWeight.Text switch { "Light" => WeightGroup.Light, "Mid" => WeightGroup.Mid, "Heavy" => WeightGroup.Heavy, _ => throw new InvalidOperationException() },
                                new Random().NextDouble() * 100, DroneState.Maitenance, new(),
                                MainWindow.BL.DisplayStation(int.Parse(Stations.Text)).LocationOfStation));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            NewId.Text = "";
            NewModel.Text = "";
            NewWeight.SelectedIndex = -1;
            Stations.SelectedIndex = -1;
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

            MainWindow.BL.DisplayStations().ToList().ForEach(s => Stations.Items.Add(s.Id));
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (packageViewInTheMiddle)
            {
                packageViewInTheMiddle = false;
                return;
            }
            ShowMenue(Drones[((DataGridRow)sender).GetIndex()].Id, "drone view");
        }

        private void PackageId_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            packageViewInTheMiddle = true;
            ShowMenue(Convert.ToInt32(((TextBlock)((DataGridCell)sender).Content).Text), "package view");
        }
    }
}
