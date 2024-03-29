﻿// File DronesViewTab.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class DronesViewTab : UserControl
    {

        public ObservableCollection<DroneForList> Drones { get; set; }
        private static IBL Bl => BlFactory.GetBl();

        private bool gridOpen = false;
        private bool packageViewInTheMiddle = false;

        private IEnumerable<DroneViewTab> cache = new List<DroneViewTab>();
        public DronesViewTab()
        {
            Drones = new(Bl.GetAllDrones());
            InitializeComponent();
        }

        public void CollapsePullUp()
        {
            if (!gridOpen)
                return;

            PullUpMenueContainer.Collapse(250);
            RefreshBl();
            gridOpen = false;
        }

        public DronesViewTab RefreshBl()
        {
            Drones.Clear();
            Bl.GetAllDrones().ToList().ForEach(d => Drones.Add(d));
            return this;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox senderAsCheckBox)
                return;

            List<Func<DroneForList, bool>> weightFuncs = new();
            List<Func<DroneForList, bool>> statusFuncs = new();
            foreach (CheckBox checkBox in FilterGrid.Children.OfType<CheckBox>())
            {
                switch (checkBox.Content)
                {
                    case "Empty":
                    case "Maintenance":
                    case "Busy":
                        if ((checkBox.IsChecked ?? false))//false is unreachable
                            statusFuncs.Add((DroneForList d) => d.State.ToString() == (string)checkBox.Content);

                        break;
                    case "Light":
                    case "Mid":
                    case "Heavy":
                        if ((checkBox.IsChecked ?? false))//false is unreachable
                            weightFuncs.Add((DroneForList d) => d.Weight.ToString() == (string)checkBox.Content);

                        break;
                    case "Collected_View":
                        break;
                    default:
                        throw new AccessViolationException();
                }
            }

            Drones.Clear();
            Bl.GetAllDronesWhere(d => (weightFuncs.Any(f => f(d)) || weightFuncs.Count == 0) && (statusFuncs.Any(f => f(d)) || statusFuncs.Count == 0)).ToList().ForEach(elem => Drones.Add(elem));
        }

        private void Collected_view(object sender, RoutedEventArgs e)
        {
            var droneView = (CollectionViewSource)Resources["DronesGroup"];
            droneView.GroupDescriptions.Clear();
            if (((CheckBox)sender).IsChecked.HasValue && ((CheckBox)sender).IsChecked.Value)
                droneView.GroupDescriptions.Add(new PropertyGroupDescription("State"));
        }

        private void ShowMenue(int? id, string typeOfMenue)
        {
            PullUpMenueContainer.Children.Clear();
            UIElement menue;
            menue = typeOfMenue switch
            {
                "drone add" => new AddDroneTab(),
                "drone view" => cache.Any(x => x.id == id) ? cache.Where(x => x.id == id).First() : AddCache(id),
                "package view" => new PackageForDroneViewTab(id),
                _ => throw new InvalidOperationException(),
            };
            PullUpMenueContainer.Children.Add(menue);
            gridOpen = true;
            PullUpMenueContainer.Expand(250, 150);
        }

        private DroneViewTab AddCache(int? id)
        {
            var newDrone = new DroneViewTab(id ?? -1);
            cache = cache.Append(newDrone);
            return newDrone;
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ShowMenue(null, "drone add");
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            //this if allows to render the package view with out being overwritten
            if (packageViewInTheMiddle)
            {
                packageViewInTheMiddle = false;
                return;
            }
            try
            {
                ShowMenue(Drones[((DataGridRow)sender).GetIndex()].Id, "drone view");
            }
            catch (Exception)
            { }
        }
    }
}
