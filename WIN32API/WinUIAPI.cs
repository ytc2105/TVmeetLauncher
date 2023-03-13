using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Navigation;

namespace WIN32API
{
    public static class WinUIAPI
    {
        #region パラメーター
        /// <summary>
        /// ShowWindowコマンド、ウィンドウ状態変更
        /// </summary>
        private enum SW : uint
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
        private enum Zhwnd : int
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }

        /// <summary>
        /// ウィンドウのサイズと位置のフラグ
        /// </summary>
        private enum SWPos : uint
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
        private enum ABMsg : int
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
        private struct APPBARDATA
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
        private enum ABEdge : int
        {
            ABE_LEFT = 0,
            ABE_TOP = 1,
            ABE_RIGHT = 2,
            ABE_BOTTOM = 3
        }

        /// <summary>
        /// ABState
        /// </summary>
        private enum ABState : int
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
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion

        #region ウィンドウ表示/非表示関連
        [DllImport("USER32.DLL")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("USER32.DLL")]
        private static extern IntPtr ShowWindow(IntPtr hWnd, uint nCmdShow);
        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("USER32.DLL")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);
        [DllImport("SHELL32.DLL")]
        private static extern int SHAppBarMessage(ABMsg dwMessage, ref APPBARDATA pData);

        private static IntPtr taskBarHwnd = FindWindow("Shell_TrayWnd", null);
        private static IntPtr startMenuHwnd = FindWindow("Windows.UI.Core.CoreWindow", "スタート");

        /// <summary>
        /// タスクバーを非表示にする
        /// </summary>
        public static void TskBarHide()
        {
            ShowWindow(taskBarHwnd, (uint)SW.SW_HIDE);
            ShowWindow(startMenuHwnd, (uint)SW.SW_HIDE);
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
        /// タスクバーのHwndを再設定
        /// </summary>
        public static void ResetHwnd()
        {
            taskBarHwnd = IntPtr.Zero;
            startMenuHwnd = IntPtr.Zero;
            for (int i = 0; i < 10 && 
                    (taskBarHwnd == IntPtr.Zero || startMenuHwnd == IntPtr.Zero); i++)
            {
                taskBarHwnd = FindWindow("Shell_TrayWnd", null);
                startMenuHwnd = FindWindow("Windows.UI.Core.CoreWindow", "スタート");
                Thread.Sleep(200);
            }
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
        #endregion

        #region Windowハンドル関連
        private const int GW_HWNDNEXT = 2;
        [DllImport("USER32.DLL")]
        private extern static IntPtr GetParent(IntPtr hwnd);
        [DllImport("USER32.DLL")]
        private extern static IntPtr GetWindow(IntPtr hwnd, int wCmd);
        [DllImport("USER32.DLL")]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        /// <summary>
        /// プロセスID(pid)をウィンドウハンドル(hwnd)に変換する
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static IntPtr GetHwndFromPid(int pid)
        {
            IntPtr hwnd = FindWindow(null, null);
            while (hwnd != IntPtr.Zero)
            {
                if (GetParent(hwnd) == IntPtr.Zero && IsWindowVisible(hwnd))
                {
                    if (pid == GetPidFromHwnd(hwnd))
                    {
                        return hwnd;
                    }
                }
                hwnd = GetWindow(hwnd, GW_HWNDNEXT);
            }
            return hwnd;
        }

        /// <summary>
        /// ウィンドウハンドル(hwnd)をプロセスID(pid)に変換する
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static int GetPidFromHwnd(IntPtr hwnd)
        {
            GetWindowThreadProcessId(hwnd, out int pid);
            return pid;
        }
        #endregion

        #region ウィンドウ探索関連
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        [DllImport("USER32.DLL")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);
        [DllImport("USER32.DLL")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// ウィンドウハンドルからタイトル取得
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static string GetTitleFromHwnd(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// ウィンドウハンドルからクラス名取得
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        public static string GetClassNameFromHwnd(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            GetClassName(hWnd, sb, sb.Capacity);
            return sb.ToString();
        }

        /// <summary>
        /// タイトルにテキストを含むhWndを取得する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static List<IntPtr> FindWindowsWithText(string text)
        {
            List<IntPtr> windows = new List<IntPtr>();
            EnumWindows(delegate (IntPtr hWnd, IntPtr lParam)
            {
                string title = GetTitleFromHwnd(hWnd);
                if (title.Contains(text))
                {
                    windows.Add(hWnd);
                }
                return true;
            }, IntPtr.Zero);

            return windows;
        }
        #endregion

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
