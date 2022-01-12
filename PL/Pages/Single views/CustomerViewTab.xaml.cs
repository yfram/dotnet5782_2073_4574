using BlApi;
using BO;
using System;
using System.Linq;
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
        private static IBL Bl => BlFactory.GetBl();

        public CustomerViewTab(int id)
        {
            InitializeComponent();
            BLCustomer = Bl.GetCustomerById(id);
        }

        private void Exit(object sender = null, RoutedEventArgs e = null)
        {
            ((CustomersViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).CollapsePullUp();
        }

        private void RefreshParentBl()
        {
            ((CustomersViewTab)((Grid)((PullGrid)((Grid)Parent).Parent).Parent).Parent).RefreshBl();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Bl.UpdateCustomer(BLCustomer.Id, CustomerName.Text, CustomerPhone.Text);
                BLCustomer = Bl.GetCustomerById(BLCustomer.Id);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            RefreshParentBl();
        }

        private void OpenPackage(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(((TextBlock)((Button)sender).Content).Text);
            var l = BLCustomer.PackagesFrom.Where(p => p.Id == id);
            if (!l.Any())
            {
                l = BLCustomer.PackagesTo.Where(p => p.Id == id);
            }

            PackageForCustomer p = l.First();
            new Window
            {
                Content = new PackageForCustomerViewTab(p),
                Title = $"package {id}",
                SizeToContent = SizeToContent.WidthAndHeight,
                ResizeMode = ResizeMode.CanResize

            }.Show();
        }
    }
}
