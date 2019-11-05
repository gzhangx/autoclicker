using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoClicker
{
    public class KeyboardHook: IDisposable
    {
        private IntPtr hookId = IntPtr.Zero;
        private Win32.HookProc cbDel;

        private Action<Key> OnKeyPress;
        public KeyboardHook(Action<Key> cb)
        {
            OnKeyPress = cb;
            cbDel = new Win32.HookProc((nCode, wParam, lParam)=>
            {
                if (nCode >= 0 && wParam == (IntPtr)Win32.WM_KEYDOWN)
                {
                    int vkCode = Marshal.ReadInt32(lParam);
                    var keyPressed = KeyInterop.KeyFromVirtualKey(vkCode);
                    if (OnKeyPress != null)
                    {
                        OnKeyPress(keyPressed);
                    }
                }
                return Win32.CallNextHookEx(hookId, nCode, wParam, lParam);
            });
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                hookId = Win32.SetWindowsHookEx(Win32.WH_KEYBOARD_LL, cbDel,
                                        Win32.GetModuleHandle(curModule.ModuleName), 0);
            }
        }        

        public void Dispose()
        {
            Win32.UnhookWindowsHookEx(hookId);
        }


    }
}
