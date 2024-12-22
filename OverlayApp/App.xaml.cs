using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Application = System.Windows.Application;
using Window = System.Windows.Window;

namespace OverlayApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private System.Windows.Forms.NotifyIcon? _notifyIcon;
        private bool _isExit;
        private const int WM_HOTKEY = 0x0312; 
        private const uint MOD_SHIFT = 0x0004;
        private const uint VK_ESCAPE = 0x1B; 
        private const int HOTKEY_ID_SHIFT_ESC = 9001; 
        private HwndSource _source;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow = new MainWindow();
            MainWindow.Show();
            MainWindow.Hide();
            MainWindow.Closing += MainWindow_Closing;

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            _notifyIcon.DoubleClick += (s, args) => ShowMainWindow();
            _notifyIcon.Icon = new System.Drawing.Icon("Assets/AppIcon.ico");
            _notifyIcon.Visible = true;

            CreateContextMenu();

            RegisterGlobalHotKey();
        }

        private void RegisterGlobalHotKey() { 
            var helper = new WindowInteropHelper(MainWindow); 
            _source = HwndSource.FromHwnd(helper.Handle); 
            _source.AddHook(HwndHook); 
            RegisterHotKey(helper.Handle, HOTKEY_ID_SHIFT_ESC, MOD_SHIFT, VK_ESCAPE);
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                var id = wParam.ToInt32();
                if (id == HOTKEY_ID_SHIFT_ESC)
                {
                    HandleShiftEscPressed();
                }
                handled = true;
            }

            return IntPtr.Zero;
        }


        private void HandleShiftEscPressed()
        {
            MainWindow.Show();
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!(MainWindow.IsActive))
                {
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window != MainWindow)
                        {
                            window.Close();
                        }
                    }
                    MainWindow.WindowState = WindowState.Normal;
                    MainWindow.Activate();
                }
            });
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Easy Overlay").Click += (s, e) => ShowMainWindow();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            _isExit = true;
            var helper = new WindowInteropHelper(MainWindow);
            UnregisterHotKey(helper.Handle, HOTKEY_ID_SHIFT_ESC); _source.RemoveHook(HwndHook);
            _source = null;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void ShowMainWindow()
        {
            if (MainWindow.IsVisible)
            {
                if (MainWindow.WindowState == WindowState.Minimized)
                {
                    MainWindow.WindowState = WindowState.Normal;
                }
                MainWindow.Activate();
            }
            else
            {
                MainWindow.Show();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide();
            }
        }
    }

}