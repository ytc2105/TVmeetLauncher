using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Diagnostics;
using WIN32API;

namespace TVmeetLauncher
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        public static BaseViewModel Instance { get; } = new BaseViewModel();

        #region 共通
        private int _headlineFontSize = 20;
        private int _subtitleFontSize = 16;
        private int _captionFontSize = 12;
        public int HeadlineFontSize
        {
            get => _headlineFontSize;
            set
            {
                _headlineFontSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HeadlineFontSize)));
            }
        }
        public int SubtitleFontSize
        {
            get => _subtitleFontSize;
            set
            {
                _subtitleFontSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SubtitleFontSize)));
            }
        }
        public int CaptionFontSize
        {
            get => _captionFontSize;
            set
            {
                _captionFontSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CaptionFontSize)));
            }
        }

        private string _title = AssemblyInfo.Name;
        public string Title
        {
            get => _title;
        }
        private string _productName = AssemblyInfo.Product;
        public string ProductName
        {
            get => _productName;
        }
        private string _productDescription = AssemblyInfo.Description;
        public string ProductDescription
        {
            get => _productDescription;
        }
        private string _versionInfo = AssemblyInfo.Version;
        public string VersionInfo
        {
            get => _versionInfo;
        }
        private string _copyright = AssemblyInfo.Copyright;
        public string Copyright
        {
            get => _copyright;
        }
        #endregion

        #region 背景画面(メイン)
        /// <summary>
        /// ロゴ類
        /// </summary>
        private int _tvLogoHeight = 60;
        private int _tvLogoWidth = 150;
        private int _eleLogoHeight = 30;
        private int _eleLogoWidth = 115;

        public int TVlogoHeight
        {
            get => _tvLogoHeight;
            set
            {
                _tvLogoHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TVlogoHeight)));
            }
        }
        public int TVlogoWidth
        {
            get => _tvLogoWidth;
            set
            {
                _tvLogoWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TVlogoWidth)));
            }
        }
        public int EleLogoHeight
        {
            get => _eleLogoHeight;
            set
            {
                _eleLogoHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EleLogoHeight)));
            }
        }
        public int EleLogoWidth
        {
            get => _eleLogoWidth;
            set
            {
                _eleLogoWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EleLogoWidth)));
            }
        }

        /// <summary>
        /// ロゴ可視ステータス
        /// </summary>
        private string _componentsVisibility = "Hidden";
        public string ComponentsVisibility
        {
            get => _componentsVisibility;
            set
            {
                _componentsVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComponentsVisibility)));
            }
        }
        #endregion

        #region スプラッシュ画面
        private int _splashScreenHeight = 480;
        private int _splashScreenWidth = 800;
        private int _splashLogoHeight = 90;
        private int _splashLogoWidth = 310;
        private int _splashFontSize = 17;
        private int _splashProgressBarSize = 30;
        private int _splashProgressBarHeight = 5;
        private int _splashProgressBarWidth = 310;

        public int SplashScreenHeight
        {
            get => _splashScreenHeight;
            set
            {
                _splashScreenHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SplashScreenHeight)));
            }
        }
        public int SplashScreenWidth
        {
            get => _splashScreenWidth;
            set
            {
                _splashScreenWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SplashScreenWidth)));
            }
        }
        public int SplashLogoHeight
        {
            get => _splashLogoHeight;
            set
            {
                _splashLogoHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SplashLogoHeight)));
            }
        }
        public int SplashLogoWidth
        {
            get => _splashLogoWidth;
            set
            {
                _splashLogoWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SplashLogoWidth)));
            }
        }
        public int SplashFontSize
        {
            get => _splashFontSize;
            set
            {
                _splashFontSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SplashFontSize)));
            }
        }
        public int SplashProgressBarSize
        {
            get => _splashProgressBarSize;
            set
            {
                _splashProgressBarSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SplashProgressBarSize)));
            }
        }
        public int SplashProgressBarHeight
        {
            get => _splashProgressBarHeight;
            set
            {
                _splashProgressBarHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SplashProgressBarHeight)));
            }
        }
        public int SplashProgressBarWidth
        {
            get => _splashProgressBarWidth;
            set
            {
                _splashProgressBarWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(_splashProgressBarWidth)));
            }
        }
        #endregion

        #region ランチャ画面
        /// <summary>
        /// タイトルバー
        /// </summary>
        private int _titleBarIconSize = 20;
        private int _titleBarHeight = 32;
        private int _titleBarFontSize = 12;
        private int _titlePopupSize = 24;
        public int TitleBarIconSize
        {
            get => _titleBarIconSize;
            set
            {
                _titleBarIconSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitleBarIconSize)));
            }
        }
        public int TitleBarHeight
        {
            get => _titleBarHeight;
            set
            {
                _titleBarHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitleBarHeight)));
            }
        }
        public int TitleBarFontSize
        {
            get => _titleBarFontSize;
            set
            {
                _titleBarFontSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitleBarFontSize)));
            }
        }
        public int TitlePopupSize
        {
            get => _titlePopupSize;
            set
            {
                _titlePopupSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TitlePopupSize)));
            }
        }

        /// <summary>
        /// ウィンドウサイズ
        /// </summary>
        private int _launcherHeight = 480;
        private int _launcherWidth = 720;
        public int LauncherHeight
        {
            get => _launcherHeight;
            set
            {
                _launcherHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LauncherHeight)));
            }
        }
        public int LauncherWidth
        {
            get => _launcherWidth;
            set
            {
                _launcherWidth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LauncherWidth)));
            }
        }


        /// <summary>
        /// 準備完了フラグ
        /// </summary>
        private bool _isLauncherReady = false;
        public bool IsLauncherReady
        {
            get => _isLauncherReady;
            set
            {
                _isLauncherReady = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsLauncherReady)));
            }
        }
        #endregion

        #region ダイアログ画面
        private int _dialogButtonHeight = 22;
        private int _dialogCheckBoxSize = 30;
        public int DialogButtonHeight
        {
            get => _dialogButtonHeight;
            set
            {
                _dialogButtonHeight = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DialogButtonHeight)));
            }
        }
        public int DialogCheckBoxSize
        {
            get => _dialogCheckBoxSize;
            set
            {
                _dialogCheckBoxSize = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DialogCheckBoxSize)));
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class CommandViewModel : BindableBase
    {
        private static readonly CommandViewModel _instance = new CommandViewModel();
        public static CommandViewModel Instance { get => _instance; }

        //ウィンドウを閉じる用Action
        public Action CloseWindowAction { get; set; }

        private bool _isDialogOpen = false;
        public bool IsDialogOpen
        {
            get { return _isDialogOpen; }
            set { SetProperty(ref _isDialogOpen, value); }
        }

        private bool _isTaskBarPollingRun = true;
        public bool IsTaskBarPollingRun
        {
            get { return _isTaskBarPollingRun; }
            set { SetProperty(ref _isTaskBarPollingRun, value); }
        }

        private bool _isTaskBarHide = false;
        public bool IsTaskBarHide
        {
            get 
            {
                VerifyTaskBarHide();
                return _isTaskBarHide;
            }
            set 
            {
                SetProperty(ref _isTaskBarHide, value);
                HideTaskbar(value); 
            }
        }
        public void VerifyTaskBarHide()
        {
            if( _isTaskBarHide != WinUIAPI.IsTaskbarHide() )
                HideTaskbar(_isTaskBarHide);
        }

        //private bool _isTaskBarAutoHide = true;
        public bool IsTaskBarAutoHide
        {
            get { return WinUIAPI.IsTaskbarAutoHide(); }
            set 
            {
                //SetProperty(ref _isTaskBarAutoHide, value);
                AutoHideTaskbar(value); 
            }
        }
        private string _autoHideVisibility = "Visible";
        public string AutoHideVisibility
        {
            get 
            {
                if (ConstParams.IsWindows11())
                    _autoHideVisibility = "Collapsed"; //@@TEST Win11のAutoHideはレジストリをいじるので起動時のみに限定
                else
                    _autoHideVisibility = "Visible";

                return _autoHideVisibility; 
            }
            //set { SetProperty(ref _isAutoHideEnable, value); }
        }

        // オプションダイアログ
        private DelegateCommand<string> _showOptionCommand;
        public DelegateCommand<string> ShowOptionCommand =>
            _showOptionCommand ?? (_showOptionCommand = new DelegateCommand<string>(ShowOptionDialog));
        // オプションダイアログ表示
        private async void ShowOptionDialog(string parameter)
        {
            if(IsDialogOpen != true) 
            {
                var result = await DialogHost.Show(new OptionDialog());

                if ((string)result == "OK")
                {
                    //
                }
            }
        }

        // バージョン情報ダイアログ
        private DelegateCommand<string> _showVersionCommand;
        public DelegateCommand<string> ShowVersionCommand =>
            _showVersionCommand ?? (_showVersionCommand = new DelegateCommand<string>(ShowVersionDialog));
        // バージョン情報ダイアログ表示
        private async void ShowVersionDialog(string parameter)
        {
            if (IsDialogOpen != true)
            {
                var result = await DialogHost.Show(new VersionDialog());

                if ((string)result == "OK")
                {
                    //
                }
            }
        }

        // 終了ダイアログ
        private DelegateCommand<String> _showCloseCommand;
        public DelegateCommand<string> ShowCloseCommand =>
            _showCloseCommand ?? (_showCloseCommand = new DelegateCommand<string>(ShowCloseDialog));
        // 終了ダイアログ表示
        private async void ShowCloseDialog(string parameter)
        {
            if (IsDialogOpen != true)
            {
                var result = await DialogHost.Show(new CloseDialog());

                if ((string)result == "終了する")
                {
                    _isTaskBarPollingRun = false;
                    CloseWindowAction();
                }
            }
        }

        // タスクバー非表示
        private DelegateCommand<object> _hideTaskbarCommand;
        public DelegateCommand<object> HideTaskbarCommand =>
            _hideTaskbarCommand ?? (_hideTaskbarCommand = new DelegateCommand<object>(HideTaskbar));
        // タスクバー非表示実行
        private void HideTaskbar(object parameter)
        {
            if ((bool)parameter)
            {
                WinUIAPI.TskBarHide();
            }
            else
            {
                WinUIAPI.TskBarDisp();
            }
        }

        // タスクバーを自動的に隠す
        private DelegateCommand<object> _autoHideTaskbarCommand;
        public DelegateCommand<object> AutoHideTaskbarCommand =>
            _autoHideTaskbarCommand ?? (_autoHideTaskbarCommand = new DelegateCommand<object>(AutoHideTaskbar));
        // タスクバーを自動的に隠す
        private void AutoHideTaskbar(object parameter)
        {
            if (ConstParams.IsWindows11())
                AutoHide((bool)parameter);
            else
                WinUIAPI.TskBarAutoHide((bool)parameter);
        }

        // 強制終了コマンド
        private DelegateCommand<object> _forceCloseCommand;
        public DelegateCommand<object> ForceCloseCommand =>
            _forceCloseCommand ?? (_forceCloseCommand = new DelegateCommand<object>(ForceClose));
        // 強制終了処理
        private void ForceClose(object parameter) 
        {
            _isTaskBarPollingRun = false;
            CloseWindowAction();
        }

        // Windows11用-レジストリを書き換えてエクスプローラを再起動(強制中断) //@@TEST TaskBarAutoHide 一先ずここに実装
        public static void AutoHide(bool enable)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StuckRects3", true))
            {
                if (key != null)
                {
                    byte[] data = (byte[])key.GetValue("Settings");
                    if (data != null)
                    {
                        // Settingsの9バイト目のLSBを1(7a->7b)にすることで自動的に隠す設定が有効になる
                        int buf = ~0x01;
                        byte compByte = (byte)buf;
                        if (enable)
                            data[8] |= 0x01;
                        else
                            data[8] &= compByte;

                        key.SetValue("Settings", data);

                        // explorer.exeの再起動
                        RestartExplorer();
                    }
                }
            }
        }

        public static void RestartExplorer()
        {
            // 一旦終了させる
            foreach (Process exp in Process.GetProcessesByName("explorer"))
            {
                try
                {
                    // explorerのプロセスを中断
                    exp.Kill();
                }
                // 終了できなかった場合の処理
                catch (Exception)
                {
                    // タスクを強制終了
                    Process.Start("taskkill", "/F /PID " + exp.Id);
                    // 少し待つ
                    System.Threading.Thread.Sleep(500);
                    // explorer.exeを起動する
                    Process explorer = new Process();
                    explorer.StartInfo.FileName = "explorer.exe";
                    //explorer.StartInfo.Arguments = "::";
                    //explorer.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    //explorer.StartInfo.CreateNoWindow = true;
                    //explorer.StartInfo.UseShellExecute = false;
                    //explorer.StartInfo.RedirectStandardOutput = true;

                    explorer.Start();
                    explorer.Kill(); // Windowの再表示を防ぐ為、起動直後にプロセスを中断
                }
            }
        }
    }
}
