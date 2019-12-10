using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AutoClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String autoType = "Click";
        readonly KeyboardHook cb;

        bool Started = false;
        Thread actionThread;
        public MainWindow()
        {
            InitializeComponent();
            cmbType.Items.Add(new { Name = "Click"});
            cmbType.Items.Add(new { Name = "Space"});
            cmbType.SelectedIndex = 0;
            cb = new KeyboardHook(key =>
            {
                Console.WriteLine(key);
                if (key.ToString() == "J")
                {
                    DoStartStop();
                }
            });
        }

        private void cmbTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            autoType = (String)cmbType.SelectedValue;            
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            DoStartStop();
        }

        private void ForceStop()
        {
            if (Started)
            {
                DoStartStop();
            }
        }
        private void DoStartStop()
        {            
            Started = !Started;
            if (actionThread != null)
            {
                actionThread.Join();
                actionThread = null;
            }else
            {
                actionThread = new Thread(() =>
                {
                    while (Started)
                    {
                        if (autoType == "Click")
                        {
                            DoMouseClick();
                        }
                        else
                        {
                            DoKey(0x20);
                        }
                        Thread.Sleep(100);
                    }
                });
                actionThread.Start();
            }
            btnStart.Content = Started ? "Click To Stop" : "Click To Start";
        }


        private void DoMouseClick()
        {
            Win32.POINT pt;
            Win32.GetCursorPos(out pt);
            uint X = (uint)pt.X; // (uint)Cursor.Position.X;
            uint Y = (uint)pt.Y; // (uint)Cursor.Position.Y;                
            Win32.mouse_event(Win32.MOUSEEVENTF_LEFTDOWN | Win32.MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
        }

        private void DoKey(uint cd)
        {
            var vsc = (byte)Win32.MapVirtualKeyEx(cd, Win32.MAPVK_VK_TO_VSC, IntPtr.Zero);
            Win32.keybd_event(0x20, vsc, 0, UIntPtr.Zero);
            Win32.keybd_event(0x20, vsc, 0, UIntPtr.Zero);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ForceStop();
        }
    }
}
