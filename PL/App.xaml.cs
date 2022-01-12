// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string currentTheme = "Dark";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (File.Exists(".userSettings"))
                currentTheme = File.ReadAllLines(".usersettings").
                    Where(l => l.StartsWith("default_theme="))
                    .First().Replace("default_theme=", "");
            ChangeTheme(currentTheme);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (currentTheme != "Default")
                File.WriteAllText(".usersettings", $"default_theme={currentTheme}");
            base.OnExit(e);
        }

        private void ChangeTheme(string themePath)
        {
            currentTheme = themePath;
            Resources.MergedDictionaries.Add(new ResourceDictionary()
            { Source = new Uri($@"/Assets/Themes/{themePath}Theme.xaml", UriKind.Relative) });
        }
    }
}
