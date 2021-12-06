using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using IBL.BO;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for DronesViewTab.xaml
    /// </summary>
    public partial class DronesViewTab : UserControl
    {
        public List<Drone> Drones { get; set; } = new();
        public DronesViewTab()
        {
            MainWindow.BL.DisplayDrones().ToList().ForEach(d => Drones.Add(MainWindow.BL.DisplayDrone(d.Id)));
            InitializeComponent();
        }
    }
}
