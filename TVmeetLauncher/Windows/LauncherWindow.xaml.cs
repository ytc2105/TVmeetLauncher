//#define PWA //@@TEST GoogleMeet_PWAテスト用

using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sprache;
using WIN32API;
using System.Text;

namespace TVmeetLauncher
{
    /// <summary>
    /// LauncherWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LauncherWindow : MetroWindow
    {
        static readonly Logger logger = Logger.GetInstance;
        static readonly CommandViewModel cvm = CommandViewModel.Instance;

        /// <summary>
        /// 会議アプリ情報リスト AppName=会議アプリ名，RegistryPath=レジストリパス, Hkey=ルートレジストリキー 
        /// </summary>
        internal static readonly List<MeetAppInfo> appsInfo = new List<MeetAppInfo>()
        {   //                  会議アプリ名,                                     (レジストリパス),                                                                           ルートレジストリキー
            //new MeetAppInfo(ConstParams.MeetingApplication.GoogleMeet,     @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\b3980fc0325e45a7cd5ddcdab50ab4ba",        Microsoft.Win32.RegistryHive.CurrentUser),
            //new MeetAppInfo(ConstParams.MeetingApplication.Zoom,           @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\ZoomUMX",                                 Microsoft.Win32.RegistryHive.CurrentUser),
            //new MeetAppInfo(ConstParams.MeetingApplication.MicrosoftTeams, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Teams",                                   Microsoft.Win32.RegistryHive.CurrentUser),
            //new MeetAppInfo(ConstParams.MeetingApplication.Webex,          @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{42C2D2B4-DCC7-47B9-B404-57705328E84A}",  Microsoft.Win32.RegistryHive.LocalMachine),
            //new MeetAppInfo(ConstParams.MeetingApplication.Skype,          @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\Skype.exe",  Microsoft.Win32.RegistryHive.LocalMachine),
            //                  会議アプリ名,                                     ルートレジストリキー
            new MeetAppInfo(ConstParams.MeetingApplication.Webex,          Microsoft.Win32.RegistryHive.LocalMachine),
            new MeetAppInfo(ConstParams.MeetingApplication.Skype,          Microsoft.Win32.RegistryHive.LocalMachine),
            new MeetAppInfo(ConstParams.MeetingApplication.Zoom,           Microsoft.Win32.RegistryHive.CurrentUser),
            new MeetAppInfo(ConstParams.MeetingApplication.GoogleMeet,     Microsoft.Win32.RegistryHive.CurrentUser),
            new MeetAppInfo(ConstParams.MeetingApplication.MicrosoftTeams, Microsoft.Win32.RegistryHive.CurrentUser),
        };

        public LauncherWindow()
        {
            InitializeComponent();

            //logger = Logger.GetInstance;
            logger.WriteLog("TVmeetLauncher start.");
            logger.LogLifeSpan = 30; //ログの寿命(日)
            logger.CheckOldLogfile();

            try
            {
                /// Meeting applications 設定
                foreach (MeetAppInfo item in appsInfo)
                {
                    SetMeetApps(item.MeetApp, item.RegistryPath, item.Hkey);
                }

                var appObjects = appsGrid.GetChildObjects();
                int column = 0, row = 0, appCnt = 0, appNum;
                appNum = appsGrid.Children.Count;

                foreach (UIElement app in appObjects.Cast<UIElement>())
                {
                    appCnt++;
                    switch (appCnt)
                    {
                        case 1:
                            appsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            appsGrid.RowDefinitions.Add(new RowDefinition());
                            break;
                        case 2:
                            appsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            break;
                        case 3:
                            appsGrid.RowDefinitions.Add(new RowDefinition());
                            if (appNum <= 4)
                            {
                                column = 0;
                                row = 1;
                            }
                            break;
                        case 4:
                            if (appNum >= 5 && appNum <= 6)
                            {
                                column = 0;
                                row = 1;
                            }
                            break;
                        case 5:
                            appsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            break;
                        case 6:
                            break;
                        case 7:
                            appsGrid.ColumnDefinitions.Add(new ColumnDefinition());
                            break;
                        case 8:
                            break;
                        default:
                            break;
                    }

                    Grid.SetColumn(app, column++);
                    Grid.SetRow(app, row);
                }
                logger.WriteLog(string.Format("Setting Complete, Apps Count {0}.", appCnt));
                this.MinHeight *= ConstParams.ViewScale; this.MinWidth *= ConstParams.ViewScale;
            }
            catch (Exception e)
            {
                logger.WriteLog("Exception throw from [" + e.TargetSite.Name + "] at [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + e.Message, Logger.LogLevel.Fatal);
            }
            finally
            {
                BaseViewModel.Instance.IsLauncherReady = true;
                Task.Run(() => TaskBarPolling()); //@@TEST POLLING
            }
        }

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        /// <param name="appName">会議アプリ名</param>
        /// <param name="registryPath">アイコン/起動ファイル参照用レジストリパス</param>
        private void SetMeetApps(ConstParams.MeetingApplication meetApp, string registryPath, Microsoft.Win32.RegistryHive hkey)
        {
            string appName = EnumEx.GetName(meetApp);
            logger.WriteLog(string.Format("Setting the Application | appName:{0}, registryPath:{1}, hkey:{2}", appName, registryPath, hkey));

            try
            {
                if (registryPath != null)
                {
                    using (Microsoft.Win32.RegistryKey appkey = Microsoft.Win32.RegistryKey.OpenBaseKey(hkey, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(registryPath, false))
                    {
                        if (appkey != null)
                        {
                            string iconPath, exePath, exeArgs;
                            switch (meetApp)
                            {
                                case ConstParams.MeetingApplication.GoogleMeet:
                                    // Icon
                                    iconPath = appkey.GetValue("DisplayIcon").ToString();
                                    // exe
                                    string uninstall = appkey.GetValue("UninstallString").ToString();  // アンストール実行パス取得
                                    exePath = uninstall.Remove(uninstall.IndexOf('\"', 1) + 1);  // 引数削除（chrome.exe実行パス取得）
                                    exePath = QuotedText.Parse(exePath);
                                    exeArgs = "--profile-directory=Default --app-id=kjgfgldnnfoeklkmfkjfagphfepbbdan";  // Google Meet起動用引数
                                    break;
                                case ConstParams.MeetingApplication.Zoom:
                                    // Icon
                                    iconPath = appkey.GetValue("DisplayIcon").ToString();
                                    // exe
                                    exePath = iconPath;
                                    // arguments
                                    exeArgs = "";
                                    break;
                                case ConstParams.MeetingApplication.MicrosoftTeams:
                                    // Icon
                                    iconPath = appkey.GetValue("DisplayIcon").ToString();
                                    // exe
                                    exePath = appkey.GetValue("InstallLocation").ToString();
                                    exePath += @"\current\Teams.exe";
                                    // arguments
                                    exeArgs = "";
                                    break;
                                case ConstParams.MeetingApplication.Webex:
                                    // Icon
                                    iconPath = appkey.GetValue("InstallLocation").ToString();
                                    iconPath += "CiscoCollabHost.exe";
                                    // exe
                                    exePath = iconPath;
                                    // arguments
                                    exeArgs = "";
                                    break;
                                case ConstParams.MeetingApplication.Skype:
                                    // Installed App
                                    if (appkey.GetValue("DisplayIcon") != null) {
                                        iconPath = appkey.GetValue("DisplayIcon").ToString();
                                        exePath = iconPath;
                                    } // UWP(WindowsStore)app
                                    else {
                                        iconPath = appkey.GetValue("PackageRootFolder").ToString() + @"\Skype\Skype.exe";
                                        exePath = @"shell:appsfolder\Microsoft.SkypeApp_kzf8qxf38zg5c!App";
                                    }

                                    // arguments
                                    exeArgs = "";
                                    break;
                                default:
                                    logger.WriteLog("Setting Invalid Application: " + appName + " |Registry: " + registryPath + " |HKEY: " + hkey.ToString());
                                    return;
                            }
                            SetAppContent(appName, iconPath, exePath, exeArgs);
                        }
                        else
                        {
                            logger.WriteLog("Can't find Registry key: " + appName + " |Registry: " + registryPath + " |HKEY: " + hkey.ToString());
                        }
                    }

                }
                else
                {
#if PWA
                    //@@TEST Google MeetのHPをPWAで開くための処理　※アイコン取得が困難な為，未実装
                    if (meetApp == ConstParams.MeetingApplication.GoogleMeet)
                    {
                        SetGoogleMeetPWA();
                    }
                    else
                    {
                        // 
                    }
#endif
                }
            }
            catch (Exception e)
            {
                logger.WriteLog("Exception throw from [" + e.TargetSite.Name + "] at [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + e.Message, Logger.LogLevel.Error);
            }
        }

#if PWA
        private void SetGoogleMeetPWA()
        {
            string internet_path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";
            string iconPath, exePath, exeArgs;
            string appName = EnumEx.GetName(ConstParams.MeetingApplication.GoogleMeet);

            try
            {
                using (Microsoft.Win32.RegistryKey internet = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(internet_path, false))
                {
                    if (internet != null)
                    {
                        string exeName = null;
                        foreach (string subKey in internet.GetSubKeyNames())
                        {
                            switch (subKey)
                            {
                                case "chrome.exe":
                                    exeName = "\\chrome_proxy.exe";
                                    break;
                                case "msedge.exe":
                                    exeName = "\\msedge_proxy.exe";
                                    break;
                                default:
                                    continue;
                            }

                            if (exeName != null)
                            {
                                using (Microsoft.Win32.RegistryKey appkey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64).OpenSubKey(internet_path + "\\" + subKey, false))
                                {
                                    if (appkey.GetValue("Path") != null)
                                    {    // Icon
                                        //iconPath = appkey.GetValue("").ToString(); //@@TEST for debug
                                        iconPath = @"..\Images\logo\gm.png";
                                        // exe
                                        exePath = appkey.GetValue("Path") + exeName;  // 引数削除（chrome.exe実行パス取得）
                                        exeArgs = @"--app=https://meet.google.com/";  // Google Meet起動用引数
                                        SetAppContent(appName, iconPath, exePath, exeArgs);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.WriteLog("Exception throw from [" + e.TargetSite.Name + "] at [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + e.Message, Logger.LogLevel.Error);
            }
        }
#endif

        /// <summary>
        /// 会議アプリコンテンツの作成と追加
        /// </summary>
        /// <param name="appName">アプリケーション表示名</param>
        /// <param name="iconPath">アイコンファイルパス</param>
        /// <param name="exePath">起動ファイルパス</param>
        /// <param name="exeArgs">起動コマンド引数</param>
        private void SetAppContent(string appName, string iconPath, string exePath, string exeArgs)
        {
            logger.WriteLog(String.Format("Setting the Content | appName:{0}, iconPath:{1}, exePath:{2}, exeArgs:{3}", appName, iconPath, exePath, exeArgs));

            /// Set App Icon Image
            Image image = new Image
            {
                Name = "img" + appName.Replace(" ", String.Empty),
                Height = ConstParams.iconHeight * ConstParams.ViewScale,    //@@TEST
                Width = ConstParams.iconWidth * ConstParams.ViewScale,
            };
            if ( !System.IO.Path.IsPathRooted(iconPath) )
                image.Source = new BitmapImage(new Uri(iconPath, UriKind.Relative));
            else
                image.Source = GetImageSourceFromAPI(iconPath);

            /// Set App Button
            ButtonEx button = new ButtonEx
            {
                Name = "btn" + appName.Replace(" ", String.Empty),
                Height = (ConstParams.iconHeight + ConstParams.buttonMargin) * ConstParams.ViewScale, //@@TEST
                Width = (ConstParams.iconWidth + ConstParams.buttonMargin) * ConstParams.ViewScale,
                Cursor = Cursors.Hand,
                ToolTip = appName + " を起動",
                //Background = new SolidColorBrush(Colors.Transparent),
                //BorderBrush = new SolidColorBrush(Colors.Transparent),
                Style = this.FindResource("MaterialDesignFlatButton") as Style,
            };
            ButtonAssist.SetCornerRadius(button, new CornerRadius(25.0f));

            TextBlock toolTipText = new TextBlock
            {
                Text = button.ToolTip.ToString(),
                FontFamily = new FontFamily("UD新ゴNT Pro"),
                FontSize = (int)(12 * ConstParams.ViewScale),
            };

            ToolTipService.SetInitialShowDelay(button, 1);
            ToolTipService.SetToolTip(button, toolTipText);

            button.Content = image;
            button.Click += new RoutedEventHandler(ProcStartApp);

            button.Tag = exePath + "|" + exeArgs;  //@@test 

            appsGrid.Children.Add(button);
        }

        /// <summary>
        /// アプリケーション起動処理
        /// </summary>
        /// <param name="sender">Buttonオブジェクト</param>
        /// <param name="e"></param>
        private void ProcStartApp(object sender, RoutedEventArgs e)
        {
            ButtonEx btn = (ButtonEx)sender;
            string[] strings = ((string)btn.Tag).Split('|');
            string exePath = strings[0];
            string args = strings[1];

            //@@TEST 画面がある場合、最前面へ
            bool isWindowExist = SearchAndMoveWindowToTop("Google Meet", exePath);

            if (!isWindowExist)
            {
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    p.StartInfo.FileName = exePath;
                    p.StartInfo.Arguments = args;
                    p.StartInfo.UseShellExecute = true;
                    try
                    {
                        p.Start();
                        // Loading effect
                        ProcAfterClick((ButtonEx)sender);

                        logger.WriteLog("Start Application: " + p.ProcessName + ", | Title:" + p.MainWindowTitle + " | ID:" + p.Id);
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog("Exception error occured: [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + ex.Message, Logger.LogLevel.Error);
                    }
                    finally
                    {
                        //@@TEST TaskBarHide
                        //cvm.VerifyTaskBarHide(); 
                    }
                }
            }
        }

        #region FindWindow
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(
            IntPtr hWnd, out int lpdwProcessId);

        /// <summary>
        /// 会議名と実行ファイルパスが一致するウィンドウを探索し、最前面へ表示する
        /// </summary>
        /// <param name="meetAppName"></param>
        /// <param name="searchFileName"></param>
        /// <returns>ウィンドウが見つかった:true 見つからなかった:false</returns>
        private bool SearchAndMoveWindowToTop(string meetAppName, string searchFileName) 
        {
            // ProcessID探索
            


            StringBuilder exeFileFullPath = new StringBuilder(1000);

            //会議名ウィンドウ探索
            IntPtr hWnd = FindWindow(null, meetAppName);
            if (hWnd != IntPtr.Zero)
            {
                //ウィンドウを作成したプロセスのIDを取得する
                GetWindowThreadProcessId(hWnd, out int processId);
                //プロセスハンドルオープン
                var handle = Kernel32.OpenProcess(Kernel32.ProcessSecurity.ProcessVmRead |
                    Kernel32.ProcessSecurity.ProcessQueryInformation, false, (uint)processId);

                try
                {
                    //ハンドルの実行ファイルパスを格納
                    if (handle != IntPtr.Zero)
                        Psapi.GetModuleFileNameEx(handle, IntPtr.Zero, exeFileFullPath, 1000);
                }
                catch(Exception ex) 
                {
                    logger.WriteLog("Exception error occured: [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " 
                        + ex.Message, Logger.LogLevel.Error);
                }
                finally
                {
                        Kernel32.CloseHandle(handle);
                }

                if (exeFileFullPath != null)
                {
                    if(searchFileName.Equals(exeFileFullPath.ToString()))
                    {
                        WinUIAPI.MoveWindowToTop(hWnd);
                        logger.WriteLog("Find Window and Move Top side Done.=> MeetAppName:" + meetAppName 
                            + " | searchFileName:" + searchFileName + " | ProcessID:" + processId, Logger.LogLevel.Debug);
                        return true; 
                    }
                }
            }
            else
            {
                logger.WriteLog("Can't Find Window => MeetAppName:" + meetAppName + " | searchFileName:" + searchFileName, 
                    Logger.LogLevel.Debug);
            }
            return false;
        }
        #endregion



        /// <summary>
        /// コンテンツクリック後の処理
        /// </summary>
        /// <param name="button">クリックされたボタン</param>
        private async void ProcAfterClick(ButtonEx button)
        {
            // Start effect, Disable button
            ButtonProgressAssist.SetValue(button, -1);
            ButtonProgressAssist.SetIsIndicatorVisible(button, true);
            ButtonProgressAssist.SetIsIndeterminate(button, true);
            button.IsEnabled = false;

            await Task.Delay(2000);
            //Thread.Sleep(3000);

            // Minimize Window size
            //this.WindowState = WindowState.Minimized;

            // End effect, Enable button
            ButtonProgressAssist.SetValue(button, 1);
            ButtonProgressAssist.SetIsIndicatorVisible(button, false);
            ButtonProgressAssist.SetIsIndeterminate(button, false);
            button.IsEnabled = true;

            //@@TEST TaskBarHide
            //cvm.VerifyTaskBarHide();
        }

        /// <summary>
        /// Iconイメージ取得 WindowsAPICodePack使用
        /// </summary>
        /// <param name="path">Iconファイルパス</param>
        /// <returns>Icon画像ソース</returns>
        private ImageSource GetImageSourceFromAPI(string path)
        {
            using (var file = ShellFile.FromFilePath(path))
            {
                // ImageSource source = file.Thumbnail.SmallBitmapSource;      // 16x16
                // ImageSource source = file.Thumbnail.MediumBitmapSource;     // 32x32
                // ImageSource source = file.Thumbnail.LargeBitmapSource;      // 48x48
                ImageSource source = file.Thumbnail.ExtraLargeBitmapSource; // 256x256

                return source;
            }
        }

        private void HideTaskbar_Checked(object sender, RoutedEventArgs e)
        {
            WinUIAPI.TskBarHide();
        }

        private void HideTaskbar_Unchecked(object sender, RoutedEventArgs e)
        {
            WinUIAPI.TskBarDisp();
        }

        private void ShowContextMenu()
        {
            var contextMenu = Resources["contextMenu"] as ContextMenu;
            contextMenu.IsOpen = true;
        }

        private void imgIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //ShowContextMenu();
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
#if !DEBUG
            // desable Alt+F4
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
#endif
        }

        private async void TaskBarPolling() 
        {
            while(cvm.IsTaskBarPollingRun)
            {
                await Task.Delay(300);
                if (cvm.IsTaskBarPollingRun) // delay中フラグ変更後も即時対応するため
                {
                    cvm.VerifyTaskBarHide(); //@@TEST TaskBarHide
                }
            }
        }

        /// <summary>
        /// "(ダブルクォート)パーサー
        /// </summary>
        public static readonly Parser<string> QuotedText = (from open in Parse.Char('"')
                                                            from content in Parse.CharExcept('"').Many().Text()
                                                            from close in Parse.Char('"')
                                                            select content).Token();
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
        public MeetAppInfo AppInfo
        {
            get { return _appInfo; }
            set { _appInfo = value; }
        }
    }
}