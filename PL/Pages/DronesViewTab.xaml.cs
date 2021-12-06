﻿using IBL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Animation;
using System.Windows;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class DronesViewTab : UserControl
    {
        public List<Drone> Drones { get; set; } = new();

        List<Drone> drones;

        public DronesViewTab()
        {
            MainWindow.BL.DisplayDrones().ToList().ForEach(d => Drones.Add(MainWindow.BL.DisplayDrone(d.Id)));
            drones = Drones.Where(d => true).ToList();
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
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
            /*MainWindow.BL.AddDrone(, GetStringInput("Enter model:"),
                            (IBL.BO.WeightGroup)GetIntInputInRange("Enter weight group(1 for light, 2 for mid, 3 for heavy)", 1, 3),
                            new Random().Next(20, 40) / 100, IBL.BO.DroneState.Maitenance, new(),
                            GetStationLocation(GetIntInput("Enter starting station id:"), Bl)));*/
        }
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Visibility = System.Windows.Visibility.Collapsed;
            DoubleAnimation myDoubleAnimation = new DoubleAnimation();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = 150;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(250));
            Storyboard.SetTargetName(myDoubleAnimation, "AddMenue");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(Grid.HeightProperty));
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(myDoubleAnimation);
            this.BeginStoryboard(storyboard);
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            Drone drone = Drones[((DataGridRow)sender).GetIndex()];
            ((MainWindow)Application.Current.MainWindow).GridMain.Children.Clear();
            ((MainWindow)Application.Current.MainWindow).GridMain.Children.Add(new DroneViewTab(drone));
        }
    }
}
