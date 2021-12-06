using IBL.BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class DronesViewTab : UserControl
    {
        public List<Drone> Drones { get; set; } = new();

        List<Drone> drones;

        public DronesViewTab()
        {
            MainWindow.BL.DisplayDrones().ToList().ForEach(d => Drones.Add(MainWindow.BL.DisplayDrone(d.Id)));
            drones = Drones.Where(d => true).ToList();
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is not CheckBox senderAsCheckBox) return;
            List<Func<Drone, bool>> funcs = new();
            foreach (CheckBox checkBox in FilterGrid.Children.OfType<CheckBox>())
                switch (checkBox.Content)
                {
                    case "Empty":
                    case "Maitenance":
                    case "Busy":
                        if ((checkBox.IsChecked ?? false))//false is unreacable
                            funcs.Add((Drone d) => d.State.ToString() == (string)checkBox.Content);
                        break;
                    case "Light":
                    case "Mid":
                    case "Heavy":
                        if ((checkBox.IsChecked ?? false))//false is unreacable
                            funcs.Add((Drone d) => d.Weight.ToString() == (string)checkBox.Content);
                        break;
                    default:
                        throw new AccessViolationException();
                }
            Drones.Clear();
            if (funcs.Count == 0)
                Drones = drones.Where(d => true).ToList();
            foreach (Func<Drone, bool> func in funcs)
                Drones.AddRange(drones.Where(func).Except(Drones));
            DroneGrid.ItemsSource = Drones;
            DroneGrid.Items.Refresh();
        }
    }
}
