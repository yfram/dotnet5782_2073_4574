using PL.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using IBL;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static IBL.IBL BL = new BL();
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
            UserControl page;
            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "HomeView":
                    page = new HomeViewTab();
                    break;
                case "DronesView":
                    page = new DronesViewTab();
                    break;
                default:
                    throw new InvalidOperationException("Unreachable!");
            }
            GridMain.Children.Add(page);
        }
    }
}
