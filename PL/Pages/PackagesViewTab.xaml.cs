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
            MainWindow.BL.DisplayPackages().ToList().ForEach(d => PackagesView.Add(d));

            packages = new(PackagesView);

            InitializeComponent();

            GotFocus += PackagesViewTab_GotFocus;
        }

        private void PackagesViewTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!gridOpen || e.OriginalSource is not Button button) return;
            PackagesView.Clear();
            packages.Clear();
            MainWindow.BL.DisplayPackages().ToList().ForEach(d => PackagesView.Add(d));
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
            if (packageView) return;
            ShowMenue(PackagesView[((DataGridRow)sender).GetIndex()].Id, "package view");
        }

        private void Filter(object sender, RoutedEventArgs e) 
        {

        }

        private void FilterByDate()
        {
        }
    }
}
