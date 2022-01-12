// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System.Windows.Controls;
using Mapsui.Utilities;
using Mapsui.Layers;
using Mapsui;
using Mapsui.Projection;
using Mapsui.Geometries;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for HomeViewTab.xaml
    /// </summary>
    public partial class HomeViewTab : UserControl
    {
        public HomeViewTab()
        {
            InitializeComponent();
            Map map = new Map();
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            // Get the lon lat coordinates from somewhere (Mapsui can not help you there)
            var centerOfJerusalem = new Point(34.78309563131772, 31.38728300557323);
            // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfJerusalem.X, centerOfJerusalem.Y).AsPoint();
            map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, 1000, easing: Easing.SpringIn);
            MyMapControl.Map = map;
        }
    }
}
