// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using BlApi;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Logging;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.Utilities;
using BO;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for HomeViewTab.xaml
    /// </summary>
    public partial class HomeViewTab : UserControl
    {
        private static Dictionary<string, int> Cache = new()
        {
            [@"Pl/Assets/drone.png"] = 0,
            [@"Pl/Assets/station.png"] = 1,
            [@"Pl/Assets/customer.png"] = 2,
        };
        private static IBL Bl => BlFactory.GetBl();

        public HomeViewTab()
        {
            InitializeComponent();
            var map = new Map();
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            var centerOfJerusalem = new Point(34.78309563131772, 31.38728300557323);
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(centerOfJerusalem.X, centerOfJerusalem.Y).AsPoint();
            map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, 1000, easing: Easing.SpringIn);
            MyMapControl.Map = map;
            RefreshBl();
        }

        private ILayer GetMarkerLayer(IEnumerable<dynamic> objects, double scale)
        {
            List<Point> points = new();
            var ly = new WritableLayer();
            Point pt;
            Feature feature;
            foreach (dynamic o in objects)
            {
                Location loc = o.GetType().Name switch
                {
                    "DroneForList" => (o as DroneForList).CurrentLocation,
                    "CustomerForList" => Bl.GetCustomerById((o as CustomerForList).Id).CustomerLocation,
                    "StationForList" => Bl.GetStationById((o as StationForList).Id).LocationOfStation,
                    _ => throw new InvalidCastException()
                };
                pt = SphericalMercator.FromLonLat(loc.Longitude, loc.Latitude);
                feature = new Feature
                {
                    Geometry = pt,
                    Styles = new Style[]
                    {
                        CreateSymbolStyle(o.GetType().Name.ToLower().Replace("forlist",""),scale),
                        new LabelStyle
                        {
                            Text = o.Id.ToString()
                        }
                    }
                };
                points.Add(pt);
                ly.Add(feature);
            }
            return ly;
        }

        private static SymbolStyle CreateSymbolStyle(string name, double scale) => new()
        {
            BitmapId = GetBitmapIdForEmbeddedResource(@$"Pl/Assets/{name}.png"),
            SymbolScale = scale
        };

        private static int GetBitmapIdForEmbeddedResource(string imagePath)
        {
            int val;
            Cache.TryGetValue(imagePath, out val);
            using (FileStream fs = new FileStream(imagePath, FileMode.Open))
            {
                var memoryStream = new MemoryStream();
                fs.CopyTo(memoryStream);
                var bitmapId = BitmapRegistry.Instance.Register(memoryStream);
                return bitmapId;
            }
        }

        public UserControl RefreshBl()
        {
            MyMapControl.Map.Layers.Add(GetMarkerLayer(Bl.GetAllCustomers(), 0.1));
            MyMapControl.Map.Layers.Add(GetMarkerLayer(Bl.GetAllDrones(), 0.1));
            MyMapControl.Map.Layers.Add(GetMarkerLayer(Bl.GetAllStations(), 0.075));
            MyMapControl.Refresh();
            return this;
        }

        internal HomeViewTab RefreshBl()
        {
            return this;
        }
    }
}
