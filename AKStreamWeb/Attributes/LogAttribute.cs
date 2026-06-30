using System;
using System.Net;
using LibCommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AKStreamWeb.Attributes
{
    /// <summary>
    /// 日志记录类，所有经过httpcontroller的进出日志都会被记录
    /// </summary>
    public class LogAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            string info = "StatusCode:Unknown";
            string remoteIpAddr = "unknown";
            string method = "unknown";
            string path = "unknown";

            try
            {
                remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                method = context.HttpContext.Request.Method;
                path = context.HttpContext.Request.Path.ToString();
                info = $@"StatusCode:{context.HttpContext.Response.StatusCode}";

                if (context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
                {
                    return;
                }

                if (context.Exception != null)
                {
                    info = $@"{info}->Exception:{context.Exception.Message}";
                    GCommon.Logger.Warn(
                        $@"[{Common.LoggerHead}]->HTTP-OUTPUT->{remoteIpAddr}->{method}->{path}->" + info);
                    return;
                }

                if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                    string body;

                    if (context.Result is ObjectResult objectResult)
                    {
                        body = JsonHelper.ToJson(objectResult.Value);
                    }
                    else if (context.Result is JsonResult jsonResult)
                    {
                        body = JsonHelper.ToJson(jsonResult.Value);
                    }
                    else if (context.Result is ContentResult contentResult)
                    {
                        body = contentResult.Content ?? "";
                    }
                    else if (context.Result is StatusCodeResult statusCodeResult)
                    {
                        body = $"StatusCodeResult:{statusCodeResult.StatusCode}";
                    }
                    else if (context.Result is EmptyResult)
                    {
                        body = "";
                    }
                    else if (context.Result != null)
                    {
                        body = $"ResultType:{context.Result.GetType().FullName}";
                    }
                    else
                    {
                        body = "null";
                    }

                    info = $@"{info}->Body: {body}";
                    GCommon.Logger.Debug(
                        $@"[{Common.LoggerHead}]->HTTP-OUTPUT->{remoteIpAddr}->{method}->{path}->" + info);
                }
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(
                    $@"[{Common.LoggerHead}]->HTTP-OUTPUT->{remoteIpAddr}->{method}->{path}->" +
                    info + "->记录HTTP输出日志时发生异常->" + ex.Message + "->" + ex.StackTrace);
            }
        }
        // public void OnActionExecuted(ActionExecutedContext context)
        // {
        //     string info = $@"StatusCode:{context.HttpContext.Response.StatusCode}";
        //     string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
        //     try
        //     {
        //         if (context.HttpContext.Response.StatusCode == (int)HttpStatusCode.OK)
        //         {
        //             if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
        //             {
        //                 info =
        //                     $@"{info}->Body: {JsonHelper.ToJson(((context.Result as ObjectResult)!).Value)}";
        //                 GCommon.Logger.Debug(
        //                     $@"[{Common.LoggerHead}]->HTTP-OUTPUT->{remoteIpAddr}->{context.HttpContext.Request.Method}->{context.HttpContext.Request.Path}->" +
        //                     info);
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         GCommon.Logger.Error(
        //             $@"[{Common.LoggerHead}]->HTTP-OUTPUT->{remoteIpAddr}->{context.HttpContext.Request.Method}->{context.HttpContext.Request.Path}->" +
        //             info + "->" + ex.Message + "->" + ex.StackTrace);
        //     }
        // }

        /// <summary>
        /// 请求中
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            string remoteIpAddr = "unknown";
            string method = "unknown";
            string path = "unknown";

            try
            {
                remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                method = context.HttpContext.Request.Method;
                path = context.HttpContext.Request.Path.ToString();

                if (string.Equals(path, "/WebHook/MediaServerRegister", StringComparison.InvariantCultureIgnoreCase))
                {
                    return;
                }

                var args = JsonHelper.ToJson(context.ActionArguments);
                if (string.IsNullOrEmpty(args))
                {
                    args = "{}";
                }

                GCommon.Logger.Debug(
                    $@"[{Common.LoggerHead}]->HTTP-INPUT->{remoteIpAddr}->{method}->{path}->{args}");
            }
            catch (Exception ex)
            {
                GCommon.Logger.Error(
                    $@"[{Common.LoggerHead}]->HTTP-INPUT->{remoteIpAddr}->{method}->{path}->" +
                    "记录HTTP输入日志时发生异常->" + ex.Message + "->" + ex.StackTrace);
            }
        }
        // public void OnActionExecuting(ActionExecutingContext context)
        // {
        //     string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
        //     try
        //     {
        //         if (context.HttpContext.Request.Method.Equals("get", StringComparison.InvariantCultureIgnoreCase))
        //         {
        //             if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
        //             {
        //                 GCommon.Logger.Debug(
        //                     $@"[{Common.LoggerHead}]->HTTP-INPUT->{remoteIpAddr}->{context.HttpContext.Request.Method}->{context.HttpContext.Request.Path}->" +
        //                     $@"{JsonHelper.ToJson(context.ActionArguments)}");
        //             }
        //         }
        //         else
        //         {
        //             if (!context.HttpContext.Request.Path.Equals("/WebHook/MediaServerRegister"))
        //             {
        //                 GCommon.Logger.Debug(
        //                     $@"[{Common.LoggerHead}]->HTTP-INPUT->{remoteIpAddr}->{context.HttpContext.Request.Method}->{context.HttpContext.Request.Path} -> " +
        //                     $@"{JsonHelper.ToJson(context.ActionArguments)}");
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         GCommon.Logger.Error(
        //             $@"[{Common.LoggerHead}]->HTTP-INPUT->{remoteIpAddr}->{context.HttpContext.Request.Method}->{context.HttpContext.Request.Path}->" +
        //             remoteIpAddr + "->" + ex.Message + "->" + ex.StackTrace);
        //     }
        // }
    }
}