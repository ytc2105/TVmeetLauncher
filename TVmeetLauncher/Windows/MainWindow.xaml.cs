using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using WIN32API;

namespace TVmeetLauncher
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            // モニタ解像度取得
            var rect = new System.Drawing.Rectangle((int)this.Left, (int)this.Top, (int)this.Width, (int)this.Height);
            var screen = System.Windows.Forms.Screen.FromRectangle(rect);
            int width = screen.Bounds.Width;
            //aint height = screen.Bounds.Height;
            ConstParams.ResolWidth = width;
            Logger.GetInstance.WriteLog("Start TVmeetLauncher.");

            InitializeComponent();
            WinUIAPI.TskBarHide();
            WinUIAPI.TskBarAutoHide(true);
        }

        private void MetroWindow_ContentRendered(object sender, System.EventArgs e)
        {
            // Window拡大率取得
            ConstParams.WinScale = this.GetDpiScaleWidth();
            // 画面内拡大倍率一括設定
            PropertyInfo[] properties = BaseViewModel.Instance.GetType().GetProperties();
            var propertiesInt = properties.Where(w => w.PropertyType == typeof(int));
            foreach(PropertyInfo propertyInfo in propertiesInt)
            {
                double value = (double)(int)propertyInfo.GetValue(BaseViewModel.Instance);
                propertyInfo.SetValue(BaseViewModel.Instance, (int)(value * ConstParams.ViewScale));
            }
            //メイン画面ロゴ表示
            BaseViewModel.Instance.ComponntsVisibility = "Visible";
            // 画面描画
            LauncherWindow launcherWindow = new LauncherWindow();
#if !DEBUG
            SplashScreen splashScreen = new SplashScreen();
            splashScreen.ShowDialog();
#endif
            launcherWindow.Show();
            // ランチャ画面を閉じたらメイン画面も閉じる
            launcherWindow.Closed += new EventHandler(this.CloseEvent);
            // ランチャ画面の終了処理を、終了ボタン押下時のアクションコマンドへ追加
            BaseViewModel.Instance.CloseWindowAction = new Action(launcherWindow.Close);
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            CommandViewModel.Instance.IsTaskBarPollingRun = false;
            WinUIAPI.TskBarDisp();
            WinUIAPI.TskBarAutoHide(false);
            Logger.GetInstance.WriteLog("Exit TVmeetLauncher.");
        }

        private void MetroWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
#if !DEBUG
            // desable Alt+F4
            if (e.Key == Key.System && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
#endif
        }

        private void CloseEvent(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}