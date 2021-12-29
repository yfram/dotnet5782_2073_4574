using BO;
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
    /// Interaction logic for PackageForCustomer.xaml
    /// </summary>
    public partial class PackageForCustomerViewTab : UserControl
    {
        private PackageForCustomer BLpackage { get => Resources["package"] as PackageForCustomer; set => Resources["package"] = value; }

        public PackageForCustomerViewTab(PackageForCustomer p)
        {
            InitializeComponent();
            BLpackage = p;
        }
    }
}
