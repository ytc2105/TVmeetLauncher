﻿using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using WIN32API;

namespace TVmeetLauncher
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        public static BaseViewModel Instance { get; } = new BaseViewModel();

        /// <summary>
        /// 共通
        /// </summary>
        private int _headlineFontSize = 20;
        private int _subtitleFontSize = 14;
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

        /// <summary>
        /// 背景画面―ロゴ類
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
        /// アプリ画面―タイトルバー
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
        /// スプラッシュ画面
        /// </summary>
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

        /// <summary>
        /// ダイアログ画面
        /// </summary>
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

        /// <summary>
        /// メイン画面ロゴ可視ステータス
        /// </summary>
        private string _componentsVisibility = "Hidden";
        public string ComponntsVisibility
        {
            get => _componentsVisibility;
            set
            {
                _componentsVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ComponntsVisibility)));
            }
        }

        /// <summary>
        /// ランチャ画面準備完了フラグ
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

        public event PropertyChangedEventHandler PropertyChanged;

        //ウィンドウを閉じる用Action
        public Action CloseWindowAction { get; set; }
    }

    public class CommandViewModel : BindableBase
    {
        private static readonly CommandViewModel _instance = new CommandViewModel();
        public static CommandViewModel Instance { get => _instance; }

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

        private bool _isTaskBarHide = true;
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
                    BaseViewModel.Instance.CloseWindowAction();
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
            WinUIAPI.TskBarAutoHide((bool)parameter);
        }
    }
}
