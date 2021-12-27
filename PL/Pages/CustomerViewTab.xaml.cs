using BO;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for CustomerViewTab.xaml
    /// </summary>
    public partial class CustomerViewTab : UserControl
    {
        private Customer BLCustomer { get => Resources["customer"] as Customer; set => Resources["customer"] = value; }
        public CustomerViewTab(int id)
        {
            InitializeComponent();
            BLCustomer = MainWindow.BL.DisplayCustomer(id);
            
            UpdateButton.GotFocus += UpdateButton_Click;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.BL.UpdateCustomer(BLCustomer.Id, CustomerName.Text,CustomerPhone.Text);
                BLCustomer = MainWindow.BL.DisplayCustomer(BLCustomer.Id);
                
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focusable = true;
            ((DronesViewTab)((Grid)((Grid)Parent).Parent).Parent).Focus();
        }
    }
}
