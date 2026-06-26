using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace LibCommon
{
    /// <summary>
    /// http请求类
    /// </summary>
    public static class NetHelper
    {
        /// <summary>
        /// 不检查证书
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain,
            SslPolicyErrors errors)
        {
            return true; //总是接受
        }
        //todo:重写
        /*
            用HttpClientFactory重构。 参考:https://www.cnblogs.com/deepthought/p/11303015.html
            使用RestSharp https://github.com/restsharp/RestSharp   https://restsharp.dev/getting-started/
         */

        #region Delete

        public static string HttpDeleteRequest(string url, Dictionary<string, string> headers, string encode = "utf-8",
            int timeout = 60000)
        {
            return HttpDeleteRequestByProxy(url, headers, null!, encode, timeout);
        }

        /// <summary>
        /// http delete请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">headers</param>
        /// <param name="proxy">代理地址</param>
        /// <param name="encode">字符编码</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static string HttpDeleteRequestByProxy(string url, Dictionary<string, string> headers, IWebProxy porxy,
            string encode, int timeout = 60000)
        {
            //验证主机名和服务器验证方案的匹配是可接受的。   
            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(CheckValidationResult);

            // 这里设置了协议类型。
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "DELETE";
            request.Timeout = timeout;
            request.Proxy = porxy;

            if (headers != null && headers.Keys.Count > 0)
            {
                foreach (var key in headers.Keys)
                {
                    request.Headers.Add(key, headers[key]);
                }
            }

            HttpWebResponse myResponse = null!;
            try
            {
                myResponse = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(encode));
                string content = reader.ReadToEnd();
                return content;
            }
            //异常请求  
            catch (WebException e)
            {
                myResponse = (HttpWebResponse)e.Response;
                if (myResponse == null) return e.Message;
                using (Stream errData = myResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(errData))
                    {
                        string text = reader.ReadToEnd();

                        return text;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (myResponse != null)
                {
                }

                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        #endregion

        #region Get

        /// <summary>
        /// http get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers">headers</param>
        /// <param name="encode"></param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static string HttpGetRequest(string url, Dictionary<string, string> headers, string encode = "utf-8",
            int timeout = 60000)
        {
            return HttpGetRequestByProxy(url, headers, null!, encode, timeout);
        }

        /// <summary>
        /// http get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">headers</param>
        /// <param name="proxy">代理地址</param>
        /// <param name="encode">字符编码</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static string HttpGetRequestByProxy(string url, Dictionary<string, string> headers, IWebProxy porxy,
            string encode, int timeout = 60000)
        {
            //验证主机名和服务器验证方案的匹配是可接受的。   
            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(CheckValidationResult);

            // 这里设置了协议类型。
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "GET";
            request.Timeout = timeout;
            request.Proxy = porxy;

            if (headers != null && headers.Keys.Count > 0)
            {
                foreach (var key in headers.Keys)
                {
                    request.Headers.Add(key, headers[key]);
                }
            }

            HttpWebResponse myResponse = null!;
            try
            {
                myResponse = (HttpWebResponse)request.GetResponse();
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(encode));
                string content = reader.ReadToEnd();
                return content;
            }
            //异常请求  
            catch (WebException e)
            {
                myResponse = (HttpWebResponse)e.Response;
                if (myResponse == null) return e.Message;
                using (Stream errData = myResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(errData))
                    {
                        string text = reader.ReadToEnd();

                        return text;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (myResponse != null)
                {
                }

                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        #endregion

        #region Get(Async)

        /// <summary>
        /// http get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers">headers</param>
        /// <param name="encode"></param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static async Task<string> HttpGetRequestAsync(string url, Dictionary<string, string> headers,
            string encode = "utf-8", int timeout = 60000)
        {
            return await HttpGetRequestByProxyAsync(url, headers, null!, encode, timeout);
        }

        /// <summary>
        /// http get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">headers</param>
        /// <param name="proxy">代理地址</param>
        /// <param name="encode">字符编码</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static async Task<string> HttpGetRequestByProxyAsync(string url, Dictionary<string, string> headers,
            IWebProxy porxy, string encode, int timeout = 60000)
        {
            //验证主机名和服务器验证方案的匹配是可接受的。   
            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(CheckValidationResult);

            // 这里设置了协议类型。
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Method = "GET";
            request.Timeout = timeout;
            request.Proxy = porxy;

            if (headers != null && headers.Keys.Count > 0)
            {
                foreach (var key in headers.Keys)
                {
                    request.Headers.Add(key, headers[key]);
                }
            }

            HttpWebResponse myResponse = null!;
            try
            {
                myResponse = (await request.GetResponseAsync() as HttpWebResponse)!;
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding(encode));
                string content = reader.ReadToEnd();
                return content;
            }
            //异常请求  
            catch (WebException e)
            {
                myResponse = (HttpWebResponse)e.Response;
                if (myResponse == null) return e.Message;
                using (Stream errData = myResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(errData))
                    {
                        string text = reader.ReadToEnd();

                        return text;
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (myResponse != null)
                {
                }

                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        #endregion


        #region post

        /// <summary>
        /// http post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">headers</param>
        /// <param name="param">请求参数</param>
        /// <param name="encode">字符编码</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static string HttpPostRequest(string url, Dictionary<string, string> headers, string param,
            string encode = "utf-8", int timeout = 60000)
        {
            return HttpPostRequestByProxy(url, headers, param, null!, encode, timeout);
        }

        /// <summary>
        /// http post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">headers</param>
        /// <param name="param">请求参数</param>
        /// <param name="proxy">代理地址</param>
        /// <param name="encode">字符编码</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static string HttpPostRequestByProxy(string url, Dictionary<string, string> headers, string param,
            IWebProxy proxy, string encode, int timeout = 60000)
        {
            #region https 请求设置

            //验证主机名和服务器验证方案的匹配是可接受的。   
            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(CheckValidationResult);

            // 这里设置了协议类型。
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;

            #endregion

            string result = string.Empty;
            HttpWebRequest request = null!;
            HttpWebResponse response = null!;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                //request.Accept = "*/*";
                request.Accept =
                    "application/json,text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                //request.Accept = "application/json";
                //request.ContentType = string.Format("application/x-www-form-urlencoded;charset={0}", encode);
                request.ContentType = string.Format("application/json;charset={0}", encode);
                request.UserAgent = null;
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
                request.Proxy = proxy;

                if (headers != null && headers.Keys.Count > 0)
                {
                    foreach (var key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(param);
                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                newStream.Dispose();

                //获取响应结果
                response = (request.GetResponse() as HttpWebResponse)!;
                Stream stream = response.GetResponseStream();

                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }

                stream.Close();
                stream.Dispose();

                return result;
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
                if (response == null) return e.Message;
                using (Stream errData = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(errData))
                    {
                        string text = reader.ReadToEnd();

                        return text;
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                if (response != null)
                {
                }

                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        #endregion

        #region post(async)

        /// <summary>
        /// http post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">headers</param>
        /// <param name="param">请求参数</param>
        /// <param name="encode">字符编码</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static async Task<string> HttpPostRequestAsync(string url, Dictionary<string, string> headers,
            string param, string encode = "utf-8", int timeout = 60000)
        {
            return await HttpPostRequestByProxyAsync(url, headers, param, null!, encode, timeout);
        }

        /// <summary>
        /// http post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="headers">headers</param>
        /// <param name="param">请求参数</param>
        /// <param name="proxy">代理地址</param>
        /// <param name="encode">字符编码</param>
        /// <param name="timeout">超时时间（毫秒）</param>
        /// <returns></returns>
        public static async Task<string> HttpPostRequestByProxyAsync(string url, Dictionary<string, string> headers,
            string param, IWebProxy proxy, string encode, int timeout = 60000)
        {
            #region https 请求设置

            //验证主机名和服务器验证方案的匹配是可接受的。   
            ServicePointManager.ServerCertificateValidationCallback =
                new RemoteCertificateValidationCallback(CheckValidationResult);

            // 这里设置了协议类型。
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.DefaultConnectionLimit = 100;
            ServicePointManager.Expect100Continue = false;

            #endregion

            string result = string.Empty;
            HttpWebRequest request = null!;
            HttpWebResponse response = null!;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                //request.Accept = "*/*";
                request.Accept =
                    "application/json,text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                //request.Accept = "application/json";
                //request.ContentType = string.Format("application/x-www-form-urlencoded;charset={0}", encode);
                request.ContentType = string.Format("application/json;charset={0}", encode);
                request.UserAgent = null;
                request.Timeout = timeout;
                request.ReadWriteTimeout = 60000;
                request.Proxy = proxy;

                if (headers != null && headers.Keys.Count > 0)
                {
                    foreach (var key in headers.Keys)
                    {
                        request.Headers.Add(key, headers[key]);
                    }
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(param);
                Stream newStream = request.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                newStream.Dispose();

                //获取响应结果
                response = (await request.GetResponseAsync() as HttpWebResponse)!;
                Stream stream = response.GetResponseStream();

                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }

                stream.Close();
                stream.Dispose();

                return result;
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
                if (response == null) return e.Message;
                using (Stream errData = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(errData))
                    {
                        string text = reader.ReadToEnd();

                        return text;
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                if (response != null)
                {
                }

                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        #endregion

        #region 下载

        /// <summary>
        /// 从文件头得到远程文件的长度
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static long GetHttpLength(string url)
        {
            long length = 0;

            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url); //打开网络连接
                HttpWebResponse rsp = (HttpWebResponse)req.GetResponse();

                if (rsp.StatusCode == HttpStatusCode.OK)
                {
                    length = rsp.ContentLength; //从文件头得到远程文件的长度
                }

                rsp.Close();
                return length;
            }
            catch (Exception)
            {
                return length;
            }
        }

        /// <summary>
        /// 文件转base64
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static string FileToBase64(string filePath)
        {
            FileStream fsForRead = new FileStream(filePath, FileMode.Open);

            try
            {
                fsForRead.Seek(0, SeekOrigin.Begin);
                byte[] bs = new byte[fsForRead.Length];
                int log = Convert.ToInt32(fsForRead.Length);
                fsForRead.Read(bs, 0, log);
                return Convert.ToBase64String(bs);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                fsForRead.Close();
            }
        }

        /// <summary>
        /// 下载文件并以base64输出
        /// </summary>
        /// <param name="url"></param>
        /// <param name="base64out"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static bool DownloadFileToBase64(string url, out string base64out)
        {
            base64out = "";
            string tmpFilePath = GCommon.TmpPicsPath + "Snap_" + DateTime.Now.Ticks + ".jpg";
            var ret = Download(url, tmpFilePath);
            if (ret)
            {
                try
                {
                    base64out = FileToBase64(tmpFilePath);
                    Console.WriteLine(base64out);
                    File.Delete(tmpFilePath);
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return false;
        }


        /// <summary>
        /// Http方式下载文件
        /// </summary>
        /// <param name="url">http地址</param>
        /// <param name="localfile">本地文件</param>
        /// <returns></returns>
        public static bool Download(string url, string localfile)
        {
            bool flag = false;
            long startPosition = 0; // 上次下载的文件起始位置
            FileStream writeStream; // 写入本地文件流对象

            long remoteFileLength = GetHttpLength(url); // 取得远程文件长度

            if (remoteFileLength <= 0 || remoteFileLength == 745)
            {
                return false;
            }

            // 判断要下载的文件夹是否存在
            if (File.Exists(localfile))
            {
                writeStream = File.OpenWrite(localfile); // 存在则打开要下载的文件
                startPosition = writeStream.Length; // 获取已经下载的长度

                if (startPosition >= remoteFileLength)
                {
                    GCommon.Logger.Debug("本地文件长度" + startPosition + "已经大于等于远程文件长度" + remoteFileLength + "。下载完成。");
                    writeStream.Close();

                    return false;
                }
                else
                {
                    writeStream.Seek(startPosition, SeekOrigin.Current); // 本地文件写入位置定位
                }
            }
            else
            {
                writeStream = new FileStream(localfile, FileMode.Create); // 文件不保存创建一个文件
                startPosition = 0;
            }

            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(url); // 打开网络连接

                if (startPosition > 0)
                {
                    myRequest.AddRange((int)startPosition); // 设置Range值,与上面的writeStream.Seek用意相同,是为了定义远程文件读取位置
                }

                Stream readStream = myRequest.GetResponse().GetResponseStream(); // 向服务器请求,获得服务器的回应数据流

                byte[] btArray = new byte[512]; // 定义一个字节数据,用来向readStream读取内容和向writeStream写入内容
                int contentSize = readStream.Read(btArray, 0, btArray.Length); // 向远程文件读第一次

                long currPostion = startPosition;

                while (contentSize > 0) // 如果读取长度大于零则继续读
                {
                    currPostion += contentSize;
                    int percent = (int)(currPostion * 100 / remoteFileLength);


                    writeStream.Write(btArray, 0, contentSize); // 写入本地文件
                    contentSize = readStream.Read(btArray, 0, btArray.Length); // 继续向远程文件读取
                }

                //关闭流
                writeStream.Close();
                readStream.Close();

                flag = true; //返回true下载成功
            }
            catch (Exception)
            {
                writeStream.Close();
                flag = false; //返回false下载失败
            }

            return flag;
        }

        #endregion
    }
}