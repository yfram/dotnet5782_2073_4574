﻿// File StationsViewTab.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for StationsViewTab.xaml
    /// </summary>
    public partial class StationsViewTab : UserControl
    {
        private enum SelectdStates { No, Number, Has };

        public ObservableCollection<StationForList> StationsView { get; set; }
        private static IBL Bl => BlFactory.GetBl();

        private bool gridOpen = false;
        private readonly bool packageView = false;

        public StationsViewTab()
        {
            StationsView = new(Bl.GetAllStations());

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

        public StationsViewTab RefreshBl()
        {
            StationsView.Clear();
            Bl.GetAllStations().ToList().ForEach(s => StationsView.Add(s));
            return this;
        }

        private void Collected_View_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var StationsGroup = (CollectionViewSource)Resources["StationsGroup"];
            StationsGroup.GroupDescriptions.Clear();

            if (Collected_View.SelectedIndex == (int)SelectdStates.No)
                return;

            string prop = Collected_View.SelectedIndex == (int)SelectdStates.Number ? "AmountOfEmptyPorts" : "HasEmptyPorts";
            StationsGroup.GroupDescriptions.Add(new PropertyGroupDescription(prop));

        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            PullUpMenueContainer.Children.Add(new AddStationTab());
            PullUpMenueContainer.Expand(250, 150);
            gridOpen = true;
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (packageView)
                return;
            try
            {
                PullUpMenueContainer.Children.Add(new StationViewTab(StationsView[((DataGridRow)sender).GetIndex()].Id));
                PullUpMenueContainer.Expand(250, 150);
                gridOpen = true;
            }
            catch (Exception ex) { }
        }
    }
}
