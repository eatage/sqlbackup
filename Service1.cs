using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using sqlbackup.Core;
using sqlbackup;
using baseclass;

namespace sqlbackup
{

    public partial class Servicesqlbackup : ServiceBase
    {
        bool ib_stop = false;
        bool ib_Ftp1Upload = false;
        bool ib_Ftp2Upload = false;
        string is_web = ConfigurationManager.AppSettings["web"];
        string is_logPath = ConfigurationManager.AppSettings["logpath"];

        public Servicesqlbackup()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ib_stop = false;
            ThreadPool.SetMaxThreads(80, 80);

            LogHelper.LogWarning("服务启动开始日志", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "--启动");

            //创建日志输出目录
            if (!Directory.Exists(is_logPath))
            {
                Directory.CreateDirectory(is_logPath);
            }

            //备份的线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(vf_backupDB), 1);

            //备份到FTP1服务器线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(vf_Ftp1), 1);

            //备份到FTP2服务器线程
            ThreadPool.QueueUserWorkItem(new WaitCallback(vf_Ftp2), 1);

            LogHelper.LogWarning("服务启动开始日志", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "--启动成功");

        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。

            ib_stop = true;

            GC.Collect();
            LogHelper.LogWarning("服务启动关闭日志", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "--关闭");
        }


