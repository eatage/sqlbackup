using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Collections;
using System.Configuration.Install;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace sqlbackup
{
    /// <summary>
    /// 安装卸载服务
    /// </summary>
    class WinService
    {
        /// <summary>
        /// 判断服务是否存在
        /// </summary>
        /// <param name="serviceName">Windows 服务名，字符串</param>
        /// <returns>如果存在Windows服务返回真</returns>
        public static bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="filePath">服务文件路径</param>
        /// <param name="serviceName">服务名称</param>
        /// <param name="options">选项</param>
        public static void InstallService(string filePath, string serviceName, string[] options)
        {
            try
            {
                if (!IsServiceExisted(serviceName))
                {
                    IDictionary mySavedState = new Hashtable();
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filePath;
                    myAssemblyInstaller.CommandLine = options;
                    myAssemblyInstaller.Install(mySavedState);
                    myAssemblyInstaller.Commit(mySavedState);
                    myAssemblyInstaller.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("请以管理员身份启动程序再执行此操作\n" + ex.Message);
            }
        }
        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="filepath"></param>
        public static void uninstallmyservice(string filepath)
        {
            try
            {
                AssemblyInstaller assemblyinstaller1 = new AssemblyInstaller();
                assemblyinstaller1.UseNewContext = true;
                assemblyinstaller1.Path = filepath;
                assemblyinstaller1.Uninstall(null);
                assemblyinstaller1.Dispose();
            }
            catch(Exception e)
            {
                throw new Exception("请以管理员身份启动程序再执行此操作\n" + e.Message);
            }
        }

        /// <summary>
        /// 安装服务
        /// </summary>
        /// <param name="servicePath">服务的exe程序路径</param>
        /// <param name="InstallUtilPath">InstallUtil.exe路径</param>
        private void InstallService(string servicePath, string InstallUtilPath)
        {
            Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            string sysDisk = System.Environment.SystemDirectory.Substring(0, 3);
            string dotNetPath = sysDisk + @"WINDOWS\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";//4.0的环境路径
            string serviceEXEPath = Application.StartupPath + @"\Servicesqlbackup.exe";//服务的exe程序路径
            string serviceInstallCommand = string.Format(@"{0}  {1}", dotNetPath, serviceEXEPath);//安装服务时使用的dos命令
            string serviceUninstallCommand = string.Format(@"{0} -U {1}", dotNetPath, serviceEXEPath);//卸载服务时使用的dos命令
            try
            {
                if (File.Exists(dotNetPath))
                {
                    string[] cmd = new string[] { serviceUninstallCommand };
                    string ss = Cmd(cmd);
                    CloseProcess("cmd.exe");
                }
            }
            catch
            {
            }
            Thread.Sleep(1000);
            try
            {
                if (File.Exists(dotNetPath))
                {
                    string[] cmd = new string[] { serviceInstallCommand };
                    string ss = Cmd(cmd);
                    CloseProcess("cmd.exe");
                }
            }
            catch
            {
            }
            try
            {
                Thread.Sleep(3000);
                ServiceController sc = new ServiceController("Servicesqlbackup");
                if (sc != null && (sc.Status.Equals(ServiceControllerStatus.Stopped)) ||
                          (sc.Status.Equals(ServiceControllerStatus.StopPending)))
                {
                    sc.Start();
                }
                sc.Refresh();
            }
            catch
            {
            }
        }
        /// <summary>  
        /// 运行CMD命令  
        /// </summary>  
        /// <param name="cmd">命令</param>  
        /// <returns></returns>  
        public static string Cmd(string[] cmd)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.StandardInput.AutoFlush = true;
            for (int i = 0; i < cmd.Length; i++)
            {
                p.StandardInput.WriteLine(cmd[i].ToString());
            }
            p.StandardInput.WriteLine("exit");
            string strRst = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();
            return strRst;
        }
        /// <summary>  
        /// 关闭进程  
        /// </summary>  
        /// <param name="ProcName">进程名称</param>  
        /// <returns></returns>  
        public static bool CloseProcess(string ProcName)
        {
            bool result = false;
            System.Collections.ArrayList procList = new System.Collections.ArrayList();
            string tempName = "";
            int begpos;
            int endpos;
            foreach (System.Diagnostics.Process thisProc in System.Diagnostics.Process.GetProcesses())
            {
                tempName = thisProc.ToString();
                begpos = tempName.IndexOf("(") + 1;
                endpos = tempName.IndexOf(")");
                tempName = tempName.Substring(begpos, endpos - begpos);
                procList.Add(tempName);
                if (tempName == ProcName)
                {
                    if (!thisProc.CloseMainWindow())
                        thisProc.Kill(); // 当发送关闭窗口命令无效时强行结束进程  
                    result = true;
                }
            }
            return result;
        }  
    }
}
