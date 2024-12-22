using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace OverlayApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DefineBoundingBoxButton.Click += DefineBoundingBoxButton_Click;
            DisplayOverlayButton.Click += DisplayOverlayButton_Click;
            SaveSettingsButton.Click += SaveSettingsButton_Click;

            SettingsManager settings = new SettingsManager();
            MonitorScaleTextBox.Text = settings.Settings.MonitorScale.ToString();
            OpacitySlider.Value = settings.Settings.Opacity;
        }

        private void DefineBoundingBoxButton_Click(object sender, RoutedEventArgs e)
        {
            var boundingBoxWindow = new BoundingBoxWindow();
            this.WindowState = WindowState.Minimized;
            boundingBoxWindow.ShowDialog();
        }

        private void DisplayOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager settings = new SettingsManager();

            double scale = (double)settings.Settings.MonitorScale / 100.0;
            int startX = (int)(settings.Settings.BoundingBox.StartX * scale);
            int startY = (int)(settings.Settings.BoundingBox.StartY * scale);
            int width = (int)((settings.Settings.BoundingBox.EndX * scale) - startX);
            int height = (int)((settings.Settings.BoundingBox.EndY * scale) - startY);
            int opacity = settings.Settings.Opacity;

            var overlayWindow = new OverlayWindow(startX, startY, width, height, opacity);
            overlayWindow.Show();
            this.WindowState = WindowState.Minimized;
        }

        private void MonitorScaleTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return !Regex.IsMatch(text, "[^0-9]+");
        }

        private void SaveSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsManager settings = new SettingsManager();
            settings.Settings.MonitorScale = Int32.Parse(MonitorScaleTextBox.Text);
            settings.Settings.Opacity = (int)OpacitySlider.Value;
            settings.SaveSettings();
        }
    }
}
