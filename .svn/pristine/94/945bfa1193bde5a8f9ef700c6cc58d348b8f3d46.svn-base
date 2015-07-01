using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using MobileGate;

namespace sqlbackup
{
    /// <summary>
    /// 
    /// </summary>
    public class Tools {
        /// <summary>
        /// 发送队列集合
        /// </summary>
        public static Dictionary<string, System.Collections.Queue> connectedSockets = new Dictionary<string, System.Collections.Queue>();
        /// <summary>
        /// 线程状态集合 1初始化 2成功 3失败
        /// </summary>
        public static Dictionary<string, string> socketStart = new Dictionary<string, string>();
        /// <summary>
        /// 滑动窗体，每个滑动窗体大小为16条
        /// </summary>
        //public static Dictionary<string, string> Slidingwindow = new Dictionary<string, string>();
        /// <summary>
        /// 长短信集合
        /// </summary>
        public static Dictionary<string, string[][]> longTxtList = new Dictionary<string, string[][]>();


        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="ws">日志信息</param>
        public static void WriteLog( string ws) {
            LogHelper.Log("日志", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "----" + ws);

        }

        public static void WriteLog(string as_type,string ws)
        {
            LogHelper.Log(as_type, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n       " + ws);
        }

        public static Int32 longcontent = 10;
        /// <summary>
        /// 长短信编号
        /// </summary>
        public static Int32 LongContxt {
            get {

                if (longcontent > 999)//999999999
                {
                    longcontent = 10;
                    return longcontent;
                } else {
                    longcontent += 1;
                    return longcontent;
                }
            }

        }

        /// <summary>
        /// 初始值
        /// </summary>
        public static uint seqid = 11111;
        /// <summary>
        /// 消息流水号
        /// </summary>
        public static uint SeqID {
            get {

                if (seqid >= 999999999)//999999999
                {
                    seqid = 11111;
                    return seqid;
                } else {
                    seqid += 1;
                    return seqid;
                }
            }

        }
        /// <summary>
        /// 初始值
        /// </summary>
        public static uint seqid2 = 200000000;
        /// <summary>
        /// 消息流水号
        /// </summary>
        public static uint SeqID2 {
            get {

                if (seqid2 >= 399999999)//999999999
                {
                    seqid2 = 200000000;
                    return seqid2;
                } else {
                    seqid2 += 1;
                    return seqid2;
                }
            }

        }
        /// <summary>
        /// 初始值
        /// </summary>
        public static uint seqid3 = 400000000;
        /// <summary>
        /// 消息流水号
        /// </summary>
        public static uint SeqID3 {
            get {

                if (seqid3 >= 599999999)//999999999
                {
                    seqid3 = 400000000;
                    return seqid3;
                } else {
                    seqid3 += 1;
                    return seqid3;
                }
            }

        }

        /// <summary>
        /// 初始值
        /// </summary>
        public static uint seqid4 = 600000000;
        /// <summary>
        /// 消息流水号
        /// </summary>
        public static uint SeqID4 {
            get {

                if (seqid4 >= 799999999)//999999999
                {
                    seqid4 = 600000000;
                    return seqid4;
                } else {
                    seqid4 += 1;
                    return seqid4;
                }
            }

        }

        /// <summary>
        /// 初始值
        /// </summary>
        public static uint seqid5 = 800000000;
        /// <summary>
        /// 消息流水号
        /// </summary>
        public static uint SeqID5 {
            get {

                if (seqid5 >= 999999999)//999999999
                {
                    seqid5 = 800000000;
                    return seqid5;
                } else {
                    seqid5 += 1;
                    return seqid5;
                }
            }

        }
        public static UInt64 msgid = 1111;
        /// <summary>
        /// 信息标识
        /// </summary>
        public static UInt64 MsgID {
            get {
                if (msgid >= 9999) {
                    msgid = 1111;
                    return Convert.ToUInt64(DateTime.Now.ToString("yyMMddHHmmssfff") + msgid.ToString());
                } else {
                    msgid += 1;
                    return Convert.ToUInt64(DateTime.Now.ToString("yyMMddHHmmssfff") + msgid.ToString());
                }
            }

        }

        public static Int64 Nid = 1111;
        /// <summary>
        /// 信息标识
        /// </summary>
        public static Int64 NewId {
            get {
                if (Nid >= 9999) {
                    Nid = 1111;
                    return Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmmssfff") + Nid.ToString());
                } else {
                    Nid += 1;
                    return Convert.ToInt64(DateTime.Now.ToString("yyMMddHHmmssfff") + Nid.ToString());
                }
            }

        }

        /// <summary>
        /// 获取程序安装目录
        /// </summary>
        /// <returns></returns>
        public static string getpath() {
            string str = Assembly.GetExecutingAssembly().GetName().CodeBase;
            if (str.ToLower().IndexOf("file:///") >= 0) {
                str = str.Substring(8, str.Length - 8);
            }
            string path = System.IO.Path.GetDirectoryName(str);
            return path;

        }

        /// <summary>
        ///记录系统信息
        /// </summary>
        /// <param name="ValueOne">业务名称</param>
        /// <param name="errMsg">字符串值</param>
        private static void ReadLog(string ValueOne, string Msg) {
            try {
                string fileName;
                fileName = getpath() + "\\log\\" + ValueOne + DateTime.Now.Date.ToShortDateString() + ".txt";
                System.IO.StreamWriter sw;

                if (!System.IO.File.Exists(fileName))//检测文件是否存在
                {
                    System.IO.FileStream fs = System.IO.File.Create(fileName);//不存在则创建它
                    fs.Close();

                }

                sw = new System.IO.StreamWriter(fileName, true, System.Text.Encoding.Unicode);
                try {
                    //定义写入日志的格式
                    sw.WriteLine(Msg);
                    sw.Flush();
                    sw.Close();
                } catch {
                    sw.Close();
                }
            } catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);//写入跟踪侦听器中
            }
        }

