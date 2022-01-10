using BO;
using System.Windows.Controls;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for PackageForCustomer.xaml
    /// </summary>
    public partial class CustomerForPackageWindow : UserControl
    {
        private CustomerForPackage BLcustomer { get => Resources["customer"] as CustomerForPackage; set => Resources["customer"] = value; }

        public CustomerForPackageWindow(CustomerForPackage p)
        {
            InitializeComponent();
            BLcustomer = p;
        }
    }
}
