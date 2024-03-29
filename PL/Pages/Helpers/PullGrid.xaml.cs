﻿// File PullGrid.xaml.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace PL.Pages
{
    /// <summary>
    /// Interaction logic for PullGrid.xaml
    /// </summary>
    public partial class PullGrid : UserControl
    {
        public UIElementCollection Children => MainGrid.Children;

        public PullGrid()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Expands the grid to <paramref name="targetHeight"/> within <paramref name="time"/> ms
        /// </summary>
        /// <param name="time">In ms</param>
        public void Expand(int time, double targetHeight)
        {
            this.Height = targetHeight;
            DoubleAnimation myDoubleAnimation = new();
            myDoubleAnimation.From = 0;
            myDoubleAnimation.To = targetHeight;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(time));
            Storyboard.SetTargetName(myDoubleAnimation, "MainGrid");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(HeightProperty));
            Storyboard storyboard = new();

            storyboard.Children.Add(myDoubleAnimation);
            BeginStoryboard(storyboard);
        }

        /// <summary>
        /// Collapses within <paramref name="time"/> ms
        /// </summary>
        /// <param name="time">In ms</param>
        public void Collapse(int time)
        {
            DoubleAnimation myDoubleAnimation = new();
            myDoubleAnimation.From = this.ActualHeight;
            myDoubleAnimation.To = 0;
            myDoubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(time));
            Storyboard.SetTargetName(myDoubleAnimation, "MainGrid");
            Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(HeightProperty));
            Storyboard storyboard = new();

            storyboard.Children.Add(myDoubleAnimation);
            BeginStoryboard(storyboard);
            storyboard.Completed += (sender, e) => { this.Height = 0; MainGrid.Children.Clear(); };
        }
    }
}
