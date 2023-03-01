using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using TVmeetLauncher.Properties;

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
        // 圧縮中フラグ
        private bool isCompressing = false;
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

        /// ログ取得実行フラグ
        public bool IsLogging { get; set; }
        /// ログ出力ディレクトリ
        public string LogFileDir { get; set; }
        /// 圧縮ログ出力ディレクトリ
        public string LogOldFileDir { get; set; }
        /// ログファイルプリフィックス名
        public string LogFileName { get; set; }
        /// ログファイル名のフォーマット
        public LogFormatFileNameType LogFormatFileName { get; set; }
        /// ログファイルのフルパス
        public string LogFileFullPath { get; set; }
        /// ログファイルの書込モード
        public LogWriteModeType LogWriteMode { get; set; }
        /// ログファイルの既定出力レベル
        public LogLevel LogDefaultLevel { get; set; }
        /// ログの寿命(日)
        public int LogLifeSpan { get; set; }
        /// ログの最大サイズ(Byte)
        public int LogFileMaxSize { get; set; }
        /// エンコード
        public string Encode { get; set; }
        #endregion

        #region "コンストラクタ"
        private Logger()
        {
            /// 各プロパティに初期値を設定
            // ログ取得実行フラグ
            IsLogging = Settings.Default.IS_LOGGING; 
            // ログ出力先ディレクトリ
            LogFileDir = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, Settings.Default.LOGDIR_PATH); //"\\log";
            // 圧縮ログ出力ディレクトリ
            LogOldFileDir = Path.Combine(LogFileDir,Settings.Default.LOGOLDDIR_PATH);
            // ログファイルプリフィックス名
            LogFileName = Settings.Default.LOGFILE_NAME; //"Log";
            // ログファイル名のフォーマット
            LogFormatFileName = LogFormatFileNameType.YYYYMMDD;
            // ログファイルのフルパス
            LogFileFullPath = Path.Combine(LogFileDir, CreateLogFilePath(LogFileName));
            // ログファイルの書込モード
            LogWriteMode = Settings.Default.LOG_ISAPPEND ? LogWriteModeType.Append : LogWriteModeType.Over;
            // ログファイルの既定出力レベル
            LogDefaultLevel = (LogLevel)Settings.Default.LOG_LEVEL; //LogLevel.Info;
            // ログの寿命(日)
            LogLifeSpan = Settings.Default.LOGFILE_PERIOD; //30;
            if (LogLifeSpan < 0)
                LogLifeSpan = 0;
            // ログの最大サイズ(Byte)
            LogFileMaxSize = Settings.Default.LOGFILE_MAXSIZE; // 1048576(1MB)
            if (LogFileMaxSize < 0)
                IsLogging = false;
            // エンコード
            Encode = "utf-8"; //"shift_jis";

            // 古いログファイルを削除
            CheckOldLogfile();
        }
#endregion

#region "公開メソッド"
        /// <summary>
        /// ログファイル書き込み
        /// </summary>
        /// <param name="logMsg">ログ内容</param>
        /// <param name="writeLogLevel">書込ログレベル</param>
        public bool WriteLog(string logMsg, LogLevel writeLogLevel)
        {
            if(!IsLogging)
                return true;

            try
            {
                // ログ出力文字列作成
                string LogString = CreateLogString(logMsg, writeLogLevel);

                // 書込ディレクトリが無ければ、作成
                if (!Directory.Exists(LogFileDir))
                    Directory.CreateDirectory(LogFileDir);

                // ログ書込モードによって[追記/上書]を行う
                using (StreamWriter Fs =
                    new StreamWriter(LogFileFullPath, Convert.ToBoolean(LogWriteMode == LogWriteModeType.Append), Encoding.GetEncoding(Encode)))
                {
                    System.Diagnostics.Debug.Write(LogString);  // @@TEST for DEBUG
                    Fs.Write(LogString);
                    Fs.Close();

                    FileInfo logFile = new FileInfo(LogFileFullPath);
                    if (LogFileMaxSize < logFile.Length && !isCompressing)
                    {
                        isCompressing = true;
                        try
                        {
                            // ログファイルを圧縮する
                            CompressLogFile();
                            // 古いログファイルを削除する
                            CheckOldLogfile();
                        }
                        finally
                        {
                            isCompressing = false;
                        }
                    }
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

#region "内部メソッド"
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
        /// <summary>
        /// ログファイルを圧縮する
        /// </summary>
        private void CompressLogFile()
        {
            // 圧縮ファイル用ディレクトリが無ければ、作成
            if (!Directory.Exists(LogOldFileDir))
                Directory.CreateDirectory(LogOldFileDir);

            string oldFilePath = Path.Combine( LogOldFileDir, LogFileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") );
            File.Move( LogFileFullPath, oldFilePath + ".log");

            FileStream inStream = new FileStream(oldFilePath + ".log", FileMode.Open, FileAccess.Read);
            FileStream outStream = new FileStream(oldFilePath + ".gz", FileMode.Create, FileAccess.Write);
            GZipStream gzStream = new GZipStream(outStream, CompressionMode.Compress);

            int size;
            byte[] buffer = new byte[LogFileMaxSize + 1000];
            while (0 < (size = inStream.Read(buffer, 0, buffer.Length)))
            {
                gzStream.Write(buffer, 0, size);
            }
            
            inStream.Close();
            gzStream.Close();
            outStream.Close();

            WriteLog("Compressed [" + LogFileFullPath + "] as [" + oldFilePath + ".log]", LogLevel.Info);

            File.Delete(oldFilePath + ".log");
        }
        /// <summary>
        /// 古いログファイルの削除
        /// </summary>
        /// <returns></returns>
        private bool CheckOldLogfile()
        {
                // ログフォルダのファイルを削除
                bool resLog = DeleteFiles(LogFileDir);
                // 圧縮フォルダのファイル削除
                bool resComp = DeleteFiles(LogOldFileDir);

                return resLog & resComp;
        }
        private bool DeleteFiles(string dir)
        {
            try
            {
                // ディレクトリが無ければ正常終了
                if (!Directory.Exists(dir))
                    return true;

                // フォルダ内ファイルを削除
                DirectoryInfo dyInfo = new DirectoryInfo(dir);
                var target = DateTime.Today.AddDays(-LogLifeSpan);
                foreach (FileInfo fInfo in dyInfo.GetFiles())
                {
                    if (fInfo.LastWriteTime < target)
                        fInfo.Delete();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
#endregion
}
