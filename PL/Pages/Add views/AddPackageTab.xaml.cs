using BlApi;
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
    /// Interaction logic for AddPackageTab.xaml
    /// </summary>
    public partial class AddPackageTab : UserControl
    {
        private IBL Bl { get => BlFactory.GetBl(); }

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
