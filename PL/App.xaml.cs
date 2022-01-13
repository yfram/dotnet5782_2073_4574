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
        internal string CurrentTheme = "Dark";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            if (File.Exists(".userSettings"))
                CurrentTheme = File.ReadAllLines(".usersettings").
                    Where(l => l.StartsWith("default_theme="))
                    .First().Replace("default_theme=", "");
            ChangeTheme(CurrentTheme);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (CurrentTheme != "Default")
                File.WriteAllTextAsync(".usersettings", $"default_theme={CurrentTheme}\n");
            base.OnExit(e);
        }

        public void ChangeTheme(string themePath)
        {
            CurrentTheme = themePath;
            Resources.MergedDictionaries.Add(new ResourceDictionary()
            { Source = new Uri($@"/Assets/Themes/{themePath}Theme.xaml", UriKind.Relative) });
        }
    }
}
