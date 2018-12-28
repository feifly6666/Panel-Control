using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;
using System.Collections;

namespace CombinationKeySet
{
    public class Call_MT70_DLL
    {
        [DllImport("MT70_SDK.DLL", EntryPoint = "EnableTouchPanel")]
        public static extern bool EnableTouchPanel(bool flag);
    }

    class KeyboardHook
    {
        int hHook;

        CallNlHookEx.HookProc KeyboardHookDelegate;

        //public event KeyEventHandler OnKeyDownEvent;

        //public event KeyEventHandler OnKeyUpEvent;

        public KeyboardHook() { }

        public void SetHook()
        {
            KeyboardHookDelegate = new CallNlHookEx.HookProc(KeyboardHookProc);
            //Process cProcess = Process.GetCurrentProcess();

            //ProcessModule cModule = cProcess.MainModule;

            //var mh = CallNlHookEx.GetModuleHandle(cProcess.MainModule.ModuleName);
            var mh = CallNlHookEx.GetModuleHandle(null);
            hHook = CallNlHookEx.SetWindowsHookEx(CallNlHookEx.WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
            if (hHook == 0)
            {
                throw new SystemException("Failed acquiring of the hook.");
            }

        }

        public void UnHook()
        {

            CallNlHookEx.UnhookWindowsHookEx(hHook);

        }


        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //如果该消息被丢弃（nCode<0）或者没有事件绑定处理程序则不会触发事件
            
            if ((nCode >= 0) && (wParam == CallNlHookEx.WM_KEYDOWN || wParam == CallNlHookEx.WM_SYSKEYDOWN))
            {

                CallNlHookEx.KeyboardHookStruct KeyDataFromHook = (CallNlHookEx.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(CallNlHookEx.KeyboardHookStruct));

                if (Form1.hashTable.Contains(KeyDataFromHook.vkCode))
                {
                    int panelEnableFlg = (int)Form1.hashTable[KeyDataFromHook.vkCode];
                    if (panelEnableFlg == 1){
                        Call_MT70_DLL.EnableTouchPanel(true);
                    }
                    if (panelEnableFlg == 2)
                    {
                        Call_MT70_DLL.EnableTouchPanel(false);
                    }
                }

            }

            return CallNlHookEx.CallNextHookEx(hHook, nCode, wParam, lParam);
            //return 0;

        }

    }
}
