using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoClick
{    
    class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }


        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        private static bool _started = false;
        static void Main(string[] args)
        {
            HotKeyManager.RegisterHotKey(System.Windows.Forms.Keys.J, 0);
            HotKeyManager.HotKeyPressed += HotKeyManager_HotKeyPressed;
            while (true)
            {
                POINT pt;
                GetCursorPos(out pt);
                uint X = (uint)pt.X; // (uint)Cursor.Position.X;
                uint Y = (uint)pt.Y; // (uint)Cursor.Position.Y;                
                if (_started)
                    mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
                Thread.Sleep(200);
            }
        }

        private static void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            _started = !_started;
            Console.WriteLine("invoked " + _started);
        }
    }
}
