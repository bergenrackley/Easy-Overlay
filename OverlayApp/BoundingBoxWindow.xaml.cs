using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Application = System.Windows.Application;
using Brushes = System.Windows.Media.Brushes;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using KeyEventHandler = System.Windows.Input.KeyEventHandler;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Path = System.IO.Path;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace OverlayApp
{
    public partial class BoundingBoxWindow : Window
    {
        private Point _startPoint = new Point();
        private Rectangle _rect;

        public BoundingBoxWindow()
        {
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(OnKeyDownHandler);
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            DrawingCanvas.Children.Clear();

            _startPoint = e.GetPosition(DrawingCanvas);

            _rect = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(_rect, _startPoint.X);
            Canvas.SetTop(_rect, _startPoint.Y);
            DrawingCanvas.Children.Add(_rect);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(DrawingCanvas);

                var x = Math.Min(pos.X, _startPoint.X);
                var y = Math.Min(pos.Y, _startPoint.Y);

                var w = Math.Max(pos.X, _startPoint.X) - x;
                var h = Math.Max(pos.Y, _startPoint.Y) - y;

                _rect.Width = w;
                _rect.Height = h;

                Canvas.SetLeft(_rect, x);
                Canvas.SetTop(_rect, y);
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var endPoint = e.GetPosition(DrawingCanvas);
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _startPoint != new Point())
            {
                SettingsManager settings = new SettingsManager();
                settings.Settings.BoundingBox = new BoundingBox
                {
                    StartX = _startPoint.X,
                    StartY = _startPoint.Y,
                    EndX = Canvas.GetLeft(_rect) + _rect.Width,
                    EndY = Canvas.GetTop(_rect) + _rect.Height
                };
                settings.SaveSettings();

                MessageBox.Show("Bounding box coordinates saved to settings.json");

                this.Close();
            }
        }
    }
}