        //备分数据库
        private void vf_backupDB(object a)
        {
            Tools.WriteLog("Log", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "进入到备份系统中  ");

            //判断当前时间是否需要备份
            //List<string> list = INIFile.ReadKeyValues("backuptime");
            List<string> list = SysVisitor.Current.ReadKeyValues("backuptime");

            string[] ls_timelist = ListToTime(list, "time");//获取备份时间设置里的时间列表
            string[] ls_waylist = ListToTime(list, "way");//获取备份时间设置里的备份方式列表
            int li_count = 0;//备份方式


            while (!ib_stop)
            {

                try
                {
                    bool OK = false;

                    for (int i = 0; i < list.Count; i++)
                    {
                        DateTime dt1 = Convert.ToDateTime(ls_timelist[i]);
                        TimeSpan time = dt1 - DateTime.Now;
                        if (time.Minutes < 1 && time.Minutes > -1)//指定时间与当前时间差值 test30分钟
                        {
                            OK = true;
                            li_count = Convert.ToInt32(ls_waylist[i]);
                            break;
                        }
                    }   //for

                    if (OK)//执行备份
                    {
                        List<string> ls_backDBlist = BackupDB.BackupDBList();
                        for (int i = 0; i < ls_backDBlist.Count; i++)
                        {
                            string FileName = BackupDB.BackupDBNow(ls_backDBlist[i], li_count);//执行备份
                            string rarFileName = BackupDB.BakTORar(FileName, ls_backDBlist[i]);//执行压缩
                            //INIFile.SetINIString("IfFtpUpLoad", rarFileName, "1,1");
                            SysVisitor.Current.of_SetMySysSet("IfFtpUpLoad", rarFileName, "1,1");
                        }
                        //BackupDB.BakTORar(ls_backDBlist[i]);
                        //INIFile.SetINIString("localbackup", "LastTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//将备份时间写入ini
                        SysVisitor.Current.of_SetMySysSet("localbackup", "LastTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));//将备份时间写入ini
                        Tools.WriteLog("Backup", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 备份成功并压缩,备份类型为" + li_count);

                        DelTolastDay();

                        Thread.Sleep(1000 * 60);  //停1分钟
                    }
                    else
                    {
                        Thread.Sleep(1000 * 60);  //停1分钟
                    }
                }
                catch (Exception ex)
                {
                    Tools.WriteLog("error", "2-备份 " + ex.ToString());
                }
            }
            return;
        }


        //ftp到一号服务器
        private void vf_Ftp1(object a)
        {
            Tools.WriteLog("Log", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "进入FTP1");
            //检查是否有未上传的文件
            while (!ib_Ftp1Upload)
            {
                bool OK = false;
                //List<string> list = INIFile.ReadKeyValues("IfFtpUpLoad");
                List<string> list = SysVisitor.Current.ReadKeyValues("IfFtpUpLoad");
                List<string> ls_Ftplist = ListToUploadList(list, "Ftp1");//获取待上传文件列表
                if (ls_Ftplist.Count > 0)
                { OK = true; }//有文件未上传

                //获取FTP1相关参数
                string IP = SysVisitor.Current.of_GetMySysSet("FTP1", "IP");
                string UserID = SysVisitor.Current.of_GetMySysSet("FTP1", "UserID");
                string Password = SysVisitor.Current.of_GetMySysSet("FTP1", "Password");
                string Path = SysVisitor.Current.of_GetMySysSet("FTP1", "Path");
                string Port = SysVisitor.Current.of_GetMySysSet("FTP1", "Port");
                FtpWeb Ftp = new FtpWeb(IP, Path, UserID, Password);
                if (!Ftp.Connect())//FTP 不可用
                {
                    ib_Ftp1Upload = true;
                    Tools.WriteLog("error", "FTP1服务器不可用");
                    break;
                }
                if (OK)
                {
                    if (!Ftp.DirectoryExist(DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        Ftp.MakeDir(DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    Path = Path + @"\" + DateTime.Now.ToString("yyyy-MM-dd");
                    //FTP文件上传
                    FtpWeb ftp = new FtpWeb(IP, Path, UserID, Password);
                    int value = 0;//计算上传成功总数
                    for (int i = 0; i < ls_Ftplist.Count; i++)
                    {
                        int j = ftp.Upload(ls_Ftplist[i].ToString());
                        if (j == 1)//上传成功
                        {
                            value++;
                        }
                        string str = SysVisitor.Current.of_GetMySysSet("IfFtpUpLoad", ls_Ftplist[i].ToString());
                        str = str.Substring(str.Length - 1, 1);
                        SysVisitor.Current.of_SetMySysSet("IfFtpUpLoad", ls_Ftplist[i].ToString(), "0," + str);//更新节点
                    }
                    Tools.WriteLog("Ftp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "FTP1已成功上传 " + value + " 个文件，剩余待上传个数：" + (ls_Ftplist.Count - value));
                }
                Thread.Sleep(60 * 1000);//等待60秒
            }
            return;
        }


        //备份到2号服务器
        private void vf_Ftp2(object a)
        {
            Tools.WriteLog("Log", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "进入FTP2");
            //检查是否有未上传的文件
            while (!ib_Ftp2Upload)
            {
                bool OK = false;
                //List<string> list = INIFile.ReadKeyValues("IfFtpUpLoad");
                List<string> list = SysVisitor.Current.ReadKeyValues("IfFtpUpLoad");
                List<string> ls_Ftplist = ListToUploadList(list, "Ftp2");//获取待上传文件列表
                if (ls_Ftplist.Count > 0)
                { OK = true; }//有文件未上传

                //获取FTP2相关参数
                string IP = SysVisitor.Current.of_GetMySysSet("FTP2", "IP");
                string UserID = SysVisitor.Current.of_GetMySysSet("FTP2", "UserID");
                string Password = SysVisitor.Current.of_GetMySysSet("FTP2", "Password");
                string Path = SysVisitor.Current.of_GetMySysSet("FTP2", "Path");
                string Port = SysVisitor.Current.of_GetMySysSet("FTP2", "Port");
                FtpWeb Ftp = new FtpWeb(IP, Path, UserID, Password);
                if (!Ftp.Connect())//FTP 不可用
                {
                    ib_Ftp2Upload = true;
                    Tools.WriteLog("error", "FTP2服务器不可用");
                    break;
                }
                if (OK)
                {
                    if (!Ftp.DirectoryExist(DateTime.Now.ToString("yyyy-MM-dd")))
                    {
                        Ftp.MakeDir(DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    Path = Path + @"\" + DateTime.Now.ToString("yyyy-MM-dd");
                    //FTP2文件上传
                    FtpWeb ftp = new FtpWeb(IP, Path, UserID, Password);
                    int value = 0;//计算上传成功总数
                    for (int i = 0; i < ls_Ftplist.Count; i++)
                    {
                        int j = ftp.Upload(ls_Ftplist[i].ToString());
                        if (j == 1)//上传成功
                        {
                            value++;
                        }
                        string str = SysVisitor.Current.of_GetMySysSet("IfFtpUpLoad", ls_Ftplist[i].ToString());
                        str = str.Substring(str.Length - 3, 1);
                        SysVisitor.Current.of_SetMySysSet("IfFtpUpLoad", ls_Ftplist[i].ToString(), str + ",0");//更新节点

                        string ls_sql = "delete from mysysset where itemtype='IfFtpUpLoad' and itemvalue='0,0'";
                        MyConfig.ExecuteNonQuery(ls_sql);
                    }
                    Tools.WriteLog("Ftp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "FTP2已成功上传 " + value + " 个文件，剩余待上传个数：" + (ls_Ftplist.Count - value));
                }
                Thread.Sleep(60 * 1000);//等待60秒
            }
            return;

        }  //Getreport


        /// <summary>
        /// 在List数组中获取备份时间或备份方式列表
        /// </summary>
        /// <param name="list">含类如time1=10:00,2的list数组</param>
        /// <param name="type">type="time"返回时间列表，type="way"返回备份类型列表</param>
        /// <returns></returns>
        private string[] ListToTime(List<string> list, string type)
        {

            string[] backway = new string[list.Count];
            System.String[] str = list.ToArray();//List<string>list转成string[]list
            string[] aa = new string[list.Count];
            if (type == "time")
            {
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        string[] num = list[i].Split('=');
                        aa[i] = num[0].ToString();
                    }
                    catch
                    {
                        Tools.WriteLog("error", "ini中backuptime节点存在不合格式的项");//ini中存在不合格式的项
                    }
                }
            }
            if (type == "way")
            {
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        string[] num = list[i].Split('=');
                        aa[i] = num[1].ToString();
                    }
                    catch
                    {
                        Tools.WriteLog("error", "ini中backuptime节点存在不合格式的项");//ini中存在不合格式的项
                    }
                }
            }
            return aa;
        }
        /// <summary>
        /// 在List数组中获取FTP未上传文件列表 如都已经上传，则删除该键
        /// </summary>
        /// <param name="list">含类如time1=10:00,2的list数组</param>
        /// <param name="type">type="Ftp1"，type="Ftp2"</param>
        /// <returns></returns>
        private List<string> ListToUploadList(List<string> list, string type)
        {
            System.String[] str = list.ToArray();//List<string>list转成string[]list
            List<string> aa = new List<string>();
            if (type == "Ftp1")
            {
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        string[] num = list[i].Split('=');
                        string[] bb = num[1].Split(',');
                        if (num[1] == "0,0")
                        {
                            //删除IfFtpUpLoad节点下该键
                            INIFile.DelKey("IfFtpUpLoad", num[0].ToString());
                        }
                        if (bb[0] == "1")
                        {
                            aa.Add(num[0].ToString());
                        }
                    }
                    catch
                    {
                        Tools.WriteLog("error", "ini中backuptime节点存在不合格式的项");//ini中存在不合格式的项
                    }
                }
            }
            if (type == "Ftp2")
            {
                for (int i = 0; i < list.Count; i++)
                {
                    try
                    {
                        string[] num = list[i].Split('=');
                        string[] bb = num[1].Split(',');
                        if (num[1] == "0,0")
                        {
                            //删除IfFtpUpLoad节点下该键
                            INIFile.DelKey("IfFtpUpLoad", num[0].ToString());
                        }
                        if (bb[1] == "1")
                        {
                            aa.Add(num[0].ToString());
                        }
                    }
                    catch
                    {
                        Tools.WriteLog("error", "ini中backuptime节点存在不合格式的项");//ini中存在不合格式的项
                    }
                }
            }
            return aa;
        }
        /// <summary>
        /// 删除N天前的本地备份文件
        /// </summary>
        private void DelTolastDay()
        {
            string day = SysVisitor.Current.of_GetMySysSet("localbackup", "Day");
            int li_day = 0;
            try
            {
                li_day = int.Parse(day);
            }
            catch { return; }
            string path = SysVisitor.Current.of_GetMySysSet("localbackup", "LocalPath");
            DirectoryInfo di = new DirectoryInfo(path);
            DirectoryInfo[] dir = di.GetDirectories("20");//获取子文件夹列表
            foreach (DirectoryInfo d in dir)
            {
                string name = d.Name;
                if (name.Length > 8)
                {
                    string timeStr = name.Substring(0, 8);
                    DateTime time = Convert.ToDateTime(timeStr);
                    DateTime timeNow = DateTime.Now;
                    TimeSpan diff = timeNow - time;
                    if (diff.Days >= li_day)
                    {
                        DirectoryInfo diinfo = new DirectoryInfo(path + @"\" + name);
                        diinfo.Delete(true);//删除目录及删除子目录和文件
                    }
                }
            }
        }

        internal void DoStart()
        {
            OnStart(null);
        }

        internal void DoStop()
        {
            OnStop();
        }
    }
}
