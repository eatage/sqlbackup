using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections;

namespace sqlbackup
{
    /// <summary>
    /// 日志记录帮助类
    /// </summary>
    public class LogHelper
    {
        //static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "smssys\\");
        static string logPath = Path.Combine("D:\\log\\", "smssys2014\\");

        static SourceLevels level = SourceLevels.All;
        public static string LogPath
        {
            get { return LogHelper.logPath; }
            set { LogHelper.logPath = value; }
        }
        static Hashtable tsTable = new Hashtable();
        //static TraceSource ts = new TraceSource("defaultTs", SourceLevels.All);
        public static void Log(string category, TraceEventType type, int eventId, string msg)
        {
            if (!tsTable.ContainsKey(category))
            {
                lock (tsTable.SyncRoot)
                {
                    if (!tsTable.ContainsKey(category))
                    {
                        tsTable.Add(category, new TraceSource(category, level));
                    }
                }
            }

            TraceSource ts = tsTable[category] as TraceSource;
            lock (ts)
            {
                string name = String.Format("{0}_{1}.txt", DateTime.Now.ToString("yyyyMMdd"), category);
                if (ts.Listeners.Count > 0 && !(ts.Listeners[0] is TextWriterTraceListener))
                {
                    ts.Flush();
                    ts.Close();
                    ts.Listeners.Clear();
                }
                if (ts.Listeners.Count == 0)
                {
                    TextWriterTraceListener listener = new TextWriterTraceListener(Path.Combine(logPath, name));
                    listener.TraceOutputOptions = TraceOptions.None;   //| TraceOptions.DateTime;
                    listener.Name = name;
                    ts.Listeners.Add(listener);
                }
                if (ts.Listeners[0].Name != name)
                {
                    ts.Flush();
                    ts.Close();
                    ts.Listeners.Clear();
                    TextWriterTraceListener listener = new TextWriterTraceListener(Path.Combine(logPath, name));
                    listener.TraceOutputOptions = TraceOptions.None;   // | TraceOptions.DateTime;
                    listener.Name = name;
                    ts.Listeners.Add(listener);
                }
                ts.TraceEvent(type, eventId, msg);
                ts.Flush();
            }
        }
        public static void Release()
        {
            lock (tsTable.SyncRoot)
            {
                tsTable.Clear();
            }
        }
        #region default log
        public static void Log(TraceEventType type, int eventId, string msg)
        {
            Log("default", type, eventId, msg);
        }
        public static void Log(string msg)
        {
            Log(TraceEventType.Information, 1, msg);
        }
        public static void Log(string format, params object[] paramters)
        {
            Log(String.Format(format, paramters));
        }
        public static void Log(TraceEventType type, string msg)
        {
            Log(type, 1, msg);
        }
        public static void Log(TraceEventType type, string format, params object[] paramters)
        {
            Log(type, String.Format(format, paramters));
        }
        #endregion
        #region 指定og文件前缀的方法
        public static void Log(string category, string msg)
        {
            Log(category, TraceEventType.Information, 1, msg);
        }
        public static void Log(string category, TraceEventType type, string msg)
        {
            Log(category, type, 1, msg);
        }
        public static void Log(string category, TraceEventType type, string format, params object[] paramters)
        {
            Log(category, type, 1, String.Format(format, paramters));
        }
        public static void Log(string category, string format, params object[] paramters)
        {
            Log(category, TraceEventType.Information, format, paramters);
        }
        #endregion
        #region 指定日志级别和文件前缀的方法
        public static void LogVerbose(string category, string msg)
        {
            Log(category, TraceEventType.Verbose, msg);
        }
        public static void LogWarning(string category, string msg)
        {
            Log(category, TraceEventType.Warning, msg);
        }
        public static void LogError(string category, string msg)
        {
            Log(category, TraceEventType.Error, msg);
        }
        public static void LogCritical(string category, string msg)
        {
            Log(category, TraceEventType.Critical, msg);
        }
        #endregion
    }

