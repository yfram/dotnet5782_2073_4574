using BlApi;
using BO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class CustomersViewTab : UserControl
    {

        public ObservableCollection<CustomerForList> CustomersView { get; set; } = new();
        private static IBL Bl => BlFactory.GetBl();

        private bool gridOpen = false;

        public CustomersViewTab()
        {
            Bl.GetAllCustomers().ToList().ForEach(d => CustomersView.Add(d));
            InitializeComponent();
        }

        public void CollapsePullUp()
        {
            if (!gridOpen)
            {
                return;
            }

            PullUpMenueContainer.Collapse(250);
            RefreshBl();
            gridOpen = false;
        }

        public void RefreshBl()
        {
            CustomersView.Clear();
            Bl.GetAllCustomers().ToList().ForEach(c => CustomersView.Add(c));
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            PullUpMenueContainer.Children.Add(new AddCustomerTab());
            PullUpMenueContainer.Expand(250, 150);
            gridOpen = true;
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            PullUpMenueContainer.Children.Add(new CustomerViewTab(CustomersView[((DataGridRow)sender).GetIndex()].Id));
            PullUpMenueContainer.Expand(250, 150);
            gridOpen = true;
        }
    }
}
