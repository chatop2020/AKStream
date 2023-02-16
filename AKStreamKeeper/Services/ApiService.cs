using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using LibCommon;
using LibCommon.Structs;
using LibCommon.Structs.WebResponse.AKStreamKeeper;
using Newtonsoft.Json;

namespace AKStreamKeeper.Services
{
    public static class ApiService
    {
        /// <summary>
        /// 获取日志级别
        /// </summary>
        /// <param name="level"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static string GetLoggerLevel(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };

            return GCommon.Logger.GetLogLevel();
        }

        /// <summary>
        /// 释放已经使用过的rtp端口
        /// </summary>
        /// <param name="port"></param>
        public static bool ReleaseRtpPort(ushort port)
        {
            lock (Common._getRtpPortLock)
            {
                var portUsed = Common.PortInfoList.FindLast(x => x.Port.Equals(port));
                if (portUsed != null)
                {
                    portUsed.Useed = false;
                    portUsed.DateTime = DateTime.Now; //更新冷却时间，在这个时间后的60秒内，此端口不允许使用
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->释放rtp端口成功:{port}");
                }
            }

            return true;
        }

        /// <summary>
        /// 释放已经使用过的rtp端口(发送)
        /// </summary>
        /// <param name="port"></param>
        public static bool ReleaseRtpPortForSender(ushort port)
        {
            lock (Common._getRtpPortLock)
            {
                var portUsed = Common.PortInfoListForSender.FindLast(x => x.Port.Equals(port));
                if (portUsed != null)
                {
                    portUsed.Useed = false;
                    portUsed.DateTime = DateTime.Now; //更新冷却时间，在这个时间后的60秒内，此端口不允许使用
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->释放rtp(发送)端口成功:{port}");
                }
            }

            return true;
        }

        /// <summary>
        /// 选择一个可用的rtp端口，仅使用偶数端口
        /// </summary>
        /// <param name="minPort"></param>
        /// <param name="maxPort"></param>
        /// <returns></returns>
        private static ushort _guessAnRtpPort(ushort minPort, ushort maxPort)
        {
            try
            {
                lock (Common._getRtpPortLock)
                {
                    IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                    List<IPEndPoint> tcpIpEndPoints = ipProperties.GetActiveTcpListeners().ToList();
                    List<IPEndPoint> udpIpEndPoints = ipProperties.GetActiveUdpListeners().ToList();
                    if (minPort > maxPort)
                    {
                        var tmp = minPort;
                        maxPort = minPort;
                        minPort = tmp;
                    }


                    if (minPort == maxPort)
                    {
                        var tcp = tcpIpEndPoints.FindLast(x => x.Port == minPort);
                        var udp = udpIpEndPoints.FindLast(x => x.Port == minPort);
                        var portUsed = Common.PortInfoList.FindLast(x => x.Port.Equals(minPort));
                        if (tcp == null && udp == null)
                        {
                            if (portUsed == null)
                            {
                                Common.PortInfoList.Add(new PortInfo()
                                {
                                    DateTime = DateTime.Now,
                                    Port = minPort,
                                    Useed = true,
                                });
                                GCommon.Logger.Info($"[{Common.LoggerHead}]->获取可用rtp端口:{minPort}");
                                return minPort;
                            }

                            if (!portUsed.Useed.Equals(true))
                            {
                                if ((DateTime.Now - portUsed.DateTime).TotalSeconds >
                                    Common.AkStreamKeeperConfig.RtpPortCdTime)
                                {
                                    portUsed.DateTime = DateTime.Now;
                                    portUsed.Useed = true;
                                    GCommon.Logger.Info($"[{Common.LoggerHead}]->获取可用rtp端口:{minPort}");
                                    return minPort;
                                }
                            }
                        }

                        GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取可用rtp端口失败");
                        return 0;
                    }

                    for (ushort port = minPort; port <= maxPort; port++)
                    {
                        if (UtilsHelper.IsOdd(port)) //如果是奇数则跳过
                        {
                            continue;
                        }

                        var tcp2 = tcpIpEndPoints.FindLast(x => x.Port == port);
                        var udp2 = udpIpEndPoints.FindLast(x => x.Port == port);
                        var portUsed2 = Common.PortInfoList.FindLast(x => x.Port.Equals(port));
                        if (tcp2 == null && udp2 == null)
                        {
                            if (portUsed2 == null)
                            {
                                Common.PortInfoList.Add(new PortInfo()
                                {
                                    DateTime = DateTime.Now,
                                    Port = port,
                                    Useed = true,
                                });
                                GCommon.Logger.Info($"[{Common.LoggerHead}]->获取可用rtp端口:{port}");
                                return port;
                            }

                            if (!portUsed2.Useed.Equals(true))
                            {
                                if ((DateTime.Now - portUsed2.DateTime).TotalSeconds >
                                    Common.AkStreamKeeperConfig.RtpPortCdTime)
                                {
                                    portUsed2.DateTime = DateTime.Now;
                                    portUsed2.Useed = true;
                                    GCommon.Logger.Info($"[{Common.LoggerHead}]->获取可用rtp端口:{port}");
                                    return port;
                                }
                            }
                        }
                    }
                }

                GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取可用rtp端口失败");
                return 0;
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error($"[{Common.LoggerHead}]->获取可用rtp端口失败->" + ex.Message + "\r\n" + ex.StackTrace);
                return 0;
            }
        }


