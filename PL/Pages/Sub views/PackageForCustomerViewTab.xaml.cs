// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using BO;
using System.Windows.Controls;

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
