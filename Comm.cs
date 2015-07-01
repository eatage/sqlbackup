using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data;
using System.Net;
 
    class comm
    {
         [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int conn, int val);

         public static string SendMessage(string Url, string strMessage)
         {
             string strResponse;
             // 初始化WebClient 
             WebClient webClient = new WebClient();
             webClient.Headers.Add("Accept", "*/*");
             webClient.Headers.Add("Accept-Language", "zh-cn");
             webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

             try
             {
                 byte[] responseData = webClient.UploadData(Url, "POST", Encoding.GetEncoding("UTF-8").GetBytes(strMessage));
                 string srcString = Encoding.GetEncoding("UTF-8").GetString(responseData);
                 strResponse = srcString;
             }
             catch
             {
                 return "-1";
             }
             return strResponse;
         }

           //保存日志
        public static int of_SaveLog(string as_text)
        {
            string ls_fileName;
            ls_fileName = DateTime.Now.ToString("yyyy-MM-dd");

            string ls_path;
            ls_path = Application.StartupPath + "\\Log\\";

            ls_fileName = ls_path + ls_fileName + ".txt";
            as_text=DateTime.Now.ToString("hh:mm:ss")+"<-->"+as_text+"\r\n";

            try
            {
                CreateDirtory(ls_path);
                File.AppendAllText(ls_fileName, as_text);

            }
            catch
            {

            }
            return 1;
        }


        //是否已经运行了某个Exe文件
        public static  bool  gf_IsHaveRun(string as_exeName)
        {
            as_exeName = as_exeName.ToLower();

            Process[] allProcess = Process.GetProcesses();
            foreach (Process p in allProcess)
            {

                if (p.ProcessName.ToLower() + ".exe" == as_exeName)
                {
                    return true;
                }
            }
            return false;
        }


        public static int gf_KillExe(string as_exeName)
        {
            as_exeName = as_exeName.ToLower();

            Process[] allProcess = Process.GetProcesses();
            foreach (Process p in allProcess)
            {

                if (p.ProcessName.ToLower() + ".exe" == as_exeName)
                {
                    for (int i = 0; i < p.Threads.Count; i++)
                        p.Threads[i].Dispose();


                    return 1;  //成功Kill
                }
            }

            //没有找到这个Exe
            return 0;
        }


        //运行Exe文件
        public static int gf_Exe(string as_Exe,string as_arguMents)
        {

            if (!File.Exists(as_Exe))
            {
                return -1;
            }

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = as_Exe;  //需要启动的程序名       
            p.StartInfo.Arguments = as_arguMents;    // sourceFile.Arj     c:\temp";//启动参数       
            p.Start();//启动       

            return 1;

        }


        //复制文件;
        public static int CopyFile(string sourcePath, string objPath)
        {
            //			char[] split = @"\".ToCharArray();
            if (!Directory.Exists(objPath))
            {
                Directory.CreateDirectory(objPath);
            }
            string[] files = Directory.GetFiles(sourcePath);
            for (int i = 0; i < files.Length; i++)
            {
                string[] childfile = files[i].Split('\\');
                File.Copy(files[i], objPath + @"\" + childfile[childfile.Length - 1], true);
            }
            string[] dirs = Directory.GetDirectories(sourcePath);
            for (int i = 0; i < dirs.Length; i++)
            {
                string[] childdir = dirs[i].Split('\\');
                CopyFile(dirs[i], objPath + @"\" + childdir[childdir.Length - 1]);
            }

            return 1;
        }


        //创建目录
        public static int CreateDirtory(string path)
        {
            if (!File.Exists(path))
            {
                string[] dirArray = path.Split('\\');
                string temp = string.Empty;
                for (int i = 0; i < dirArray.Length - 1; i++)
                {
                    temp += dirArray[i].Trim() + "\\";
                    if (!Directory.Exists(temp))
                        Directory.CreateDirectory(temp);
                }
            }

            return 1;

        }


        //取到上一级目录
            public static string  GetLastPath(string as_path)
        {
            if (as_path.Substring(as_path.Length -1)=="\\")
            {
                as_path=as_path.Substring(0,as_path.Length -1);
            }

            int li_pos;
            li_pos=as_path.LastIndexOf("\\");

            as_path=as_path.Substring(0,li_pos);

            return as_path;
        }

        //用K,M ,G 的方式显示文件大小
        public static string of_DispFileSize(long al_Filesize)
        {
            string ls_filesize;
            
            decimal  ldc_size;
            ldc_size = al_Filesize / 1000;
            ls_filesize = ldc_size.ToString("###.#") + "K";

            if (al_Filesize > 1000)
            {
                al_Filesize = al_Filesize / 1000;
                ls_filesize = ldc_size.ToString("###.#") + "M";
            }

            if (al_Filesize > 1000)
            {
                al_Filesize = al_Filesize / 1000;
                ls_filesize = ldc_size.ToString("###.#") + "G";
            }

            return ls_filesize;
    }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="action">标题</param>
        /// <param name="as_file">文件标示</param>
        /// <param name="strMessage">信息内容</param>
        public static void WriteTextLog(string action, string as_file, string strMessage)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string fileFullPath = path + DateTime.Now.ToString("yyyy-MM-dd") + "_" + as_file + ".txt";
            StringBuilder str = new StringBuilder();
            str.Append("时间:    " + DateTime.Now.ToString() + "\r\n");
            str.Append("Action:  " + action + "\r\n");
            str.Append("Message: " + strMessage + "\r\n");
            str.Append("-----------------------------------------------------------" + "\r\n  \r\n ");
            StreamWriter sw = default(StreamWriter);
            if (!File.Exists(fileFullPath))
            {
                sw = File.CreateText(fileFullPath);
            }
            else
            {
                sw = File.AppendText(fileFullPath);
            }
            sw.WriteLine(str.ToString());
            sw.Close();
        }

    //互联网是否已经联结
     public static bool of_WebnetIsConnect()
     {
            int Out;
            if (InternetGetConnectedState(out Out, 0) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
     }


     public static string of_GetLastPath(string as_path)
     {
         if(as_path.Substring(as_path.Length -2,1)=="\\")
         {
             as_path=as_path.Substring(0,as_path.Length -2);
         }
          
         int li_pos;
         li_pos=as_path.LastIndexOf("\\");

         as_path = as_path.Substring(0, li_pos);

         return as_path;
     }

    
        public static string Execute(string dosCommand, int milliseconds)
    {
        string output = "";     //输出字符串
        if (dosCommand != null && dosCommand != "")
        {
            Process process = new Process();     //创建进程对象
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "cmd.exe";      //设定需要执行的命令
            startInfo.Arguments = "/C " + dosCommand;   //设定参数，其中的“/C”表示执行完命令后马上退出
            startInfo.UseShellExecute = false;     //不使用系统外壳程序启动
            startInfo.RedirectStandardInput = false;   //不重定向输入
            startInfo.RedirectStandardOutput = true;   //重定向输出
            startInfo.CreateNoWindow = true;     //不创建窗口
            process.StartInfo = startInfo;
            try
            {
                if (process.Start())       //开始进程
                {
                    if (milliseconds == 0)
                        process.WaitForExit();     //这里无限等待进程结束
                    else
                        process.WaitForExit(milliseconds);  //这里等待进程结束，等待时间为指定的毫秒
                    output = process.StandardOutput.ReadToEnd();//读取进程的输出
                }
            }
            catch
            {
            }
            finally
            {
                if (process != null)
                    process.Close();
            }
        }
        return output;
    }




       


        public static string[] of_GetArrayFromStr(string as_Str,string as_sub)
        {
            int li_pos=0;
            int li_row=0;
            int li_count=1;
            li_pos = as_Str.IndexOf(as_sub, 0);
            while ((li_pos < as_Str.Length)&&(li_pos>0))
            {
                li_pos = as_Str.IndexOf(as_sub, li_pos+1); //+ 1
                li_count++;
            }


            string[] lsa_str = new string[li_count];
            li_pos = as_Str.IndexOf(as_sub, 0);
            int li_begin=0;
            string ls_value;
            while ((li_pos < as_Str.Length) && (li_pos > 0))
            {
                li_pos = as_Str.IndexOf(as_sub, li_begin );
                if (li_pos < 0)
                {
                    break;   //可以跳出去了，字符已经结束了 
                }
                ls_value = as_Str.Substring(li_begin, li_pos - li_begin);
                li_begin = li_pos + as_sub.Length;
                lsa_str[li_row] = ls_value;
                li_row++;
            }

            if (li_pos < 0) li_pos = 0;

            li_pos = as_Str.Length;
            ls_value = as_Str.Substring(li_begin, li_pos - li_begin); //- as_sub.Length
            lsa_str[li_row] = ls_value;

            return lsa_str;
        }
        /// <summary>
        /// 合并字节数组
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static byte[] GetByte(List<byte[]> list)
        {
            byte[] all = new byte[(list.Count - 1) * 140 + list[list.Count - 1].Length];  //总长度
            for (int i = 0; i < list.Count; i++)
            {
                list[i].CopyTo(all, i * 140);
            }
            return all;
        }



        public static string DataTableToJson(DataTable dt)
        {
            StringBuilder Json = new StringBuilder();
            Json.Append("[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":\"" + dt.Rows[i][j].ToString() + "\"");
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]");
            return Json.ToString();
        }
    


}

 