        /// <summary>
        /// 选择一个可用的rtp(发送)端口，仅使用偶数端口
        /// </summary>
        /// <param name="minPort"></param>
        /// <param name="maxPort"></param>
        /// <returns></returns>
        private static ushort _guessAnRtpPortForSender(ushort minPort, ushort maxPort)
        {
            try
            {
                lock (Common._getRtpPortLock)
                {
                    IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                    List<IPEndPoint> tcpIpEndPoints = ipProperties.GetActiveTcpListeners().ToList();
                    List<IPEndPoint> udpIpEndPoints = ipProperties.GetActiveUdpListeners().ToList();
                    if (minPort > maxPort)
                    {
                        var tmp = minPort;
                        maxPort = minPort;
                        minPort = tmp;
                    }


                    if (minPort == maxPort)
                    {
                        var tcp = tcpIpEndPoints.FindLast(x => x.Port == minPort);
                        var udp = udpIpEndPoints.FindLast(x => x.Port == minPort);
                        var portUsed = Common.PortInfoListForSender.FindLast(x => x.Port.Equals(minPort));
                        if (tcp == null && udp == null)
                        {
                            if (portUsed == null)
                            {
                                Common.PortInfoListForSender.Add(new PortInfo()
                                {
                                    DateTime = DateTime.Now,
                                    Port = minPort,
                                    Useed = true,
                                });
                                GCommon.Logger.Info($"[{Common.LoggerHead}]->获取可用rtp(发送)端口:{minPort}");
                                return minPort;
                            }

                            if (!portUsed.Useed.Equals(true))
                            {
                                if ((DateTime.Now - portUsed.DateTime).TotalSeconds >
                                    Common.AkStreamKeeperConfig.RtpPortCdTime)
                                {
                                    portUsed.DateTime = DateTime.Now;
                                    GCommon.Logger.Info($"[{Common.LoggerHead}]->获取可用rtp(发送)端口:{minPort}");
                                    return minPort;
                                }
                            }
                        }

                        GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取可用rtp(发送)端口失败");
                        return 0;
                    }

                    for (ushort port = minPort; port <= maxPort; port++)
                    {
                        if (UtilsHelper.IsOdd(port)) //如果是奇数则跳过
                        {
                            continue;
                        }

                        var tcp2 = tcpIpEndPoints.FindLast(x => x.Port == port);
                        var udp2 = udpIpEndPoints.FindLast(x => x.Port == port);
                        var portUsed2 = Common.PortInfoListForSender.FindLast(x => x.Port.Equals(port));
                        if (tcp2 == null && udp2 == null)
                        {
                            if (portUsed2 == null)
                            {
                                Common.PortInfoListForSender.Add(new PortInfo()
                                {
                                    DateTime = DateTime.Now,
                                    Port = port,
                                    Useed = true,
                                });
                                GCommon.Logger.Info($"[{Common.LoggerHead}]->获取可用rtp(发送)端口:{port}");
                                return port;
                            }

                            if (!portUsed2.Useed.Equals(true))
                            {
                                if ((DateTime.Now - portUsed2.DateTime).TotalSeconds >
                                    Common.AkStreamKeeperConfig.RtpPortCdTime)
                                {
                                    portUsed2.DateTime = DateTime.Now;
                                    GCommon.Logger.Info($"[{Common.LoggerHead}]->获取可用rtp(发送)端口:{port}");
                                    return port;
                                }
                            }
                        }
                    }
                }

                GCommon.Logger.Warn($"[{Common.LoggerHead}]->获取可用rtp(发送)端口失败");
                return 0;
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error($"[{Common.LoggerHead}]->获取可用rtp端口失败->" + ex.Message + "\r\n" + ex.StackTrace);
                return 0;
            }
        }

        /// <summary>
        /// 获取流媒体服务器运行状态
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static int CheckMediaServerRunning(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.MediaServerInstance != null)
            {
                if (Common.MediaServerInstance.IsRunning)
                {
                    var pid = Common.MediaServerInstance.GetPid();
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->获取流媒体服务器运行状态成功:pid:{pid}");
                    return pid;
                }

                GCommon.Logger.Error($"[{Common.LoggerHead}]->获取流媒体服务器运行状态失败");
                return -1;
            }

