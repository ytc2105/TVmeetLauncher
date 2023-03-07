using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace TVmeetLauncher
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SplashScreen : Window
    {

        //public LauncherWindow launcherWindow;
        private bool IsClosed = false;

        public SplashScreen()
        {
            InitializeComponent();
            Closed += (s, e) => { IsClosed = true; };
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
            // プログレス非同期表示後にランチャウィンドウ生成
            //launcherWindow = new LauncherWindow();
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i <=1024; i++)
            {
                if (IsClosed)
                    return;

                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(10);
            }
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (IsClosed) 
                return;

            // プログレスバー進捗描画用
            //progressBar.Value = e.ProgressPercentage;

            // 進捗100% & ランチャ準備完了
            if (e.ProgressPercentage == 100 && BaseViewModel.Instance.IsLauncherReady)
            {
                //launcherWindow.Show();
                Close();
            }
            // タイムアウトで強制終了
            else if (e.ProgressPercentage == 1024)
            {
                throw new Exception("ミーティングアプリの読み込みがタイムアウトしました。"); //@@TEST 強制終了用、App.xaml.csでキャッチ
            }
        }

        public void CloseEvent(object sender, EventArgs e)
        {
            Close();
        }
    }
}
