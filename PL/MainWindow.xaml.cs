using PL.Pages;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GridMain.Children.Add(new HomeViewTab());
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridMain.Children.Clear();
            UserControl page = ((ListViewItem)((ListView)sender).SelectedItem).Name switch
            {
                "HomeView" => new HomeViewTab(),
                "DronesView" => new DronesViewTab(),
                "StationsView" => new StationsViewTab(),
                "CustomersView" => new CustomersViewTab(),
                "PackagesView" => new PackagesViewTab(),
                _ => throw new InvalidOperationException("Unreachable!"),
            };
            GridMain.Children.Add(page);
        }
    }
}
