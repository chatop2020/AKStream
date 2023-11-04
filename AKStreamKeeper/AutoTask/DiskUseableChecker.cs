using System;
using System.Runtime.InteropServices;
using System.Threading;
using LibCommon;

namespace AKStreamKeeper.AutoTask;

public class DiskUseableChecker
{
    public DiskUseableChecker()
    {
        if (Common.AkStreamKeeperConfig.CheckLinuxDiskMount == true &&
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
            Common.AkStreamKeeperConfig.CustomRecordPathList != null &&
            Common.AkStreamKeeperConfig.CustomRecordPathList.Count > 0)
        {
            new Thread(new ThreadStart(delegate
            {
                try
                {
                    Checker();
                }
                catch
                {
                }
            })).Start();
        }
    }

    private void Checker()
    {
        while (true)
        {
            try
            {
                lock (Common.DisksUseable)
                {
                    Common.DisksUseable.Clear();
                    foreach (var path in Common.AkStreamKeeperConfig
                                 .CustomRecordPathList)
                    {
                        var ret = UtilsHelper.DirAreMounttedAndWriteableForLinux(path);
                        Common.DisksUseable.Add(path, ret);
                    }
                }
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error($"[{Common.LoggerHead}]->执行磁盘挂载检测时出现异常->{ex.Message}\r\n{ex.StackTrace}");
            }

            Thread.Sleep(5000);
        }
    }
}