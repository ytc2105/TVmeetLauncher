//#define PWA //@@TEST GoogleMeet_PWAテスト用

using MahApps.Metro.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Shell;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WIN32API;

namespace TVmeetLauncher
{
    /// <summary>
    /// LauncherWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LauncherWindow : MetroWindow
    {
        static readonly Logger logger = Logger.GetInstance;

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
            //new MeetAppInfo(ConstParams.MeetingApplication.MicrosoftTeamsWorkOrSchool, Microsoft.Win32.RegistryHive.CurrentUser),
        };

        public LauncherWindow()
        {
            InitializeComponent();

            try
            {
                /// Meeting applications 設定
                foreach (MeetAppInfo item in appsInfo)
                {
                    SetMeetApps(item);
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
                if (appCnt == 0)
                {
                    logger.WriteLog($"Apps Count {appCnt}, Can't find any Meeting Application.", Logger.LogLevel.Error);
                    AddCloseToContentRenderedEvent("ミーティングアプリケーションが見つかりませんでした。\nテレビ会議システムを終了します。", "異常終了");
                }
                else 
                {
                    logger.WriteLog(string.Format("Setting Complete, Apps Count {0}.", appCnt));
                    Task.Run(() => TaskBarPolling()); //@@TEST POLLING
                }
                BaseViewModel.Instance.IsLauncherReady = true;
            }
            catch (Exception e)
            {
                logger.WriteLog("Exception throw from [" + e.TargetSite.Name + "] at [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + e.Message, Logger.LogLevel.Fatal);
                AddCloseToContentRenderedEvent($"申し訳ありません。\nアプリケーション起動中に異常を検知したため\nテレビ会議システムを終了します。\n\n--異常メッセージ--\n{e?.Message}", "異常終了");
            }
        }

        private void AddCloseToContentRenderedEvent(string msg, string title)
        {
            ContentRendered += (s, e) =>
            {
                MessageBox.Show(msg, title);
                Close();
            };
        }

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        /// <param name="appName">会議アプリ名</param>
        /// <param name="registryPath">アイコン/起動ファイル参照用レジストリパス</param>
        private void SetMeetApps(MeetAppInfo meet)
        {

            ConstParams.MeetingApplication meetApp  = meet.MeetApp;
            string appName                          = meet.AppName;
            string registryPath                     = meet.RegistryPath;
            Microsoft.Win32.RegistryHive hkey       = meet.Hkey;
            
            bool isReSetup = false;
            if (meet.ExePath != null)
                isReSetup = true;

            try
            {
                if (registryPath != null)
                {
                    if (!isReSetup)
                        logger.WriteLog(string.Format("Setting the Application | appName:{0}, registryPath:{1}, hkey:{2}", appName, registryPath, hkey));
                    else
                        logger.WriteLog(string.Format("ReSetting the Application | appName:{0}, registryPath:{1}, hkey:{2}", appName, registryPath, hkey));

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
                                    // Installed App
                                    if (appkey.GetValue("DisplayIcon") != null)
                                    {
                                        // Icon
                                        iconPath = appkey.GetValue("DisplayIcon").ToString();
                                        // exe
                                        exePath = appkey.GetValue("InstallLocation").ToString();
                                        exePath += @"\current\Teams.exe";
                                    } // UWP(WindowsStore)app
                                    else
                                    {
                                        iconPath = appkey.GetValue("PackageRootFolder").ToString() + @"\msteams.exe";
                                        exePath = "msteams.exe"; //@"shell:appsfolder\Microsoft.SkypeApp_kzf8qxf38zg5c!App";
                                    }

                                    // arguments
                                    exeArgs = "";
                                    break;
                                //case ConstParams.MeetingApplication.MicrosoftTeamsWorkOrSchool:
                                //    // Installed App
                                //    if (appkey.GetValue("DisplayIcon") != null)
                                //    {
                                //        // Icon
                                //        iconPath = appkey.GetValue("DisplayIcon").ToString();
                                //        // exe
                                //        exePath = appkey.GetValue("InstallLocation").ToString();
                                //        exePath += @"\current\Teams.exe";
                                //    } // UWP(WindowsStore)app
                                //    else
                                //    {
                                //        iconPath = appkey.GetValue("PackageRootFolder").ToString() + @"\msteams.exe";
                                //        exePath = "msteams.exe"; //@"shell:appsfolder\Microsoft.SkypeApp_kzf8qxf38zg5c!App";
                                //    }

                                //    // arguments
                                //    exeArgs = "";
                                //    break;
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
                                        exePath = "Skype.exe"; //@"shell:appsfolder\Microsoft.SkypeApp_kzf8qxf38zg5c!App";
                                    }

                                    // arguments
                                    exeArgs = "";
                                    break;
                                default:
                                    logger.WriteLog("Setting Invalid Application: " + appName + " |Registry: " + registryPath + " |HKEY: " + hkey.ToString(), Logger.LogLevel.Warn);
                                    return;
                            }
                            meet.IconPath = iconPath;
                            meet.ExePath = exePath;
                            meet.ExeArgs = exeArgs;
                            if(!isReSetup)
                                SetAppContent(meet);
                        }
                        else
                        {
                            logger.WriteLog("Can't find Registry key: " + appName + " |Registry: " + registryPath + " |HKEY: " + hkey.ToString(), Logger.LogLevel.Warn);
                        }
                    }

                }
                else
                {
                    logger.WriteLog(string.Format("Can't find Meeting Application | appName:{0}, registryPath:{1}, hkey:{2}", appName, registryPath, hkey), Logger.LogLevel.Warn);
#if PWA
                    //@@TEST Google MeetのURLをPWAで開くための処理　※アイコン取得が困難な為，未実装
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
        private void SetAppContent(MeetAppInfo meet)
        {
            string  appName     = meet.AppName,
                    iconPath    = meet.IconPath,
                    exePath     = meet.ExePath,
                    exeArgs     = meet.ExeArgs;
            logger.WriteLog(String.Format("Setting the Content | appName:{0}, iconPath:{1}, exePath:{2}, exeArgs:{3}", appName, iconPath, exePath, exeArgs));

            try
            {
                /// Set App Icon Image
                Image image = new Image
                {
                    Name = "img" + appName.Replace(" ", String.Empty),
                    Height = ConstParams.iconHeight * ConstParams.ViewScale,
                    Width = ConstParams.iconWidth * ConstParams.ViewScale,
                };
                if (!System.IO.Path.IsPathRooted(iconPath))
                    image.Source = new BitmapImage(new Uri(iconPath, UriKind.Relative));
                else
                    image.Source = GetImageSourceFromAPI(iconPath);

                /// Set App Button
                ButtonEx button = new ButtonEx
                {
                    Name = "btn" + appName.Replace(" ", String.Empty),
                    Height = (ConstParams.iconHeight + ConstParams.buttonMargin) * ConstParams.ViewScale,
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
                    FontSize = BaseViewModel.Instance.CaptionFontSize,
                };

                ToolTipService.SetInitialShowDelay(button, 1);
                ToolTipService.SetToolTip(button, toolTipText);

                button.Content = image;
                button.AppInfo = meet;

                button.Click += new RoutedEventHandler(ProcStartApp);

                appsGrid.Children.Add(button);
            }
            catch (Exception e)
            {
                logger.WriteLog("Exception throw from [" + e.TargetSite.Name + "] at [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + e.Message, Logger.LogLevel.Error);
            }
        }

        /// <summary>
        /// アプリケーション起動処理
        /// </summary>
        /// <param name="sender">Buttonオブジェクト</param>
        /// <param name="e"></param>
        private void ProcStartApp(object sender, RoutedEventArgs e)
        {
            ButtonEx btn = (ButtonEx)sender;
            MeetAppInfo meet = btn.AppInfo;
            bool isWindowExist = false;

            //画面がある場合、最前面へ(現状、※Google Meetのみ対応)
            if (meet.MeetApp == ConstParams.MeetingApplication.GoogleMeet)
                isWindowExist = SearchWindow(meet);
            
            if (!isWindowExist)
            {
                using (System.Diagnostics.Process p = new System.Diagnostics.Process())
                {
                    p.StartInfo.FileName = meet.ExePath;
                    p.StartInfo.Arguments = meet.ExeArgs;
                    p.StartInfo.UseShellExecute = true;
                    try
                    {
                        p.Start();
                        logger.WriteLog($"Start Application:{p.ProcessName}, | Title:{p.MainWindowTitle}, | ID:{p.Id}"
                             + $".|| ExePath=[{meet.ExePath}], | Arg=[{meet.ExeArgs}]");
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog("Exception error occured: [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | " + ex.Message, Logger.LogLevel.Error);
                        // アップデート後はパスが変わることがある為、再設定
                        SetMeetApps(meet);
                    }
                }
            }
            else
            {
                //ウィンドウを前面へ移動
                WinUIAPI.MoveWindowToTop(meet.HWnd);
                string title        = WinUIAPI.GetTitleFromHwnd(meet.HWnd),
                       className    = WinUIAPI.GetClassNameFromHwnd(meet.HWnd);
                logger.WriteLog("Move window to top side is done. => MeetAppName:" + meet.AppName
                    + " | Title:" + title + " | ClassName:" + className + " | hWnd:" + meet.HWnd.ToString(), Logger.LogLevel.Info);
            }
            // Loading effect(連続クリック防止)
            ProcAfterClick(btn);
        }

        /// <summary>
        /// 会議アプリのウィンドウを探索して会議情報インスタンスへ格納
        /// 会議アプリ名が含まれるウィンドウを探索し、実行ファイルパスが一致したウィンドウ情報を格納する
        /// </summary>
        /// <param name="meet">会議情報インスタンス</param>
        /// <returns>true:ウィンドウが見つかった false:見つからなかった</returns>
        private bool SearchWindow(MeetAppInfo meet)
        {
            //会議名がタイトルに含まれるウィンドウ探索
            List<IntPtr> windows = WinUIAPI.FindWindowsWithText(meet.AppName);

            foreach(IntPtr hWnd in windows)
            {
                if (hWnd != IntPtr.Zero)
                {
                    //ウィンドウを作成したプロセスのIDを取得する
                    int processId = WinUIAPI.GetPidFromHwnd(hWnd);

                    /// Windowの実行ファイルパスを取得
                    //プロセスハンドルオープン
                    var handle = Kernel32.OpenProcess(Kernel32.ProcessSecurity.ProcessVmRead |
                        Kernel32.ProcessSecurity.ProcessQueryInformation, false, (uint)processId);
                    StringBuilder exeFileFullPath = new StringBuilder(1024);

                    try
                    {
                        //ハンドルの実行ファイルパスを格納
                        if (handle != IntPtr.Zero)
                            Psapi.GetModuleFileNameEx(handle, IntPtr.Zero, exeFileFullPath, (uint)exeFileFullPath.Capacity);
                    }
                    catch (Exception ex)
                    {
                        logger.WriteLog("Exception error occured: [" + System.Reflection.MethodBase.GetCurrentMethod().Name + "]. | "
                            + ex.Message, Logger.LogLevel.Error);
                    }
                    finally
                    {
                        Kernel32.CloseHandle(handle);
                    }

                    if (exeFileFullPath != null)
                    {   // 実行ファイルパスが一致
                        if (meet.ExePath.Equals(exeFileFullPath.ToString()))
                        {
                            //該当ウィンドウのProcessIDを会議情報へ保存
                            meet.ProcessID = processId;
                            meet.HWnd = hWnd;
                            logger.WriteLog("Find Window.=> MeetAppName:" + meet.AppName
                                + " | searchFileName:" + meet.ExePath + " | ProcessID:" + processId, Logger.LogLevel.Debug);
                            return true;
                        }
                    }
                }
            }

            logger.WriteLog("Can't Find Window => MeetAppName:" + meet.AppName + " | searchFileName:" + meet.ExePath,
                    Logger.LogLevel.Debug);
            return false;
        }

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

            // End effect, Enable button
            ButtonProgressAssist.SetValue(button, 1);
            ButtonProgressAssist.SetIsIndicatorVisible(button, false);
            ButtonProgressAssist.SetIsIndeterminate(button, false);
            button.IsEnabled = true;
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

        /// <summary>
        /// "(ダブルクォート)パーサー
        /// </summary>
        public static readonly Parser<string> QuotedText = (from open in Parse.Char('"')
                                                            from content in Parse.CharExcept('"').Many().Text()
                                                            from close in Parse.Char('"')
                                                            select content).Token();

        //@@TEST TaskBar非表示-自動的に隠す対応用
        private async void TaskBarPolling() 
        {
            CommandViewModel cvm = CommandViewModel.Instance;
            while (cvm.IsTaskBarPollingRun)
            {
                await Task.Delay(300);
                if (cvm.IsTaskBarPollingRun) // delay中フラグ変更後も即時対応するため
                {
                    cvm.VerifyTaskBarHide();
                }
            }
        }

    }

}