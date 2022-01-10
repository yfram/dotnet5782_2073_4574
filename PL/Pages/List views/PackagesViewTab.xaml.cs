﻿using BO;
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
    /// Interaction logic for PackagesViewTab.xaml
    /// </summary>
    public partial class PackagesViewTab : UserControl
    {

        public ObservableCollection<PackageForList> PackagesView { get; set; } = new();

        public List<PackageForList> packages;

        private bool gridOpen = false;
        private bool packageView = false;

        public PackagesViewTab()
        {
            MainWindow.BL.GetAllPackages().ToList().ForEach(d => PackagesView.Add(d));

            packages = new(PackagesView);

            InitializeComponent();

            GotFocus += PackagesViewTab_GotFocus;
        }

        private void PackagesViewTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!gridOpen || e.OriginalSource is not Button button)
            {
                return;
            }

            PackagesView.Clear();
            packages.Clear();
            MainWindow.BL.GetAllPackages().ToList().ForEach(d => PackagesView.Add(d));
            packages = new(PackagesView);

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
                case "package view":
                    menue = new PackageViewTab(id ?? -1);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            storyboard.Children.Add(myDoubleAnimation);
            BeginStoryboard(storyboard);

            UpdateMenue.Children.Add(menue);
        }

        private void AddCustomer(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.BL.AddPackage(int.Parse(Sid.Text), int.Parse(Rid.Text), (WeightGroup)(((ComboBox)Weight).SelectedIndex + 1), (PriorityGroup)((ComboBox)(Priority)).SelectedIndex + 1);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Sid.Text = "";
            Rid.Text = "";
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
            var p = ((DataGridRow)sender).DataContext as PackageForList;
            ShowMenue(p.Id, "package view");
        }

        private void Filter(object sender, RoutedEventArgs e)
        {
            PackagesView.Clear();
            packages.Where(p =>
            FilterDate().Any(pI => pI.Id == p.Id) &&
            FilterState().Any(pI => pI.Id == p.Id)
            ).ToList().ForEach(p => PackagesView.Add(p));
        }

        private IEnumerable<PackageForList> FilterState()
        {
            if (!(FilterByState.IsChecked ?? false)) return packages;
            string check = ((ComboBoxItem)StateFilter.SelectedItem).Content.ToString().Replace(" ", "");
            return
                MainWindow.BL.
                GetObjectsWhere<PackageForList>(p => p.Status.ToString().ToLower() == check).
                Cast<PackageForList>();
        }

        private IEnumerable<PackageForList> FilterDate()
        {
            if (StartDate.Value is null || EndDate.Value is null) return packages;
            if (!(FilterByDate.IsChecked ?? false))
            {
                return packages;
            }
            List<PackageForList> datePackages =
                MainWindow.BL.GetObjectsWhere<Package>(p => p.TimeToPickup >= StartDate.Value && p.TimeToPickup <= EndDate.Value).
                Cast<PackageForList>().ToList();
            return datePackages;
        }

        private void Collected_view(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox senderAsCheckBox) return;
            //becuase of the order of the && operator this works
            if ((string)(senderAsCheckBox.Tag) == "Sender" && CollectedReciver.IsChecked.Value && CollectedSender.IsChecked.Value)
                CollectedReciver.IsChecked = !senderAsCheckBox.IsChecked;
            else if (CollectedReciver.IsChecked.Value && CollectedSender.IsChecked.Value)
                CollectedSender.IsChecked = !senderAsCheckBox.IsChecked;
            var packageView = (CollectionViewSource)Resources["PackagesGroup"];
            packageView.GroupDescriptions.Clear();
            if (senderAsCheckBox.IsChecked.Value)
            {
                packageView.GroupDescriptions.Add(new PropertyGroupDescription(senderAsCheckBox.Tag == "Reciver" ?
                    "NameOfReciver" : "NameOfSender"));
            }

        }
    }
}