using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WIN32API;

namespace TVmeetLauncher
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        const string ApplicationId = "C6CEF670-7D2B-4BA6-879D-4DC5CEB56FF6";
        const string HandlerName = "handler";

        [STAThread]
        public static void Main(string[] args)
        {
            using (var semaphore = new Semaphore(1, 1, ApplicationId, out bool createdNew))
            {
                if (createdNew)
                {
                    ChannelServices.RegisterChannel(new IpcServerChannel(ApplicationId), true);
                    RemotingServices.Marshal(new Handler(), HandlerName, typeof(Handler));

                    var app = new App();
                    app.InitializeComponent();
                    app.Run();
                }
                else
                {
                    ChannelServices.RegisterChannel(new IpcClientChannel(), true);
                    ((Handler)Activator.GetObject(typeof(Handler), "ipc://" + ApplicationId + "/" + HandlerName)).Handle();
                }
            }
        }

        private class Handler : MarshalByRefObject
        {
            public void Handle()
            {
                Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (Current.MainWindow.WindowState == WindowState.Minimized)
                        Current.MainWindow.WindowState = WindowState.Normal;
                    else
                        Current.MainWindow.Activate();
                }));
            }

            public override object InitializeLifetimeService() => null;
        }

        //全例外処理の監視
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // UIスレッドの未処理例外で発生
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            // UIスレッド以外の未処理例外で発生
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
            // それでも処理されない例外で発生
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exception = e.Exception;
            HandleException(exception);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            var exception = e.Exception.InnerException;
            HandleException(exception);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            HandleException(exception);
        }

        private void HandleException(Exception e)
        {
            // ログ出力
#if DEBUG
            MessageBox.Show($"Handle Exception occured.\n--ERROR CONTENTS--\n{e?.ToString()}");
#endif
            MessageBox.Show($"申し訳ありません。\nアプリケーション実行中に異常を検知したため\nテレビ会議システムを終了します。\n\n--異常メッセージ--\n{e?.Message}", "異常終了");
            Logger.GetInstance.WriteLog($"Handle Exception occured. | {e?.ToString()}", Logger.LogLevel.Fatal);
            
            CommandViewModel.Instance.IsTaskBarPollingRun = false;
            WinUIAPI.TskBarDisp();
            WinUIAPI.TskBarAutoHide(false);
            Logger.GetInstance.WriteLog($"Terminate \"TV Meeting Launcher\".", Logger.LogLevel.Fatal);
            Environment.Exit(1);
        }

    }
}