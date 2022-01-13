// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Collections.Generic;
using System.IO;
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
using System.Xml.Linq;

namespace PL
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : UserControl
    {
        public SettingsWindow()
        {
            InitializeComponent();
            Directory.EnumerateFiles(@"Pl/Assets/Themes").ToList().ForEach(f =>
                Options.Items.Add(f.Replace("Theme.xaml", "").Remove(0, f.LastIndexOf('\\') + 1)));
            Options.SelectedIndex = Options.Items.IndexOf(((App)Application.Current).CurrentTheme);


        }

        private void changeDal(object sender, RoutedEventArgs e)
        {
            XElement dalConfig = XElement.Load(@"xml\dal-config.xml");
            string current = dalConfig.Element("dal").Value;
            dalConfig.Element("dal").Value = current == "list" ? "xml" : "list";

            dalConfig.Save(@"xml\dal-config.xml");

            BlApi.BlFactory.Init();

            ((MainWindow)Application.Current.MainWindow).RefreshAll();
        }

        private void ApplyChanges(object sender, RoutedEventArgs e)
        {
            ((Grid)Parent).Visibility = Visibility.Collapsed;
            ((App)Application.Current).ChangeTheme(Options.SelectedItem as string ?? "Dark");
        }
    }
}
