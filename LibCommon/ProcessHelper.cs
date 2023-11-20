using System;
using System.Diagnostics;
using System.IO;

namespace LibCommon
{
    public class ProcessHelper
    {
        private DataReceivedEventHandler _err = null!;
        private EventHandler _exitEventHandle = null!;
        private DataReceivedEventHandler _std = null!;


        public ProcessHelper(DataReceivedEventHandler std = null!, DataReceivedEventHandler err = null!,
            EventHandler exitEvent = null!)
        {
            _std = std;
            _err = err;
            _exitEventHandle = exitEvent;
        }


        /// <summary>
        /// 杀掉进程
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static void KillProcess(string filePath)
        {
            Process[] processes =
                Process.GetProcessesByName(Path.GetFileNameWithoutExtension(filePath));
            foreach (var process in processes)
            {
                process.Kill();
            }
        }

        /// <summary>
        /// 杀死进程
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public bool KillProcess(Process process)
        {
            if (!process.HasExited)
            {
                process.Kill();
            }

            Process[] processes =
                Process.GetProcessesByName(Path.GetFileNameWithoutExtension(process?.StartInfo.FileName));
            return !(processes.Length > 0);
        }

        /// <summary>
        /// 检查进程是否正在运行
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public int CheckProcessExists(Process process)
        {
            if (process.HasExited)
            {
                return -1;
            }

            Process[] processes =
                Process.GetProcessesByName(Path.GetFileNameWithoutExtension(process.StartInfo.FileName));
            bool hasValue = processes.Length > 0;
            if (hasValue)
            {
                return processes[0].Id;
            }

            return -1;
        }


        /// <summary>
        /// 执行外部程序，含超时
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="args"></param>
        /// <param name="milliseconds"></param>
        /// <param name="stdOutput"></param>
        /// <param name="stdError"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public bool RunProcess(string filePath, string args, int milliseconds, out string stdOutput,
            out string stdError)
        {
            stdOutput = null!;
            stdError = null!;
            try
            {
                string escapedArgs = args.Replace("\"", "\\\"");

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException(filePath + "不存在");
                }

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = filePath;
                    process.StartInfo.UseShellExecute = false; //不使用shell以免出现操作系统shell出错
                    process.StartInfo.CreateNoWindow = true; //不显示窗口
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.Arguments = escapedArgs;

                    bool result = process.Start();
                    if (result)
                    {
                        result = process.WaitForExit(milliseconds);
                    }

                    if (result)
                    {
                        stdOutput = process.StandardOutput.ReadToEnd();
                        stdError = process.StandardError.ReadToEnd()!;
                    }

                    return result;
                }
            }
            catch (Exception ex) //异常直接返回错误
            {
                //异常处理
                throw ex;
            }
        }


        /// <summary>
        /// 执行程序，直到结束
        /// </summary>
        /// <param name="StartFileName"></param>
        /// <param name="StartFileArg"></param>
        /// <returns></returns>
        public Process RunProcess(string StartFileName, string StartFileArg)
        {
            Process CmdProcess = new Process();
            CmdProcess.StartInfo.FileName = StartFileName; // 命令
            CmdProcess.StartInfo.Arguments = StartFileArg; // 参数

            CmdProcess.StartInfo.CreateNoWindow = true; // 不创建新窗口
            CmdProcess.StartInfo.UseShellExecute = false;
            CmdProcess.StartInfo.RedirectStandardInput = true; // 重定向输入
            CmdProcess.StartInfo.RedirectStandardOutput = true; // 重定向标准输出
            CmdProcess.StartInfo.RedirectStandardError = true; // 重定向错误输出
            if (_std != null)
            {
                CmdProcess.OutputDataReceived += _std;
            }

            if (_err != null)
            {
                CmdProcess.ErrorDataReceived += _err;
            }

            CmdProcess.EnableRaisingEvents = true; // 启用Exited事件
            if (_exitEventHandle != null)
            {
                CmdProcess.Exited += _exitEventHandle; // 注册进程结束事件
            }

            CmdProcess.Start();
            CmdProcess.BeginOutputReadLine();
            CmdProcess.BeginErrorReadLine();
            return CmdProcess;
        }
    }
}