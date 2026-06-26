using System;
using System.IO;
using LibCommon;

namespace AKStreamKeeper.Misc
{
    public static class FFmpegGetDuration
    {
        private static bool IfNotMp4(string ffmpegBinPath, string videoFilePath, out string videoPath)
        {
            string ext = Path.GetExtension(videoFilePath);
            string newFileName = videoFilePath.Replace(ext, ".mp4");
            string args = " -i " + videoFilePath + " -c copy -movflags faststart " + newFileName;
            videoPath = newFileName;
            if (!string.IsNullOrEmpty(ext) && !ext.Trim().ToLower().Equals(".mp4"))
            {
                ProcessHelper tmpProcessHelper = new ProcessHelper(null, null, null);
                if (tmpProcessHelper.RunProcess(ffmpegBinPath, args, 60 * 1000 * 5, out string std, out string err))
                {
                    if (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err))
                    {
                        if (File.Exists(newFileName))
                        {
                            FileInfo fi = new FileInfo(newFileName);
                            if (fi.Length > 100)
                            {
                                File.Delete(videoFilePath);
                                return true;
                            }

                            return false;
                        }

                        return false;
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 输出视频的时长（毫秒）
        /// </summary>
        /// <param name="ffmpegBinPath"></param>
        /// <param name="videoFilePath"></param>
        /// <param name="duartion"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool GetDuration(string ffmpegBinPath, string videoFilePath, out long duartion, out string path)
        {
            duartion = -1;
            if (File.Exists(ffmpegBinPath) && File.Exists(videoFilePath))
            {
                string newPath = "";
                var ret = IfNotMp4(ffmpegBinPath, videoFilePath, out newPath);
                if (ret)
                {
                    videoFilePath = newPath;
                }

                path = videoFilePath;
                string args = " -i " + videoFilePath;
                ProcessHelper tmpProcessHelper = new ProcessHelper(null, null, null);
                if (tmpProcessHelper.RunProcess(ffmpegBinPath, args, 1000, out string std, out string err))
                {
                    if (!string.IsNullOrEmpty(std) || !string.IsNullOrEmpty(err))
                    {
                        string tmp = "";
                        if (!string.IsNullOrEmpty(std))
                        {
                            tmp = UtilsHelper.GetValue(std, "Duration:", ",");
                        }

                        if (string.IsNullOrEmpty(tmp))
                        {
                            tmp = UtilsHelper.GetValue(err, "Duration:", ",");
                        }

                        if (!string.IsNullOrEmpty(tmp))
                        {
                            string[] tmpArr = tmp.Split(':', StringSplitOptions.RemoveEmptyEntries);
                            if (tmpArr.Length == 3)
                            {
                                int hour = int.Parse(tmpArr[0]);
                                int min = int.Parse(tmpArr[1]);
                                int sec = 0;
                                int msec = 0;
                                if (tmpArr[2].Contains('.'))
                                {
                                    string[] tmpArr2 = tmpArr[2].Split('.', StringSplitOptions.RemoveEmptyEntries);
                                    sec = int.Parse(tmpArr2[0]);
                                    msec = int.Parse(tmpArr2[1]);
                                }
                                else
                                {
                                    sec = int.Parse(tmpArr[2]);
                                }

                                hour = hour * 3600; //换成秒数
                                min = min * 60;
                                sec = sec + hour + min; //合计秒数
                                duartion = sec * 1000 + (msec * 10); //算成毫秒
                                return true;
                            }
                        }
                    }
                }
            }

            path = videoFilePath;
            return false;
        }
    }
}