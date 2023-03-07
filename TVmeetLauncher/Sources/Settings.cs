using Sprache;
using System;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Windows;
using System.Windows.Controls;
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
            [Description("Microsoft Teams")]
            MicrosoftTeamsWorkOrSchool,
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

        /// <summary>
        /// Windows Verisionを取得
        /// </summary>
        /// <returns>string:Windows Version</returns>
        public static string GetWinVer() 
        {
            string winVer = null;
            ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementBaseObject mo in moc)
            {
                winVer = mo["Caption"].ToString();
                Logger.GetInstance.WriteLog(winVer, Logger.LogLevel.Debug);
            }
            moc.Dispose();
            mc.Dispose();

            return winVer;
        }

        public static bool IsWindows11()
        {
            if ( GetWinVer().Contains("Windows 11") )
                return true;
            else
                return false;
        }
    }
    
    /// <summary>
    /// 製品情報格納クラス
    /// </summary>
    public static class AssemblyInfo
    {
        #region メンバ変数
        private static readonly Assembly assembly = Assembly.GetExecutingAssembly();
        private static readonly string  _name    = assembly.GetName().Name;     // アセンブリ名  
        private static readonly Version _version = assembly.GetName().Version;  // アセンブリバージョン
        //private static readonly string _title       = assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;                  // "タイトル"
        private static readonly string _description = assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description;      // "説明"
        //private static readonly string _company     = assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;              // "会社"
        private static readonly string _product     = assembly.GetCustomAttribute<AssemblyProductAttribute>().Product;              // "製品"
        private static readonly string _copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;          // "著作権"
        //private static readonly string _trademark   = assembly.GetCustomAttribute<AssemblyTrademarkAttribute>().Trademark;          // "商標"
        //private static readonly string _language    = assembly.GetCustomAttribute<NeutralResourcesLanguageAttribute>().CultureName; // "ja-JP"
        private static readonly string _guid = assembly.GetCustomAttribute<GuidAttribute>().Value;                           // "00000000-1111-2222-3333-444444444444"
        //private static readonly bool   _comVisible  = assembly.GetCustomAttribute<ComVisibleAttribute>().Value;                     // "true"
        //private static readonly string _fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;                       // "3.0.0.0"
        //private static readonly string _infoVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion; // "4.0.0.0"
        #endregion

        /// <summary>
        /// 製品名(タイトル)
        /// </summary>
        public static string Name { get { return _name; } }
        /// <summary>
        /// バージョン情報
        /// </summary>
        public static string Version { get { return _version.ToString(3); } }
        /// <summary>
        /// 製品説明
        /// </summary>
        public static string Description { get { return _description; } }
        /// <summary>
        /// 製品名(商標マーク付き)
        /// </summary>
        public static string Product { get { return _product; } }
        /// <summary>
        /// 著作権表示
        /// </summary>
        public static string Copyright { get { return _copyright; } }
        /// <summary>
        /// アセンブリGUID
        /// </summary>
        public static string Guid { get { return _guid; } }

    }

    /// <summary>
    /// 会議アプリケーション情報格納用クラス
    /// </summary>
    public class MeetAppInfo
    {
        private readonly ConstParams.MeetingApplication _meetApp;
        private string _appName;
        private readonly string _registryPath;
        private Microsoft.Win32.RegistryHive _hkey;

        public ConstParams.MeetingApplication MeetApp { get { return _meetApp; } }
        public string AppName { get { return _appName; } set { _appName = value; } }
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

                // Windows Store HKCU, for UWP apps (Skype & Microsoft Teams)
                if (meetApp == ConstParams.MeetingApplication.Skype || meetApp == ConstParams.MeetingApplication.MicrosoftTeams)
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
                // 64bit, for except Skype "AND" case of can't find Skype at previous key
                if ( ret == null)
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
                Logger.GetInstance.WriteLog("Exception throw from [" + e.TargetSite.Name + "] at [" + 
                    System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + e.Message, Logger.LogLevel.Error);
            }

            return ret;
        }

        private int _processID = -1;
        private IntPtr _hWnd = IntPtr.Zero;
        private string _iconPath = null;
        private string _exePath = null;
        private string _exeArgs = null;
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

    /// <summary>
    /// 拡張Buttonコントロール
    /// ・会議アプリ情報プロパティ追加
    /// </summary>
    public class ButtonEx : Button
    {
        static ButtonEx()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonEx), new FrameworkPropertyMetadata(typeof(ButtonEx)));
        }

        private MeetAppInfo _appInfo;
        /// <summary>
        /// 会議アプリクラスプロパティ
        /// </summary>
        public MeetAppInfo AppInfo
        {
            get { return _appInfo; }
            set { _appInfo = value; }
        }
    }
}
