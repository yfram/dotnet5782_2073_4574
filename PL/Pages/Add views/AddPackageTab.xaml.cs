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
        private IBL Bl => BlFactory.GetBl();

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Exit();
        }
    }
}
