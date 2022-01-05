using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class CustomersViewTab : UserControl
    {

        public ObservableCollection<CustomerForList> CustomersView { get; set; } = new();

        public List<CustomerForList> customers;

        private bool gridOpen = false;
        private bool packageView = false;

        public CustomersViewTab()
        {
            MainWindow.BL.DisplayCustomers().ToList().ForEach(d => CustomersView.Add(d));

            customers = new(CustomersView);

            InitializeComponent();

            GotFocus += CustomersViewTab_GotFocus;
        }

        private void CustomersViewTab_GotFocus(object sender, RoutedEventArgs e)
        {
            if (!gridOpen || e.OriginalSource is not Button button)
            {
                return;
            }

            CustomersView.Clear();
            customers.Clear();
            MainWindow.BL.DisplayCustomers().ToList().ForEach(d => CustomersView.Add(d));
            customers = new(CustomersView);

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
                case "customer view":
                    menue = new CustomerViewTab(id ?? -1);
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
                MainWindow.BL.AddCustomer(new(int.Parse(NewId.Text), NewName.Text, NewPhone.Text, new(Double.Parse(NewLong.Text), Double.Parse(NewLat.Text)), new List<BO.PackageForCustomer>(), new List<BO.PackageForCustomer>()));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            NewId.Text = "";
            NewName.Text = "";
            NewPhone.Text = "";
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

            ShowMenue(CustomersView[((DataGridRow)sender).GetIndex()].Id, "customer view");
        }
    }
}
