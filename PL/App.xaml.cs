// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ChangeTheme("");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            key = key.CreateSubKey("Drones Manager", true);
            key = key.CreateSubKey($"Version {}");
            base.OnExit(e);
        }

        private void ChangeTheme(string themePath)
        {
            Resources.MergedDictionaries.RemoveAt(Resources.MergedDictionaries.Count - 1);
            Resources.MergedDictionaries.Add(new ResourceDictionary()
            { Source = new Uri($@"/Assets/Themes/{themePath}Theme.xaml", UriKind.Relative) });
        }
    }
}