        /// <summary>
        /// 按指定的编码，把相应字符串转换为指定的字节数组
        /// </summary>
        /// <param name="con"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static byte[] StringToByte(string con, string encode) {
            //0：纯ASCII字符串3：WRITECARD写卡操作4：BINARY二进制编码8：UCS2编码15: GB2312编码
            if (encode == "ASCII")
                return Encoding.ASCII.GetBytes(con);
            if (encode == "DEFAULT")
                return Encoding.Default.GetBytes(con);
            if (encode == "WRITECARD")
                return Encoding.ASCII.GetBytes(con);
            if (encode == "BINARY")
                return Encoding.ASCII.GetBytes(con);
            if (encode == "UCS2")
                return Encoding.BigEndianUnicode.GetBytes(con);
            if (encode == "GB2312")
                return Encoding.Default.GetBytes(con);
            return Encoding.ASCII.GetBytes("");

        }

        /// <summary>
        /// 按指定编码，把相应的字节数组转换为字符串
        /// </summary>
        /// <param name="con"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string ByteToString(byte[] con, string encode) {
            //0：纯ASCII字符串3：WRITECARD写卡操作4：BINARY二进制编码8：UCS2编码15: GB2312编码
            if (encode == "ASCII")
                return Encoding.ASCII.GetString(con).Replace("\0", "");
            if (encode == "DEFAULT")
                return Encoding.Default.GetString(con).Replace("\0", "");
            if (encode == "WRITECARD")
                return Encoding.ASCII.GetString(con).Replace("\0", "");
            if (encode == "BINARY")
                return Encoding.ASCII.GetString(con).Replace("\0", "");
            if (encode == "UCS2")
                return Encoding.BigEndianUnicode.GetString(con).Replace("\0", "");
            if (encode == "GB2312")
                return Encoding.Default.GetString(con).Replace("\0", "");
            return "";
        }

        /// <summary>
        /// 数组MD5加密 Hash
        /// </summary>
        /// <param name="ib"></param>
        /// <returns></returns>
        public static byte[] MD5(byte[] ib) {
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            return md5.ComputeHash(ib, 0, ib.Length);
        }

        /// <summary>
        /// 数字转为字节数组
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] UintToUintArray(uint num) {
            return BitConverter.GetBytes(num);
        }

        /// <summary>
        /// 无符号整形高低位互换(4字节)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] IntChangeNetByte(byte[] num) {
            byte[] tnum = num;
            byte t;
            t = tnum[0];
            tnum[0] = tnum[3];
            tnum[3] = t;
            t = tnum[1];
            tnum[1] = tnum[2];
            tnum[2] = t;
            return tnum;
        }

        /// <summary>
        /// 无符号整形高低位互换(8字节)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] Int64ChangeNetByte(byte[] num) {
            byte[] tnum = new byte[8];
            for (int i = 0; i < 8; i++) {
                tnum[i] = num[i];
            }
            for (int i = 0; i < tnum.Length / 2; i++) {
                byte t = tnum[i];
                tnum[i] = tnum[tnum.Length - 1 - i];
                tnum[tnum.Length - 1 - i] = t;
            }
            return tnum;
        }

        /// <summary>
        /// 从字节数组中取其中一部分生成新的字节数组
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="bi"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] SubByte(byte[] sb, int bi, int length) {
            byte[] rb = new byte[length];
            Array.Copy(sb, bi, rb, 0, length);
            return rb;
        }

        /// <summary>
        /// 字节数组转为数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static uint UintArrayToUint(byte[] num) {
            return BitConverter.ToUInt32(num, 0);
        }

        /// <summary>
        /// 数字转为字节数组(8字节)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static byte[] Uint64ToUintArray(UInt64 num) {
            return BitConverter.GetBytes(num);
        }

        /// <summary>
        /// 字节数组转为数字(8字节)
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static UInt64 UintArrayToUint64(byte[] num) {
            return BitConverter.ToUInt64(num, 0);
        }

        /// <summary>
        /// 拆分 ,
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string[] SplitListString(string content) {
            char[] SplitString1 = new char[] { ',' };
            string[] _ShortList = content.Split(SplitString1, StringSplitOptions.RemoveEmptyEntries);
            return _ShortList;
        }
    }
}
