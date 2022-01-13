// File AddPackageTab.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for AddPackageTab.xaml
    /// </summary>
    public partial class AddPackageTab : UserControl
    {
        private static IBL Bl => BlFactory.GetBl();

        public AddPackageTab()
        {
            InitializeComponent();
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((PackagesViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void AddPackage(object sender, RoutedEventArgs e)
        {
            try
            {
                Bl.AddPackage(int.Parse(Sid.Text), int.Parse(Rid.Text), (WeightGroup)(((ComboBox)Weight).SelectedIndex + 1), (PriorityGroup)((ComboBox)(Priority)).SelectedIndex + 1);
            }
            catch (BlApi.Exceptions.ObjectAllreadyExistsException)
            {
                MessageBox.Show($"Package {Sid.Text} cannot be added, as it already exists",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Exit();
        }
    }
}