            GCommon.Logger.Error($"[{Common.LoggerHead}]->获取流媒体服务器运行状态失败");
            return -1;
        }

        /// <summary>
        /// 重新加载流媒体服务器配置文件（热加载）
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool ReloadMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.MediaServerInstance != null)
            {
                if (Common.MediaServerInstance.IsRunning)
                {
                    var ret = Common.MediaServerInstance.Reload();
                    if (ret > 0)
                    {
                        GCommon.Logger.Info($"[{Common.LoggerHead}]->热加载流媒体服务器配置文件成功");
                        return true;
                    }

                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.MediaServer_ReloadExcept,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ReloadExcept],
                    };
                    GCommon.Logger.Warn($"[{Common.LoggerHead}]->热加载流媒体服务器配置文件失败");
                    return false;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_NotRunning,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_NotRunning],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->热加载流媒体服务器配置文件失败->流媒体服务器没有运行");
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_InstanceIsNull,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
            };
            GCommon.Logger.Warn($"[{Common.LoggerHead}]->热加载流媒体服务器配置文件失败->流媒体服务器实例为空");
            return false;
        }

        /// <summary>
        /// 重启流媒体服务器
        /// </summary>
        /// <param name="rs"></param>
        /// <returns>流媒体服务器pid</returns>
        public static int RestartMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.MediaServerInstance != null)
            {
                var ret = Common.MediaServerInstance.Restart();
                if (ret > 0)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->重启流媒体服务器成功->PID:{ret}");
                    return ret;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_RestartExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_RestartExcept],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->重启流媒体服务器失败");
                return -1;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_InstanceIsNull,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
            };
            GCommon.Logger.Warn($"[{Common.LoggerHead}]->重启流媒体服务器失败->流媒体服务实例为空");
            return -1;
        }

        /// <summary>
        /// 终止流媒体服务器
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool ShutdownMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.MediaServerInstance != null)
            {
                if (!Common.MediaServerInstance.IsRunning)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->关闭流媒体服务器->流媒体不在运行状态");
                    return true;
                }

                var ret = Common.MediaServerInstance.Shutdown();
                if (ret)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->关闭流媒体服务器成功");
                    return ret;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_ShutdownExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_ShutdownExcept],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->关闭流媒体服务器失败");
                return false;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_InstanceIsNull,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
            };
            GCommon.Logger.Warn($"[{Common.LoggerHead}]->关闭流媒体服务器失败->流媒体服务器实例为空");
            return false;
        }

        /// <summary>
        /// 启动流媒体服务器
        /// </summary>
        /// <param name="rs"></param>
        /// <returns>流媒体服务器pid</returns>
        public static int StartMediaServer(out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (Common.MediaServerInstance != null)
            {
                if (Common.MediaServerInstance.IsRunning)
                {
                    var pid = Common.MediaServerInstance.GetPid();
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->启动流媒体服务器->流媒体服务器处于启动状态->PID:{pid}");
                    return pid;
                }

                var ret = Common.MediaServerInstance.Startup();
                if (ret > 0)
                {
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->启动流媒体服务器成功->PID:{ret}");
                    return ret;
                }

                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.MediaServer_StartUpExcept,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_StartUpExcept],
                };
                GCommon.Logger.Warn($"[{Common.LoggerHead}]->启动流媒体服务器失败");
                return -1;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.MediaServer_InstanceIsNull,
                Message = ErrorMessage.ErrorDic![ErrorNumber.MediaServer_InstanceIsNull],
            };
            GCommon.Logger.Warn($"[{Common.LoggerHead}]->启动流媒体服务器失败->流媒体服务实例为空");
            return -1;
        }

        /// <summary>
        /// 清理空目录
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool CleanUpEmptyDir(out ResponseStruct rs, string rootPath = "")
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (string.IsNullOrEmpty(rootPath))
            {
                foreach (var path in Common.AkStreamKeeperConfig.CustomRecordPathList)
                {
                    string dirList = "";
                    if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                    {
                        try
                        {
                            DirectoryInfo dir = new DirectoryInfo(path);
                            DirectoryInfo[] subdirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);
                            foreach (DirectoryInfo subdir in subdirs)
                            {
                                FileSystemInfo[] subFiles = subdir.GetFileSystemInfos();
                                var l = subFiles.Length;
                                if (l == 0)
                                {
                                    subdir.Delete();
                                    dirList += "清理空目录 ->" + subdir + "\r\n";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            GCommon.Logger.Error(
                                $@"[{Common.LoggerHead}]->清理空目录时发生异常->{ex.Message}\r\n{ex.StackTrace}");
                        }

                        if (!string.IsNullOrEmpty(dirList))
                        {
                            GCommon.Logger.Info($@"[{Common.LoggerHead}]->{dirList}");
                        }
                    }
                }
            }
            else
            {
                string dirList = "";
                if (!string.IsNullOrEmpty(rootPath) && Directory.Exists(rootPath))
                {
                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo(rootPath);
                        DirectoryInfo[] subdirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);
                        foreach (DirectoryInfo subdir in subdirs)
                        {
                            FileSystemInfo[] subFiles = subdir.GetFileSystemInfos();
                            var l = subFiles.Length;
                            if (l == 0)
                            {
                                subdir.Delete();
                                dirList += "清理空目录 ->" + subdir + "\r\n";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        GCommon.Logger.Error($@"[{Common.LoggerHead}]->清理空目录时发生异常->{ex.Message}\r\n{ex.StackTrace}");
                    }

                    if (!string.IsNullOrEmpty(dirList))
                    {
                        GCommon.Logger.Info($@"[{Common.LoggerHead}]->{dirList}");
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="fileList"></param>
        /// <param name="rs"></param>
        /// <returns>返回未正常删除的文件列表</returns>
        public static ResKeeperDeleteFileList DeleteFileList(List<string> fileList, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var result = new List<string>();
            if (fileList != null && fileList.Count > 0)
            {
                foreach (var path in fileList)
                {
                    if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                        }
                        catch
                        {
                            result.Add(path);
                        }
                    }

                    Thread.Sleep(10);
                }

                if (result.Count > 0)
                {
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->批量删除文件时有部分文件没有被删除->{JsonHelper.ToJson(result, Formatting.Indented)}");
                }

                return new ResKeeperDeleteFileList()
                {
                    PathList = result,
                };
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Sys_ParamsIsNotRight,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_ParamsIsNotRight],
            };
            return new ResKeeperDeleteFileList();
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool FileExists(string filePath, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            var found = File.Exists(filePath);
            GCommon.Logger.Debug($"[{Common.LoggerHead}]->检查文件是否存在:{filePath}:{found}");
            return found;
        }

        /// <summary>
        /// 删除一个指定文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static bool DeleteFile(string filePath, out ResponseStruct rs)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                    GCommon.Logger.Info($"[{Common.LoggerHead}]->删除文件:{filePath}成功");
                    return true;
                }
                catch (Exception ex)
                {
                    rs = new ResponseStruct()
                    {
                        Code = ErrorNumber.None,
                        Message = ErrorMessage.ErrorDic![ErrorNumber.None],
                        ExceptMessage = ex.Message,
                        ExceptStackTrace = ex.StackTrace,
                    };
                    GCommon.Logger.Warn(
                        $"[{Common.LoggerHead}]->删除文件:{filePath}失败->{JsonHelper.ToJson(rs, Formatting.Indented)}");
                    return false;
                }
            }
            else
            {
                rs = new ResponseStruct()
                {
                    Code = ErrorNumber.Sys_SpecifiedFileNotExists,
                    Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_SpecifiedFileNotExists],
                };
                GCommon.Logger.Warn(
                    $"[{Common.LoggerHead}]->删除文件:{filePath}失败->{JsonHelper.ToJson(rs, Formatting.Indented)}");
                return false;
            }
        }

        /// <summary>
        /// 找一个可用的rtp端口
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ushort GuessAnRtpPort(out ResponseStruct rs, ushort? min = 0, ushort? max = 0)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            ushort port = 0;
            if ((min == null || min == 0) && (max == null || max == 0))
            {
                port = _guessAnRtpPort(Common.AkStreamKeeperConfig.MinRtpPort,
                    Common.AkStreamKeeperConfig.MaxRtpPort);
            }
            else
            {
                port = _guessAnRtpPort((ushort)min, (ushort)max);
            }

            if (port > 0)
            {
                return port;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Sys_SocketPortForRtpExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_SocketPortForRtpExcept],
            };
            return 0;
        }

        /// <summary>
        /// 找一个可用的rtp(发送)端口
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public static ushort GuessAnRtpPortForSender(out ResponseStruct rs, ushort? min = 0, ushort? max = 0)
        {
            rs = new ResponseStruct()
            {
                Code = ErrorNumber.None,
                Message = ErrorMessage.ErrorDic![ErrorNumber.None],
            };
            ushort port = 0;
            if ((min == null || min == 0) && (max == null || max == 0))
            {
                port = _guessAnRtpPortForSender(Common.AkStreamKeeperConfig.MinSendRtpPort,
                    Common.AkStreamKeeperConfig.MaxSendRtpPort);
            }
            else
            {
                port = _guessAnRtpPortForSender((ushort)min, (ushort)max);
            }

            if (port > 0)
            {
                return port;
            }

            rs = new ResponseStruct()
            {
                Code = ErrorNumber.Sys_SocketPortForRtpExcept,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_SocketPortForRtpExcept],
            };
            return 0;
        }
    }
}