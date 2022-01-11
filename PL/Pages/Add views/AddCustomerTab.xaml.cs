using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for AddCustomerTab.xaml
    /// </summary>
    public partial class AddCustomerTab : UserControl
    {
        private IBL Bl { get => BlFactory.GetBl(); }

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
                    new(Double.Parse(NewLong.Text), Double.Parse(NewLat.Text)),
                    new List<BO.PackageForCustomer>(), new List<BO.PackageForCustomer>()));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Exit();
        }
    }
}