    ///<summary>
    ///程序时间测量帮助类
    ///</summary>
    ///<remarks>
    ///使用StreamWriter来记录文件,不再使用后需要调用Close方法关闭文件，Close会调用Flow（）方法
    ///</remarks>
    public class MeasureHelper : IDisposable
    {
        string splitDatetimeFormat = "yyyy-MM-dd_HH-mm-ss-fff";
        int line = 0;
        StreamWriter writer = null;
        bool isDispoed = false;
        static string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "smssys\\");

        public static void Log(string name, string[] paramters)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd-HH");
            string fullNameFormat = System.IO.Path.Combine(logPath, "{0}_{1}.csv");

            StreamWriter ret = null;
            FileInfo file = new FileInfo(String.Format(fullNameFormat, name, now));
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            if (file.Directory.Exists)
            {
                ret = new StreamWriter(String.Format(fullNameFormat, name, now), false, Encoding.GetEncoding(936));
            }
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("{0}", DateTime.Now.ToString("HH:mm:ss.fff")));
            foreach (string v in paramters)
            {
                sb.Append(v.ToString());
                sb.Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            ret.WriteLine(sb.ToString());
            ret.Close();
            ret.Dispose();
        }



        /// <summary>
        /// 时间分片的格式字符串(见<see cref="System.Datetime.ToString()"/>).默认为yyyy-MM-dd_HH-mm-ss
        /// </summary>
        public string SplitDatetimeFormat
        {
            get { return splitDatetimeFormat; }
            set { splitDatetimeFormat = value; }
        }

        /// <summary>
        /// 获取或设置是否自动提交.默认为True
        /// </summary>
        public bool AutoFlush
        {
            get
            {
                return writer == null ? false : writer.AutoFlush;
            }
            set
            {
                if (writer != null)
                {
                    writer.AutoFlush = value;
                }
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称（文件名前缀）</param>
        public MeasureHelper(string name)
            : this(name, null) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称（文件名前缀）</param>
        /// <param name="headLine">首行</param>
        public MeasureHelper(string name, string[] columnNames)
            : this(name, columnNames, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log\\")) { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称（文件名前缀）</param>
        /// <param name="headLine">首行</param>
        /// <param name="path">日志的路径（如果不存在会自动创建，因此请确保程序对制定的路径有创建权限）</param>
        public MeasureHelper(string name, string[] columnNames, string path)
        {
            string now = DateTime.Now.ToString("yyyy-MM-dd-HH");
            string fullNameFormat = System.IO.Path.Combine(path, "{0}_{1}.csv");
            writer = CreateStreamWriter(String.Format(fullNameFormat, name, now));
            writer.AutoFlush = true;
            if (columnNames != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("序号,时间,");
                foreach (string s in columnNames)
                {
                    sb.Append(s);
                    sb.Append(",");
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                writer.WriteLine(sb.ToString());
            }
        }
        /// <summary>
        /// 写记录
        /// </summary>
        /// <param name="msg">信息</param>
        public void WriteLog(params string[] paramters)
        {
            if (isDispoed)
            {
                throw new Exception("MeasureHelper is dispoed!");
            }
            try
            {
                if (paramters != null)
                {
                    int l = Interlocked.Increment(ref line);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(String.Format("{0},{1},", l, DateTime.Now.ToString("HH:mm:ss.fff")));
                    foreach (string v in paramters)
                    {
                        sb.Append(v.ToString());
                        sb.Append(",");
                    }
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
                    writer.WriteLine(sb.ToString());
                }


                //writer.WriteLine(msg);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        private StreamWriter CreateStreamWriter(string fullName)
        {
            StreamWriter ret = null;
            FileInfo file = new FileInfo(fullName);
            if (!file.Directory.Exists)
            {
                file.Directory.Create();
            }
            if (file.Directory.Exists)
            {
                ret = new StreamWriter(fullName, false, Encoding.GetEncoding(936));
            }
            return ret;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!isDispoed)
            {
                isDispoed = true;
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                    writer = null;
                }
            }
        }
        /// <summary>
        /// 关闭并释放所有资源
        /// </summary>
        public void Close()
        {
            Dispose();
        }

        #endregion
    }
}
