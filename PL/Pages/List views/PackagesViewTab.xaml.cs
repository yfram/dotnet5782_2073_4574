// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi;
using BO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for PackagesViewTab.xaml
    /// </summary>
    public partial class PackagesViewTab : UserControl
    {

        public ObservableCollection<PackageForList> PackagesView { get; set; }

        private IBL Bl = BlFactory.GetBl();

        private bool gridOpen = false;
        private bool packageView = false;

        public PackagesViewTab()
        {
            PackagesView = new(Bl.GetAllPackages());
            InitializeComponent();
        }

        public void CollapsePullUp()
        {
            if (!gridOpen)
            {
                return;
            }

            gridOpen = false;
            PullUpContainer.Collapse(250);
            PullUpContainer.Children.Clear();
            RefreshBl();
        }

        public void RefreshBl()
        {
            PackagesView.Clear();
            Bl.GetAllPackages().ToList().ForEach(p => PackagesView.Add(p));
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            PullUpContainer.Children.Add(new AddPackageTab());
            PullUpContainer.Expand(250, 150);
            gridOpen = true;
        }

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (packageView)
            {
                return;
            }

            PackageForList p = ((DataGridRow)sender).DataContext as PackageForList ?? throw new();
            PullUpContainer.Children.Add(new PackageViewTab(p.Id));
            PullUpContainer.Expand(250, 150);
            gridOpen = true;
        }

        private void Filter(object sender, RoutedEventArgs e)
        {
            PackagesView.Clear();
            Bl.GetAllPackagesWhere(p =>
            FilterDate().Any(pI => pI.Id == p.Id) &&
            FilterState().Any(pI => pI.Id == p.Id)).ToList().ForEach(p => PackagesView.Add(p));
        }

        private IEnumerable<PackageForList> FilterState()
        {
            if (!(FilterByState.IsChecked ?? false))
            {
                return Bl.GetAllPackages();
            }

            string check = ((ComboBoxItem)StateFilter.SelectedItem).Content.ToString().Replace(" ", "");
            return Bl.GetObjectsWhere<PackageForList>(p => p.Status.ToString().ToLower() == check).
                Cast<PackageForList>();
        }

        private IEnumerable<PackageForList> FilterDate()
        {
            if (StartDate.Value is null || EndDate.Value is null)
            {
                return Bl.GetAllPackages();
            }

            if (!(FilterByDate.IsChecked ?? false))
            {
                return Bl.GetAllPackages();
            }

            List<PackageForList> datePackages =
                Bl.GetObjectsWhere<Package>(p => p.TimePickedUp >= StartDate.Value && p.TimePickedUp <= EndDate.Value).
                Cast<PackageForList>().ToList();
            return datePackages;
        }

        private void Collected_view(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox senderAsCheckBox)
            {
                return;
            }
            //because of the order of the && operator this works
            if ((string)(senderAsCheckBox.Tag) == "Sender" && CollectedReciver.IsChecked.Value && CollectedSender.IsChecked.Value)
            {
                CollectedReciver.IsChecked = !senderAsCheckBox.IsChecked;
            }
            else if (CollectedReciver.IsChecked.Value && CollectedSender.IsChecked.Value)
            {
                CollectedSender.IsChecked = !senderAsCheckBox.IsChecked;
            }

            var packageView = (CollectionViewSource)Resources["PackagesGroup"];
            packageView.GroupDescriptions.Clear();
            if (senderAsCheckBox.IsChecked.Value)
            {
                packageView.GroupDescriptions.Add(new PropertyGroupDescription(senderAsCheckBox.Tag == "Receiver" ?
                    "NameOfReciver" : "NameOfSender"));
            }

        }
    }
}
