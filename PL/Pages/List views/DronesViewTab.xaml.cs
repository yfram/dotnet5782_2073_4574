using BO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class DronesViewTab : UserControl
    {

        public ObservableCollection<DroneForList> Drones { get; set; } = new(MainWindow.BL.GetAllDrones());

        private bool gridOpen = false;
        private bool packageViewInTheMiddle = false;

        public DronesViewTab()
        {
            InitializeComponent();
        }

        public void CollapsePullUp()
        {
            if (!gridOpen) return;
            PullUpMenueContainer.Collapse(250);
            RefreshBl();
            gridOpen = false;
        }

        public void RefreshBl()
        {
            Drones.Clear();
            MainWindow.BL.GetAllDrones().ToList().ForEach(d => Drones.Add(d));
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox senderAsCheckBox) return;

            List<Func<DroneForList, bool>> weightFuncs = new();
            List<Func<DroneForList, bool>> statusFuncs = new();
            foreach (CheckBox checkBox in FilterGrid.Children.OfType<CheckBox>())
            {
                switch (checkBox.Content)
                {
                    case "Empty":
                    case "Maitenance":
                    case "Busy":
                        if ((checkBox.IsChecked ?? false))//false is unreachable
                            statusFuncs.Add((DroneForList d) => d.State.ToString() == (string)checkBox.Content);
                        break;
                    case "Light":
                    case "Mid":
                    case "Heavy":
                        if ((checkBox.IsChecked ?? false))//false is unreachable
                            weightFuncs.Add((DroneForList d) => d.Weight.ToString() == (string)checkBox.Content);
                        break;
                    case "Collected_View":
                        break;
                    default:
                        throw new AccessViolationException();
                }
            }

            Drones.Clear();
            MainWindow.BL.GetAllDronesWhere(d => (weightFuncs.Any(f => f(d)) || weightFuncs.Count == 0) && (statusFuncs.Any(f => f(d)) || statusFuncs.Count == 0)).ToList().ForEach(elem => Drones.Add(elem));
        }

        private void Collected_view(object sender, RoutedEventArgs e)
        {
            var droneView = (CollectionViewSource)Resources["DronesGroup"];
            droneView.GroupDescriptions.Clear();
            if (((CheckBox)sender).IsChecked.HasValue && ((CheckBox)sender).IsChecked.Value)
                droneView.GroupDescriptions.Add(new PropertyGroupDescription("State"));
        }

        private void ShowMenue(int? id, string typeOfMenue)
        {
            gridOpen = true;
            PullUpMenueContainer.Expand(250, 150);
            UIElement menue = new();
            switch (typeOfMenue)
            {
                case "drone add":
                    menue = new AddDroneTab();
                    break;
                case "drone view":
                    menue = new DroneViewTab(id ?? -1);
                    break;
                case "package view":
                    menue = new PackageForDroneViewTab(id);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            PullUpMenueContainer.Children.Add(menue);
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e) => ShowMenue(null, "drone add");

        private void Row_DoubleClick(object sender, RoutedEventArgs e)
        {
            //this if allows to render the package view with out being overwritten
            if (packageViewInTheMiddle)
            {
                packageViewInTheMiddle = false;
                return;
            }
            ShowMenue(Drones[((DataGridRow)sender).GetIndex()].Id, "drone view");
        }

        private void PackageId_DoubleClicked(object sender, MouseButtonEventArgs e)
        {
            packageViewInTheMiddle = true;
            ShowMenue(Convert.ToInt32(((TextBlock)((DataGridCell)sender).Content).Text), "package view");
        }
    }
}
