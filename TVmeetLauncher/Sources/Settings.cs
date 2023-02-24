using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace TVmeetLauncher
{
    /// <summary>
    /// 定数クラス
    /// </summary>
    public static class ConstParams
    {
        /// <summary>
        /// 会議アプリの種類
        /// </summary>
        public enum MeetingApplication
        {
            [Description("Google Meet")]
            GoogleMeet,
            [Description("Zoom")]
            Zoom,
            [Description("Microsoft Teams")]
            MicrosoftTeams,
            [Description("Webex")]
            Webex,
            [Description("Skype")]
            Skype,
        }

        public static readonly int iconHeight = 150;
        public static readonly int iconWidth = 150;
        public static readonly int buttonMargin = 40;

        // 画面横解像度
        private static double _resolWidth = 3840.0; // 初期値 4K
        public static double ResolWidth { get { return _resolWidth; } set { _resolWidth = value; } }
        // Windows画面拡大縮小倍率
        private static double _winScale = 1.0; // 初期値　1倍(100%)
        public static double WinScale { get { return _winScale; } set { _winScale = value; } }
        // 1倍基準の横解像度
        public static readonly double EqualResolWidth = 1280.0; //WXGA
        /// <summary>
        /// 拡大倍率 画面横解像度 / 等倍横解像度(1280) / 画面拡大縮小倍率 
        /// 1.0～3.0倍
        /// </summary>
        public static double ViewScale
        {
            get
            {
                double res = ResolWidth / EqualResolWidth / WinScale;
                if (res < 1.0)
                    res = 1.0;
                else if (res > 3.0)
                    res = 3.0;
                return res;
            }
        }

        /// <summary>
        /// DPI倍率 横
        /// </summary>
        /// <param name="visual"></param>
        /// <returns></returns>
        public static double GetDpiScaleWidth(this Visual visual)
        {
            Point dpiScale = GetDpiScaleFactor(visual);
            double dpiScaleWidth = dpiScale.X;

            return dpiScaleWidth;
        }

        /// <summary>
        /// 現在の <see cref="T:System.Windows.Media.Visual"/> から、DPI 倍率を取得します。
        /// </summary>
        /// <returns>
        /// X 軸 および Y 軸それぞれの DPI 倍率を表す <see cref="T:System.Windows.Point"/>
        /// 構造体。取得に失敗した場合、(1.0, 1.0) を返します。
        /// </returns>
        public static Point GetDpiScaleFactor(this Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);
            if (source != null && source.CompositionTarget != null)
            {
                return new Point(
                    source.CompositionTarget.TransformToDevice.M11,
                    source.CompositionTarget.TransformToDevice.M22);
            }

            return new Point(1.0, 1.0);
        }
    }

    /// <summary>
    /// 会議アプリケーション情報格納用クラス
    /// </summary>
    public class MeetAppInfo
    {
        readonly Logger logger = Logger.GetInstance;

        private readonly ConstParams.MeetingApplication _meetApp;
        private readonly string _appName;
        private readonly string _registryPath;
        private Microsoft.Win32.RegistryHive _hkey;

        public ConstParams.MeetingApplication MeetApp { get { return _meetApp; } }
        public string AppName { get { return _appName; } }
        public string RegistryPath { get { return _registryPath; } }
        public Microsoft.Win32.RegistryHive Hkey { get { return _hkey; } }

        /// <summary>
        /// ※未使用
        /// </summary>
        /// <param name="meetApp"></param>
        /// <param name="registryName"></param>
        /// <param name="hkey"></param>
        public MeetAppInfo(ConstParams.MeetingApplication meetApp, string registryName, Microsoft.Win32.RegistryHive hkey)
        {
            _meetApp = meetApp;
            _appName = EnumEx.GetName(_meetApp);

            _registryPath = registryName;
            _hkey = hkey;
        }
        /// <summary>
        /// 会議アプリ情報クラスコンストラクタ
        /// </summary>
        /// <param name="meetApp"></param>
        /// <param name="hkey"></param>
        public MeetAppInfo(ConstParams.MeetingApplication meetApp, Microsoft.Win32.RegistryHive hkey)
        {
            _meetApp = meetApp;
            _appName = EnumEx.GetName(_meetApp);

            _hkey = hkey;
            _registryPath = GetRegistryPathFromDisplayName(meetApp, hkey);
        }

        private string GetRegistryPathFromDisplayName(ConstParams.MeetingApplication meetApp, Microsoft.Win32.RegistryHive hkey)
        {
            string uninstall_path;
            string ret = null;

            try
            {
                Microsoft.Win32.RegistryKey x64BaseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(hkey, Microsoft.Win32.RegistryView.Registry64);
                Microsoft.Win32.RegistryKey x86BaseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32);
                Microsoft.Win32.RegistryKey WinStoreBaseKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.CurrentUser, Microsoft.Win32.RegistryView.Registry64);

                // Windows Store HKCU, for Skype
                if (meetApp == ConstParams.MeetingApplication.Skype)
                {
                    uninstall_path = @"SOFTWARE\Classes\Local Settings\Software\Microsoft\Windows\CurrentVersion\AppModel\Repository\Packages";
                    using (Microsoft.Win32.RegistryKey uninstall = WinStoreBaseKey.OpenSubKey(uninstall_path, false))
                    {
                        if (uninstall != null)
                        {
                            foreach (string subKey in uninstall.GetSubKeyNames())
                            {
                                string appName = EnumEx.GetName(meetApp);
                                using (Microsoft.Win32.RegistryKey appkey = WinStoreBaseKey.OpenSubKey(uninstall_path + "\\" + subKey, false))
                                {
                                    if (appkey.GetValue("DisplayName") != null)
                                    {
                                        if (appkey.GetValue("DisplayName").ToString().StartsWith(appName))
                                        {
                                            ret = $"{uninstall_path}" + "\\" + $"{subKey}";
                                            _hkey = Microsoft.Win32.RegistryHive.CurrentUser;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // 64bit, for except Skype or case of can't find Skype at previous key
                if( ret == null)
                {
                    uninstall_path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
                    using (Microsoft.Win32.RegistryKey uninstall = x64BaseKey.OpenSubKey(uninstall_path, false))
                    {
                        if (uninstall != null)
                        {
                            foreach (string subKey in uninstall.GetSubKeyNames())
                            {
                                string appName = EnumEx.GetName(meetApp);
                                using (Microsoft.Win32.RegistryKey appkey = x64BaseKey.OpenSubKey(uninstall_path + "\\" + subKey, false))
                                {
                                    if (appkey.GetValue("DisplayName") != null)
                                    {
                                        if (appkey.GetValue("DisplayName").ToString().StartsWith(appName))
                                        {
                                            ret = $"{uninstall_path}" + "\\" + $"{subKey}";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                // 32bit for case of can't find apps at previous key
                if (ret == null && hkey == Microsoft.Win32.RegistryHive.LocalMachine)
                {
                    uninstall_path = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
                    using (Microsoft.Win32.RegistryKey uninstall = x86BaseKey.OpenSubKey(uninstall_path, false))
                    {
                        if (uninstall != null)
                        {
                            foreach (string subKey in uninstall.GetSubKeyNames())
                            {
                                string appName = EnumEx.GetName(meetApp);
                                using (Microsoft.Win32.RegistryKey appkey = x86BaseKey.OpenSubKey(uninstall_path + "\\" + subKey, false))
                                {
                                    if (appkey.GetValue("DisplayName") != null)
                                    {
                                        if (appkey.GetValue("DisplayName").ToString().StartsWith(appName))
                                        {
                                            ret = $"{uninstall_path}" + "\\" + $"{subKey}";
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.WriteLog("Exception throw from [" + e.TargetSite.Name + "] at [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + e.Message, Logger.LogLevel.Error);
            }

            return ret;
        }

        private int _processID = -1;
        private IntPtr _hWnd = IntPtr.Zero; //@@TEST
        private string _iconPath;
        private string _exePath;
        private string _exeArgs;
        public int ProcessID
        {
            get { return _processID; }
            set { _processID = value; }
        }
        public IntPtr HWnd
        { 
            get { return _hWnd; }
            set { _hWnd = value; }
        }
        public string IconPath
        {
            get { return _iconPath; }
            set { _iconPath = value; }
        }
        public string ExePath
        { 
            get { return _exePath; } 
            set { _exePath = value; }
        }
        public string ExeArgs
        {
            get { return _exeArgs; }
            set { _exeArgs = value; }
        }
    }

    /// <summary>
    /// Enum操作用拡張クラス
    /// </summary>
    internal static class EnumEx
    {
        /// <summary>
        /// Description属性を取得
        /// </summary>
        public static string GetName(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            var desciptionString = attributes.Select(n => n.Description).FirstOrDefault();
            if (desciptionString != null)
            {
                return desciptionString;
            }
            return value.ToString();
        }
    }
}
