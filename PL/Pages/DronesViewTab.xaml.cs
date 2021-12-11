using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;
using System.Windows;
using System.Text.RegularExpressions;
using System.Windows.Input;
using BO;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class DronesViewTab : UserControl
    {
        public List<Drone> Drones { get; set; } = new();

        public List<Drone> drones;

        private bool gridOpen = false;

        public DronesViewTab()
        {
            MainWindow.BL.DisplayDrones().ToList().ForEach(d => Drones.Add(MainWindow.BL.DisplayDrone(d.Id)));
            drones = Drones.Where(d => true).ToList();
            InitializeComponent();
            GotFocus += DronesViewTab_GotFocus;
        }

        private void DronesViewTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!gridOpen || e.OriginalSource is not Button button) return;
            Drones.Clear();
            drones.Clear();
            MainWindow.BL.DisplayDrones().ToList().ForEach(d => Drones.Add(MainWindow.BL.DisplayDrone(d.Id)));
            drones = Drones.Where(d => true).ToList();

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
            DroneGrid.ItemsSource = Drones;
            DroneGrid.Items.Refresh();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox senderAsCheckBox) return;
            List<Func<Drone, bool>> funcs = new();
            foreach (CheckBox checkBox in FilterGrid.Children.OfType<CheckBox>())
                switch (checkBox.Content)
                {
                    case "Empty":
                    case "Maitenance":
                    case "Busy":
                        if ((checkBox.IsChecked ?? false))//false is unreacable
                            funcs.Add((Drone d) => d.State.ToString() == (string)checkBox.Content);
                        break;
                    case "Light":
                    case "Mid":
                    case "Heavy":
                        if ((checkBox.IsChecked ?? false))//false is unreacable
                            funcs.Add((Drone d) => d.Weight.ToString() == (string)checkBox.Content);
                        break;
                    default:
                        throw new AccessViolationException();
                }
            Drones.Clear();
            if (funcs.Count == 0)
                Drones = drones.Where(d => true).ToList();
            foreach (Func<Drone, bool> func in funcs)
                Drones.AddRange(drones.Where(func).Except(Drones));
            DroneGrid.ItemsSource = Drones;
            DroneGrid.Items.Refresh();
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
            gridOpen = true;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            Storyboard.SetTargetName(myDoubleAnimation, "UpdateMenue");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(HeightProperty));
            Storyboard storyboard = new Storyboard();

            storyboard.Children.Add(myDoubleAnimation);
            BeginStoryboard(storyboard);

            UpdateMenue.Children.Clear();
            Drone drone = Drones[((DataGridRow)sender).GetIndex()];
            UpdateMenue.Children.Add(new DroneViewTab(drone));
        }
    }
}
