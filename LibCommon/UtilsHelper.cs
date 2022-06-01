using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Serialization;
using LibCommon.Structs;
using Newtonsoft.Json;

namespace LibCommon
{
    /// <summary>
    /// 常用工具类
    /// </summary>
    public static class UtilsHelper
    {

        /// <summary>
        /// 为字符串添加引号
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string AddQuote(string str)
        {
            return $"\"{str.Trim()}\"";
        }
        /// <summary>
        /// 获取启动时传入参数列表
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> GetMainParams(string[] args)
        {
            List<KeyValuePair<string, string>> tmpReturn = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Trim().Length == 2 && args[i].Trim().StartsWith('-'))
                {
                    if (!string.IsNullOrEmpty(args[i + 1].Trim()))
                    {
                        tmpReturn.Add(new KeyValuePair<string,string>(args[i].Trim(),args[i+1].Trim()));
                    }
                }
            }
            return tmpReturn;
        }
        
        /// <summary>
        /// 按指定数量对List分组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="groupNum"></param>
        /// <returns></returns>
        public static List<List<T>> GetListGroup<T>(List<T> list, int groupNum)
        {
            List<List<T>> listGroup = new List<List<T>>();
            for (int i = 0; i < list.Count(); i += groupNum)
            {
                listGroup.Add(list.Skip(i).Take(groupNum).ToList());
            }

            return listGroup;
        }
        
        /// <summary>
        /// 日期转long 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ConvertDateTimeToLong(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dt.Subtract(dtStart);
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
            return timeStamp;
        }


        /// <summary>
        /// 检查是否为网络异常的http报错
        /// </summary>
        /// <param name="httpResponse"></param>
        /// <returns></returns>
        public static bool HttpClientResponseIsNetWorkError(string httpResponse)
        {
            string tmp = httpResponse.Trim().ToLower();
            if (tmp.Equals("network is unreachable network is unreachable")
                || tmp.Equals("connection refused connection refused")
                || tmp.Equals("the operation has timed out.")
                || tmp.Equals("an error occurred while sending the request. the response ended prematurely.")
                || tmp.Equals(
                    "an error occurred while sending the request. unable to read data from the transport connection: connection reset by peer.")
                || tmp.Contains("operation timed out")
                || tmp.Contains("network is down")
                || tmp.Contains("no route")
                || tmp.Contains("connection refused")
            )
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断stirng是空的扩展方法
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StringIsNullEx(string str)
        {
            if (string.IsNullOrEmpty(str) || str.ToLower().Trim().Equals("string"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 非gb28181设备的streamid计算
        /// </summary>
        /// <param name="videoSrcUrl"></param>
        /// <returns></returns>
        public static string GetDisGB28181VideoChannelMainId(string videoSrcUrl)
        {
            var crc32 = CRC32Helper.GetCRC32(videoSrcUrl.ToUpper().Trim());
            return string.Format("{0:X8}", crc32);
        }

        /// <summary>
        /// 获取gb28181设备实时流的ssrc信息
        /// </summary>
        /// <param name="deivceId"></param>
        /// <param name="channelId"></param>
        /// <returns>第一返回是用于sip推流的ssrc,第二个返回是用于zlm的streamid</returns>
        public static KeyValuePair<string, string> GetSSRCInfo(string deivceId, string channelId)
        {
            var tag = "0" + channelId.Substring(3, 5) +
                      deivceId + channelId;
            var crc32 = CRC32Helper.GetCRC32(tag);
            var crc32Str = crc32.ToString().PadLeft(10, '0');
            char[] tmpChars = crc32Str.ToCharArray();
            tmpChars[0] = '0'; //实时流ssrc第一位是0
            string ssrcid = new string(tmpChars);
            return new KeyValuePair<string, string>(ssrcid, string.Format("{0:X8}", uint.Parse(ssrcid)));
        }

        /// <summary>
        /// 检查ffmpeg是否存在，是否正常
        /// </summary>
        /// <param name="ffpath"></param>
        /// <returns></returns>
        public static bool CheckFFmpegBin(string ffpath = "")
        {
            if (string.IsNullOrEmpty(ffpath))
            {
                ffpath = "ffmpeg";
            }

            ProcessHelper tmpProcessHelper = new ProcessHelper(null, null, null);
            try
            {
                tmpProcessHelper.RunProcess(ffpath, "", 5000, out string std, out string err);

                if (!string.IsNullOrEmpty(std))
                {
                    if (std.ToLower().Contains("ffmpeg version"))
                    {
                        return true;
                    }
                }

                if (!string.IsNullOrEmpty(err))
                {
                    if (err.ToLower().Contains("ffmpeg version"))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否是正常可用的端口
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsPortOK(string number)
        {
            try
            {
                var ret = ushort.TryParse(number, out ushort port);
                if (ret)
                {
                    return !PortInUse(port);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检测端口是否被占用
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        private static bool PortInUse(int port)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            List<IPEndPoint> ipEndPoints = ipProperties.GetActiveTcpListeners().ToList();
            if (ipEndPoints.Count > 0)
            {
                var ret = ipEndPoints.FindLast(x => x.Port == port);
                return ret == null ? false : true;
            }

            return false;
        }


        /// <summary>
        /// DateTime转时间戳
        /// </summary>
        /// <param name="time">DateTime时间</param>
        /// <param name="type">0为毫秒,1为秒</param>
        /// <returns></returns>
        public static string ConvertTimestamp(DateTime time, int type = 0)
        {
            double intResult = 0;
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            if (type == 0)
            {
                intResult = (time - startTime).TotalMilliseconds;
            }
            else if (type == 1)
            {
                intResult = (time - startTime).TotalSeconds;
            }
            else
            {
                Console.WriteLine("参数错误!");
            }

            return Math.Round(intResult, 0).ToString();
        }

        /// <summary>
        /// 判断是否为奇数
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static bool IsOdd(int num)
        {
            return (num & 1) == 1;
        }

        /// <summary>
        /// 获取文件的MD5码
        /// </summary>
        /// <param name="fileName">传入的文件名（含路径及后缀名）</param>
        /// <returns></returns>
        public static string Md5WithFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }

        /// <summary>
        /// 获取MD5加密码值
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(string str)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] bytValue, bytHash;
                bytValue = Encoding.UTF8.GetBytes(str);
                bytHash = md5.ComputeHash(bytValue);
                md5.Clear();
                string sTemp = "";
                for (int i = 0; i < bytHash.Length; i++)
                {
                    sTemp += bytHash[i].ToString("X").PadLeft(2, '0');
                }

                str = sTemp.ToLower();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return str;
        }

        /// <summary>
        /// XML转类实例
        /// </summary>
        /// <param name="xmlBody"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T XMLToObject<T>(XElement xmlBody)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));

            return (T) xmlSerializer.Deserialize(xmlBody.CreateReader());
        }

        /// <summary>
        /// 生成一个新的序列id
        /// </summary>
        /// <returns></returns>
        public static int CreateNewCSeq()
        {
            var r = new Random();
            return r.Next(1, ushort.MaxValue);
        }

        
        
        /// <summary>
        /// 通过mac地址获取ip地址
        /// </summary>
        /// <param name="mac"></param>
        /// <param name="getIPV6"></param>
        /// <returns></returns>
        public static IPInfo GetIpAddressByMacAddress(string mac, bool getIPV6 = false)
        {
            bool found = false;
            string tmpMac = mac.Replace("-", "").Replace(":", "").ToUpper().Trim();
            IPInfo ipInfo = new IPInfo();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                if (found) break;
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                    adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    string macadd = adapter.GetPhysicalAddress().ToString();
                    if (macadd.ToUpper().Trim().Equals(tmpMac))
                    {
                        //获取以太网卡网络接口信息
                        IPInterfaceProperties ip = adapter.GetIPProperties();
                        //获取单播地址集
                        UnicastIPAddressInformationCollection ipCollection = ip.UnicastAddresses;
                        foreach (UnicastIPAddressInformation ipadd in ipCollection)
                        {
                            if (ipadd.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                //判断是否为ipv4
                                ipInfo.IpV4 = ipadd.Address.ToString().Trim();
                            }

                            if (getIPV6)
                            {
                                if (ipadd.Address.AddressFamily == AddressFamily.InterNetworkV6)
                                {
                                    //判断是否为ipv6
                                    ipInfo.IpV6 = ipadd.Address.ToString().Trim();
                                }
                            }

                            if (getIPV6)
                            {
                                if (!string.IsNullOrEmpty(ipInfo.IpV4) && !string.IsNullOrEmpty(ipInfo.IpV6))
                                {
                                    found = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(ipInfo.IpV4))
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (found)
            {
                return ipInfo;
            }

            return null!;
        }

        /// <summary>
        /// 写入Json配置文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool WriteJsonConfig<T>(string filePath, T obj)
        {
            try
            {
                var jsonStr = JsonHelper.ToJson(obj!, Formatting.Indented);
                File.WriteAllText(filePath, jsonStr);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 读取json配置文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object ReadJsonConfig<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string tmpStr = File.ReadAllText(filePath);
                    return JsonHelper.FromJson<T>(tmpStr)!;
                }
                catch
                {
                    return null!;
                }
            }

            return null!;
        }


        /// <summary>
        /// 是否为Url
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsUrl(string str)
        {
            try
            {
                string Url = @"^http(s)?://([\w-]+\.)+[\w-]+(:\d*)?(/[\w- ./?%&=]*)?$";
                return Regex.IsMatch(str, Url, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否rtmp地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsRtmpUrl(string str)
        {
            try
            {
                string Url = @"^rtmp(s)?://([\w-]+\.)+[\w-]+(:\d*)?(/[\w- ./?%&=]*)?$";
                return Regex.IsMatch(str, Url, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否rtsp地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsRtspUrl(string str)
        {
            try
            {
                string Url = "^((rtsp|rtsps)?://)"
                             + "?(([0-9a-zA-Z_!~*'().&=+$%-]+:)?[0-9a-zA-Z_!~*'().&=+$%-]+@)?" //rtsp的user@
                             + "(([0-9]{1,3}.){3}[0-9]{1,3}" // IP形式的URL- 199.194.52.184
                             + "|" // 允许IP和DOMAIN（域名）
                             + "([0-9a-zA-Z_!~*'()-]+.)*" // 域名- www.
                             + "([0-9a-zA-Z][0-9a-z-]{0,61})?[0-9a-z]." // 二级域名
                             + "[a-zA-Z]{2,6})" // first level domain- .com or .museum
                             + "(:[0-9]{1,4})?" // 端口- :80
                             + "((/?)|" // a slash isn't required if there is no file name
                             + "(/[0-9a-zA-Z_!~*'().;?:@&=+$,%#-]+)+/?)"
                             + "[A-Za-z0-9_!~*'().;?:@&=+$,%#-]{4,40}$";
                return Regex.IsMatch(str, Url, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>  
        /// 将 DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(DateTime time)
        {
            DateTime time2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            long timeStamp = (time.Ticks - time2.Ticks) / 10000; //除10000调整为13位     
            return timeStamp;
        }

        /// <summary>  
        /// 将Unix时间戳格式 转换为DateTime时间格式
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static DateTime ConvertDateTimeToInt(long time)
        {
            DateTime time2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            DateTime dateTime = time2.AddSeconds(time);
            return dateTime;
        }

        /// <summary>
        /// 正则获取内容
        /// </summary>
        /// <param name="str"></param>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetValue(string str, string s, string e)
        {
            Regex rg = new Regex("(?<=(" + s + "))[.\\s\\S]*?(?=(" + e + "))",
                RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(str).Value;
        }


        /// <summary>
        /// 获取两个时间差的毫秒数
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public static long GetTimeGoneMilliseconds(DateTime starttime, DateTime endtime)
        {
            TimeSpan ts = endtime.Subtract(starttime);
            return (long) ts.TotalMilliseconds;
        }

        /// <summary>
        /// 获取时间戳(毫秒级)
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStampMilliseconds()
        {
            return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// 检测是否为ip 地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIpAddr(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }


        /// <summary>
        /// 生成guid
        /// </summary>
        /// <returns></returns>
        public static string? CreateGUID()
        {
            return Guid.NewGuid().ToString("D");
        }

        /// <summary>
        /// 是否为GUID
        /// </summary>
        /// <param name="strSrc"></param>
        /// <returns></returns>
        public static bool IsUUID(string strSrc)
        {
            if (String.IsNullOrEmpty(strSrc))
            {
                return false;
            }

            bool _result = false;
            try
            {
                Guid _t = new Guid(strSrc);
                _result = true;
            }
            catch
            {
            }

            return _result;
        }


        /// <summary>
        /// 结束自己
        /// </summary>
        public static void KillSelf()
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// 获取pid
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static int GetProcessPid(string fileName)
        {
            Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(fileName));
            if (processes.Length > 0)
            {
                if (!processes[0].HasExited)
                {
                    return processes[0].Id;
                }
            }

            return -1;
        }


        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        public static string generalGuid()
        {
            Random rand = new Random((int) DateTime.Now.Ticks);
            string random_str = "";
            for (int i = 0; i < 6; ++i)
            {
                for (int j = 0; j < 8; j++)
                    switch (rand.Next() % 2)
                    {
                        case 1:
                            random_str += (char) ('A' + rand.Next() % 26);
                            break;
                        default:
                            random_str += (char) ('0' + rand.Next() % 10);
                            break;
                    }

                if (i < 5)
                    random_str += "-";
            }

            return random_str;
        }

        /// <summary>
        /// 替换#开头的所有行为;开头
        /// </summary>
        /// <param name="filePath"></param>
        public static void ReplaceSharpWord(string filePath)
        {
            if (File.Exists(filePath))
            {
                var list = File.ReadAllLines(filePath).ToList();
                var tmp_list = new List<string>();
                foreach (var str in list)
                {
                    if (!str.StartsWith('#'))
                    {
                        tmp_list.Add(str);
                    }
                    else
                    {
                        int index = str.IndexOf("#", StringComparison.Ordinal);
                        tmp_list.Add(str.Remove(index, index).Insert(index, ";"));
                    }
                }

                File.WriteAllLines(filePath, tmp_list);
            }
        }


        /// <summary>
        /// 删除List<T>中null的记录
        /// </summary>
        /// <param name="list"></param>
        /// <typeparam name="T"></typeparam>
        public static void RemoveNull<T>(List<T> list)
        {
            // 找出第一个空元素 O(n)
            int count = list.Count;
            for (int i = 0; i < count; i++)
                if (list[i] == null)
                {
                    // 记录当前位置
                    int newCount = i++;

                    // 对每个非空元素，复制至当前位置 O(n)
                    for (; i < count; i++)
                        if (list[i] != null)
                            list[newCount++] = list[i];

                    // 移除多余的元素 O(n)
                    list.RemoveRange(newCount, count - newCount);
                    break;
                }
        }
    }
}