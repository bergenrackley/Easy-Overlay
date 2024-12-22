using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace OverlayApp
{
    public partial class OverlayWindow : Window
    {
        private int StartX { get; set; }
        private int StartY { get; set; }
        private int Width { get; set; }
        private int Height { get; set; }
        private Bitmap bitmap;
        private BitmapImage bitmapImage;

        public OverlayWindow(int startX, int startY, int width, int height, int opacity)
        {
            InitializeComponent();

            this.IsHitTestVisible = false; 
            OverlayImage.IsHitTestVisible = false;

            StartX = startX;
            StartY = startY;
            Width = width;
            Height = height;
            OverlayImage.Opacity = (double)opacity / 100.0;

            StartImageStream();

            Loaded += OverlayWindow_Loaded;
        }

        private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }

        private void StartImageStream()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    CaptureScreen();
                    Dispatcher.Invoke(() =>
                    {
                        bitmapImage = ConvertToBitmapImage(bitmap);
                        OverlayImage.Source = bitmapImage;
                    });
                    Thread.Sleep(100);
                }
            });
        }

        private void CaptureScreen()
        {
            var screenBounds = new Rectangle(StartX, StartY, Width, Height);
            bitmap = new Bitmap(screenBounds.Width, screenBounds.Height);

            using (var g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(screenBounds.Location, System.Drawing.Point.Empty, screenBounds.Size);
            }
        }

        private BitmapImage ConvertToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        //private void OnKeyDownHandler(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Escape)
        //    {
        //        this.Close();
        //        Application.Current.MainWindow.WindowState = WindowState.Normal;
        //        Application.Current.MainWindow.Activate();
        //    }
        //}

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;
        private const int WS_EX_LAYERED = 0x00080000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
