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

        public SplashScreen()
        {
            InitializeComponent();
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            //launcherWindow = new LauncherWindow();
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i <= 100; i++)
            {
                (sender as BackgroundWorker).ReportProgress(i);
                Thread.Sleep(10);
            }
        }

        void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;

            if (e.ProgressPercentage >= 100 && BaseViewModel.Instance.IsLauncherReady)
            {
                Close();
                //launcherWindow.Show();
            }
        }

        public void CloseEvent(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
