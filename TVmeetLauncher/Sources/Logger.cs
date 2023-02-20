using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVmeetLauncher
{
    /// <summary>
    /// ログ出力クラス
    /// </summary>
    /// ログ出力を行う。※インスタンス化不要。公開プロパティ[Instance]経由でアクセス
    internal class Logger
    {
        #region "メンバー変数"
        // シングルトンクラスアクセス用変数
        private static readonly Logger instance = new Logger();
        #endregion

        #region "公開列挙型"
        /// <summary>ログ出力レベル</summary>
        public enum LogLevel
        {
            Debug = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
            Fatal = 5,
            None = 9
        }

        /// <summary>ログ書込モード：Append=追記/Over=上書き</summary>
        public enum LogWriteModeType
        {
            Append,
            Over
        }

        /// <summary>ログファイル名フォーマット</summary>
        public enum LogFormatFileNameType
        {
            YYYYMMDD,
            YYYYMMDDHHMMSS,
            YYYYMMDDHHMMSSFFF,
            None
        }
        #endregion

        #region "公開プロパティ"
        /// クラスインスタンスアクセス用
        public static Logger GetInstance
        {
            get { return instance; }
        }

        /// ログ出力ディレクトリ
        public string LogFileDir { get; set; }

        /// ログファイル名
        public string LogFileName { get; set; }

        /// ログファイルの書込モード
        public LogWriteModeType LogWriteMode { get; set; }

        /// ログファイル名のフォーマット
        public LogFormatFileNameType LogFormatFileName { get; set; }

        /// ログファイルの既定出力レベル
        public LogLevel LogDefaultLevel { get; set; }

        /// ログの寿命(日)
        public int LogLifeSpan { get; set; }

        /// エンコード
        public string Encode { get; set; }

        #endregion

        #region "コンストラクタ"
        private Logger()
        {
            // 各プロパティに初期値を設定
            // ログ出力先ディレクトリ
            LogFileDir = System.AppDomain.CurrentDomain.BaseDirectory + "\\log";
            // ログファイル名
            LogFileName = "Log";
            // ログファイルの書込モード
            LogWriteMode = LogWriteModeType.Append;
            // ログファイル名のフォーマット
            LogFormatFileName = LogFormatFileNameType.YYYYMMDD;
            // ログファイルの既定出力レベル
            LogDefaultLevel = LogLevel.Info;
            // ログの寿命(日)
            LogLifeSpan = 30;
            // エンコード
            Encode = "utf-8"; //"shift_jis";
        }
        #endregion

        #region "公開メソッド"
        #region "ログファイル書き込み"
        /// <summary>
        /// ログファイル書き込み
        /// </summary>
        /// <param name="logMsg">ログ内容</param>
        /// <param name="writeLogLevel">書込ログレベル</param>
        public bool WriteLog(string logMsg, LogLevel writeLogLevel)
        {
            try
            {
                // ログ出力文字列作成
                string LogString = CreateLogString(logMsg, writeLogLevel);

                // 書込ディレクトリが無ければ、作成
                if (!Directory.Exists(LogFileDir))
                {
                    Directory.CreateDirectory(LogFileDir);
                }

                // ログ書込モードによって[追記/上書]を行う
                using (StreamWriter Fs = new StreamWriter(Path.Combine(LogFileDir, CreateLogFilePath(LogFileName)), Convert.ToBoolean(LogWriteMode == LogWriteModeType.Append ? true : false), System.Text.Encoding.GetEncoding(Encode)))
                {
                    System.Diagnostics.Debug.Write(LogString);  // @@TEST for DEBUG
                    Fs.Write(LogString);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// ログファイル-デフォルトレベル書き込み
        /// </summary>
        /// <param name="logMsg">ログ内容</param>
        public bool WriteLog(string logMsg)
        {
            return WriteLog(logMsg, LogDefaultLevel);
        }
        #endregion


        #region "古いログファイルの削除"
        /// <summary>
        /// 古いログファイルの削除
        /// </summary>
        /// <returns></returns>
        public bool CheckOldLogfile()
        {
            try
            {
                // ディレクトリが無ければ正常終了
                if (!Directory.Exists(LogFileDir))
                    return true;

                DirectoryInfo dyInfo = new DirectoryInfo(LogFileDir);
                // フォルダのファイルを取得
                var target = DateTime.Today.AddDays(-LogLifeSpan);
                foreach (FileInfo fInfo in dyInfo.GetFiles())
                {
                    // 日付の比較
                    if (fInfo.LastWriteTime < target)
                    {
                        fInfo.Delete();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
        #endregion

        #region "内部メソッド"
        #region "ログ出力文字列作成"
        /// <summary>
        /// ログ出力文字列作成
        /// </summary>
        /// <param name="logMsg">ログ内容</param>
        /// <param name="writeLogLevel">書込ログレベル</param>
        private string CreateLogString(string logMsg, LogLevel writeLogLevel)
        {
            string logTemplate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff") + "\t" + (" 【" + writeLogLevel.ToString() + "】").PadRight(7, ' ') + "\t" + "{0}" + "\r\n";

            // ログ文字列
            string logString = string.Format(logTemplate, logMsg);
            return logString;
        }
        #endregion

        #region "ログファイルパス設定"
        /// <summary>
        /// ログファイルパス設定
        /// </summary>
        /// <param name="logFileName">ログファイル名</param>
        private string CreateLogFilePath(string logFileName)
        {
            switch (LogFormatFileName)
            {
                case LogFormatFileNameType.YYYYMMDD:
                    logFileName += "_" + DateTime.Now.ToString("yyyyMMdd");
                    break;
                case LogFormatFileNameType.YYYYMMDDHHMMSS:
                    logFileName += "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    break;
                case LogFormatFileNameType.YYYYMMDDHHMMSSFFF:
                    logFileName += "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    break;
                case LogFormatFileNameType.None:
                    break;
            }

            return logFileName + ".log";
        }
        #endregion
        #endregion
    }
}
