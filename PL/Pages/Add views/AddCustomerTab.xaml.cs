// File AddCustomerTab.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for AddCustomerTab.xaml
    /// </summary>
    public partial class AddCustomerTab : UserControl
    {
        private static IBL Bl => BlFactory.GetBl();

        public AddCustomerTab()
        {
            InitializeComponent();
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((CustomersViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void AddCustomer(object sender, RoutedEventArgs e)
        {
            try
            {
                Bl.AddCustomer(new(int.Parse(NewId.Text), NewName.Text, NewPhone.Text,
                    new(Double.Parse(NewLong.Text), double.Parse(NewLat.Text)),
                    new List<BO.PackageForCustomer>(), new List<BO.PackageForCustomer>()));

            }
            catch (BlApi.Exceptions.ObjectAllreadyExistsException)
            {
                MessageBox.Show($"Customer {NewId.Text} cannot be added, as it already exists",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Exit();
        }
    }
}
