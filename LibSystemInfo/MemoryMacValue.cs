using System;
using System.Diagnostics;
using LibCommon;
using LibCommon.Structs;

namespace LibSystemInfo
{
    public static class MemoryMacValue
    {
        /// <summary>
        /// 获取mac系统总物理内存使用情况(总量/已使用/未使用)(返回单位KB)
        /// </summary>
        /// <returns></returns>
        public static MemoryInfo GetMemoryStatus()
        {
            MemoryInfo _memoryInfo = new MemoryInfo();
            try
            {
                string output = "";
                var info = new ProcessStartInfo();
                info.FileName = "/bin/bash";
                info.Arguments = "-c \"/usr/bin/top -n0 -l 1| awk '/PhysMem/'\"";
                info.RedirectStandardOutput = true;
                using (var process = Process.Start(info))
                {
                    output = process.StandardOutput.ReadToEnd();
                }

                //模板:PhysMem: 8143M used (2080M wired), 49M unused.
                var line = output.Trim();
                if (!string.IsNullOrWhiteSpace(line) && line.ToLower().Contains("physmem") &&
                    line.ToLower().Contains("wired") &&
                    line.ToLower().Contains("unused") && line.EndsWith('.'))
                {
                    var lines = line.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (lines.Length == 2)
                    {
                        string l1;
                        string l2;
                        string l3;
                        //取出wired值
                        var tmpStr1 = UtilsHelper.GetValue(lines[0], "\\(", "\\)");
                        var posLetter = tmpStr1.IndexOfAny(new char[] { 'M', 'K', 'B', 'G' });
                        if (posLetter > 0)
                        {
                            l1 = tmpStr1.Substring(posLetter, 1);
                            tmpStr1 = tmpStr1.Remove(posLetter);
                            //取出unused值
                            lines[1] = lines[1].Replace("unused.", "").Trim();
                            posLetter = lines[1].IndexOfAny(new char[] { 'M', 'K', 'B', 'G' });
                            if (posLetter > 0)
                            {
                                l2 = lines[1].Substring(posLetter, 1);
                                var tmpStr2 = lines[1].Remove(posLetter);
                                //取出used值  
                                lines[0] = lines[0].Remove(0, lines[0].IndexOf(':') + 1).Trim();
                                posLetter = lines[0].IndexOfAny(new char[] { 'M', 'K', 'B', 'G' });
                                l3 = lines[0].Substring(posLetter, 1);
                                var tmpStr3 = lines[0].Substring(0, posLetter);

                                double tmpint1 = 0;
                                double tmpint2 = 0;
                                double tmpint3 = 0;

                                switch (l3.ToLower())
                                {
                                    case "b":

                                        if (!double.TryParse(tmpStr3, out tmpint3))
                                        {
                                            return null;
                                        }

                                        tmpint3 = tmpint3 / 1024;

                                        break;
                                    case "k":
                                        if (!double.TryParse(tmpStr3, out tmpint3))
                                        {
                                            return null;
                                        }

                                        break;
                                    case "m":
                                        if (!double.TryParse(tmpStr3, out tmpint3))
                                        {
                                            return null;
                                        }

                                        tmpint3 = tmpint3 * 1024;
                                        break;
                                    case "g":
                                        if (!double.TryParse(tmpStr3, out tmpint3))
                                        {
                                            return null;
                                        }

                                        tmpint3 = tmpint3 * 1024 * 1024;
                                        break;
                                }

                                switch (l2.ToLower())
                                {
                                    case "b":

                                        if (!double.TryParse(tmpStr2, out tmpint2))
                                        {
                                            return null;
                                        }

                                        tmpint2 = tmpint2 / 1024;

                                        break;
                                    case "k":
                                        if (!double.TryParse(tmpStr2, out tmpint2))
                                        {
                                            return null;
                                        }

                                        break;
                                    case "m":
                                        if (!double.TryParse(tmpStr2, out tmpint2))
                                        {
                                            return null;
                                        }

                                        tmpint2 = tmpint2 * 1024;
                                        break;
                                    case "g":
                                        if (!double.TryParse(tmpStr2, out tmpint2))
                                        {
                                            return null;
                                        }

                                        tmpint2 = tmpint2 * 1024 * 1024;
                                        break;
                                }

                                switch (l1.ToLower())
                                {
                                    case "b":

                                        if (!double.TryParse(tmpStr1, out tmpint1))
                                        {
                                            return null;
                                        }

                                        tmpint1 = tmpint1 / 1024;

                                        break;
                                    case "k":
                                        if (!double.TryParse(tmpStr1, out tmpint1))
                                        {
                                            return null;
                                        }

                                        break;
                                    case "m":
                                        if (!double.TryParse(tmpStr1, out tmpint1))
                                        {
                                            return null;
                                        }

                                        tmpint1 = tmpint1 * 1024;
                                        break;
                                    case "g":
                                        if (!double.TryParse(tmpStr1, out tmpint1))
                                        {
                                            return null;
                                        }

                                        tmpint1 = tmpint1 * 1024 * 1024;
                                        break;
                                }


                                _memoryInfo.Total = (ulong)(tmpint3 + tmpint2);
                                _memoryInfo.Used = (ulong)(tmpint3 - tmpint2 - tmpint1);
                                _memoryInfo.Free = (ulong)(tmpint2 + tmpint1);
                                _memoryInfo.FreePercent =
                                    Math.Round(
                                        double.Parse(_memoryInfo.Free.ToString()) * 100.00 /
                                        double.Parse(_memoryInfo.Total.ToString()),
                                        3);
                                _memoryInfo.UpdateTime = DateTime.Now;
                            }
                        }
                    }
                }


                return _memoryInfo;
            }
            catch
            {
                return _memoryInfo;
            }
        }
    }
}