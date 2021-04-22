using System;
using System.Collections.Generic;
using System.IO;
using LibCommon.Structs;

namespace LibSystemInfo
{
    public static class DiskInfoValue
    {
        /// <summary>
        /// 获取当前驱动使用情况
        /// </summary>
        public static List<DriveInfoDiy> GetDrivesInfo()
        {
            DriveInfo[] driveInfoArr = null;
            driveInfoArr = DriveInfo.GetDrives();
            List<DriveInfoDiy> result = new List<DriveInfoDiy>();
            foreach (var drv in driveInfoArr)
            {
                DriveInfoDiy driveInfo = new DriveInfoDiy();
                if (drv.IsReady && drv.DriveType != DriveType.Removable && drv.TotalSize > 0)
                {
                    driveInfo.Name = drv.Name;
                    driveInfo.IsReady = drv.IsReady;
                    driveInfo.Total = drv.TotalSize;
                    driveInfo.Free = drv.AvailableFreeSpace;
                    driveInfo.Used = drv.TotalSize - drv.AvailableFreeSpace;
                    driveInfo.FreePercent = Math.Round(drv.AvailableFreeSpace * 100.00 / drv.TotalSize, 3);
                    driveInfo.UpdateTime = DateTime.Now;
                    if (!driveInfo.Name.ToLower().Trim().StartsWith("/boot") &&
                        !driveInfo.Name.ToLower().Trim().StartsWith("/dev") &&
                        !driveInfo.Name.ToLower().Trim().StartsWith("/run") &&
                        !driveInfo.Name.ToLower().Trim().StartsWith("/sys") &&
                        !driveInfo.Name.ToLower().Trim().StartsWith("/var")
                    )
                    {
                        result.Add(driveInfo);
                    }
                }
            }

            return result;
        }
    }
}