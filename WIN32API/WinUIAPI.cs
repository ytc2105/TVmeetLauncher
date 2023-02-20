﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WIN32API
{
    public static class WinUIAPI
    {
        //private static readonly int SW_SHOW = 5;
        //private static readonly int SW_HIDE = 0;

        /// <summary>
        /// ShowWindowコマンド、ウィンドウ状態変更
        /// </summary>
        public enum SW : uint
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11,
        }

        /// <summary>
        /// Z オーダーで配置されたウィンドウの前にあるウィンドウへのハンドル
        /// </summary>
        public enum Zhwnd : int
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }

        /// <summary>
        /// ウィンドウのサイズと位置のフラグ
        /// </summary>
        public enum SWPos : uint
        {
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040
        };

        /// <summary>
        /// ABMsg 送るAppBarメッセージの識別子（以下のいずれか1つ） 
        /// ・ABM_ACTIVATE---AppBarがアクティブになった事をシステムに通知
        /// ・ABM_GETAUTOHIDEBAR---スクリーンの特定の端に関連付けられているオートハイドAppBarのハンドルを返す
        /// ・ABM_GETSTATE---タスクバーがオートハイドか常に最前面のどちらの常態にあるかを返す
        /// ・ABM_GETTASKBARPOS---タスクバーの使用領域を返す
        /// ・ABM_NEW---新しいAppBarを登録し、システムが通知に使用するメッセージIDを指定する
        /// ・ABM_QUERYPOS---AppBarのためのサイズとスクリーン位置を要求する
        /// ・ABM_REMOVE---AppBarの登録を削除する
        /// ・ABM_SETAUTOHIDEBAR---スクリーンの端にオートハイドAppBarを登録または削除する
        /// ・ABM_SETPOS---AppBarのサイズとスクリーン座標を設定する
        /// ・ABM_WINDOWPOSCHANGED---AppBarの位置が変更されたことをシステムに通知する
        /// pData： TAppBarData構造体（各フィールドはdwMessageに依存する） 
        /// </summary>
        public enum ABMsg : int
        {
            ABM_NEW = 0,
            ABM_REMOVE = 1,
            ABM_QUERYPOS = 2,
            ABM_SETPOS = 3,
            ABM_GETSTATE = 4,
            ABM_GETTASKBARPOS = 5,
            ABM_ACTIVATE = 6,
            ABM_GETAUTOHIDEBAR = 7,
            ABM_SETAUTOHIDEBAR = 8,
            ABM_WINDOWPOSCHANGED = 9,
            ABM_SETSTATE = 10
        }

        /// <summary>
        /// APPBARDATA SHAppBarMessage関数にて使用されるAppBarに関する構造体。
        /// cbSize.....SizeOf(TAppBarData)
        /// hWnd.....AppBarのハンドル
        /// uCallbackMessage.....任意のメッセージID（hWndのAppBarにメッセージを通知する際（ABM_NEWメッセージを送る際）に使用）
        /// uEdge.....スクリーンの端を指定するフラグ（ABM_GETAUTOHIDEBAR、ABM_QUERYPOS、ABM_SETAUTOHIDEBAR、ABM_SETPOSメッセージを送る際に使用し、以下のいずれか1つ）
        /// ・ABE_BOTTOM---下サイド
        /// ・ABE_LEFT--- 左サイド
        /// ・ABE_RIGHT---右サイド
        /// ・ABE_TOP---上サイド
        /// rc.....AppBarやタスクバーのスクリーン座標での表示領域（ABM_GETTASKBARPOS、ABM_QUERYPOS、ABM_SETPOSメッセージを送る際に使用する）
        /// lParam.....メッセージ依存のパラメータ（ABM_SETAUTOHIDEBARメッセージと共に使用される）
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public uint uCallbackMessage;
            public ABEdge uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        /// <summary>
        /// ABEdge
        /// </summary>
        public enum ABEdge : int
        {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3
        }

        /// <summary>
        /// ABState
        /// </summary>
        enum ABState : int
        {
            ABS_MANUAL = 0,
            ABS_AUTOHIDE = 1,
            ABS_ALWAYSONTOP = 2,
            ABS_AUTOHIDEANDONTOP = 3,
        }

        /// <summary>
        /// RECT
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("USER32.DLL")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        private static extern IntPtr ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);

        [DllImport("SHELL32.DLL")]
        public static extern int SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

        private static readonly IntPtr taskBarHwnd = FindWindow("Shell_TrayWnd", null);
        private static readonly IntPtr startMenuHwnd = FindWindow("Windows.UI.Core.CoreWindow", "スタート");

        /// <summary>
        /// タスクバーを非表示にする
        /// </summary>
        public static void TskBarHide()
        {
            SetWindowPos(taskBarHwnd, (IntPtr)Zhwnd.HWND_BOTTOM, 0, 0, 0, 0,
                (uint)(SWPos.SWP_HIDEWINDOW));
            ShowWindow(startMenuHwnd, (uint)SW.SW_HIDE);

            //StringBuilder startMenuName = new StringBuilder(1024);
            //int res = GetClassName(startMenuHwnd, startMenuName, startMenuName.Capacity);
        }
        /// <summary>
        /// タスクバーを表示する
        /// </summary>
        public static void TskBarDisp()
        {
            ShowWindow(taskBarHwnd, (uint)SW.SW_SHOW);
            ShowWindow(startMenuHwnd, (uint)SW.SW_SHOW);
        }

        /// <summary>
        /// タスクバーを自動で隠す
        /// </summary>
        /// <param name="isAutoHide">true:自動で隠す false:常に表示</param>
        public static void TskBarAutoHide(bool isAutoHide)
        {
            APPBARDATA pData = new APPBARDATA();
            pData.cbSize = Marshal.SizeOf(pData);
            pData.hWnd = taskBarHwnd;
            //pData.uEdge = ABEdge.ABE_LEFT;
            //SHAppBarMessage(ABMsg.ABM_GETSTATE, ref pData);
            if (isAutoHide)  // タスクバーを自動で隠す
            {
                pData.lParam = (IntPtr)ABState.ABS_AUTOHIDE;
            }
            else // タスクバーを常に表示
            {
                pData.lParam = (IntPtr)ABState.ABS_MANUAL;
            }
            //タスクバーにメッセージ送信
            SHAppBarMessage(ABMsg.ABM_SETSTATE, ref pData);
        }

        /// <summary>
        /// タスクバー非表示状態取得
        /// </summary>
        /// <returns>true:非表示 false:表示</returns>
        public static bool IsTaskbarHide()
        {
            bool isTaskbarHide = !IsWindowVisible(taskBarHwnd);

            return isTaskbarHide; 
        }

        /// <summary>
        /// タスクバーを自動的に隠す状態取得
        /// </summary>
        /// <returns>true:自動的に隠す false:それ以外</returns>
        public static bool IsTaskbarAutoHide()
        {
            bool isTaskbarAutoHide = false;

            APPBARDATA pData = new APPBARDATA();
            uint uState = (uint)SHAppBarMessage(ABMsg.ABM_GETSTATE, ref pData);

            switch (uState)
            {
                case (int)ABState.ABS_ALWAYSONTOP:
                    isTaskbarAutoHide = false;
                    break;
                case (int)ABState.ABS_AUTOHIDE:
                    isTaskbarAutoHide = true;
                    break;
                case (int)ABState.ABS_AUTOHIDEANDONTOP:
                    isTaskbarAutoHide = true;
                    break;
                case (int)ABState.ABS_MANUAL:
                    isTaskbarAutoHide = false;
                    break;
            }

            return isTaskbarAutoHide;
        }

        /// <summary>
        /// Windowを最前面へ移動する
        /// </summary>
        /// <param name="hWnd"></param>
        public static void MoveWindowToTop(IntPtr hWnd) 
        {
            ShowWindow(hWnd, (uint)SW.SW_RESTORE);
            SetWindowPos(hWnd, (IntPtr)Zhwnd.HWND_TOP, 0, 0, 0, 0, 
                (uint)SWPos.SWP_NOMOVE | (uint)SWPos.SWP_NOSIZE);
        }
    }

    public static class Psapi
    {
        [DllImport("psapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcesses(uint[] lpidProcesses, uint cb, out uint lpcbNeeded);

        [DllImport("psapi.dll")]
        public static extern uint GetModuleFileNameEx(IntPtr hProcess, IntPtr hModule, StringBuilder lpFilename, uint nSize);

        [DllImport("psapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumProcessModulesEx(IntPtr hProcess, IntPtr[] lphModule, int cb,
            out uint lpcbNeeded, [MarshalAs(UnmanagedType.I4)] ListModules dwFilterFlag);

        public enum ListModules : int
        {
            ListModules32Bit = 0x01,
            ListModules64Bit = 0x02,
            ListModulesAll = 0x03,
            ListModulesDefault = 0x0,
        }

        //public static bool GetWindowFileName(IntPtr hWnd, string lpFileName, uint nSize)
        //{
        //    uint processID;
        //    GetWindowThreadProcessId(hWnd, ref processID);
        //    IntPtr hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, processID);
        //    bool ret = false;
        //    if (hProcess != null)
        //    {
        //        uint cbReturned;
        //        ret = 0 != GetModuleFileNameEx(hProcess, null, lpFileName, nSize);
        //        CloseHandle(hProcess);
        //    }

        //    return ret;
        //}
    }

    public static class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess([MarshalAs(UnmanagedType.I4)] ProcessSecurity dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle,
            uint dwProcessId);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        public enum ProcessSecurity : uint
        {
            ProcessVmRead = 0x0010,
            ProcessQueryInformation = 0x0400,
        }
    }
}