using System;
using System.Threading.Tasks;
using LibCommon;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AKStreamKeeper
{
    /// <summary>
    /// 异常情况处理的中间件
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private IHostingEnvironment environment;

        public ExceptionMiddleware(RequestDelegate next, IHostingEnvironment environment)
        {
            this.next = next;
            this.environment = environment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
                var features = context.Features;
            }
            catch (AkStreamException e)
            {
                await MyHandleException(context, e);
            }
            catch (Exception e)
            {
                await HandleException(context, e);
            }
        }

        private async Task MyHandleException(HttpContext context, AkStreamException e)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "text/json;charset=utf-8;";
            string error = JsonHelper.ToJson(e.ResponseStruct);
            string info = $@"StatusCode:{context.Response.StatusCode}";
            string remoteIpAddr = context.Connection.RemoteIpAddress.ToString();
            info = $@"{info}->Body: {error}";
            GCommon.Logger.Error(
                $@"[{Common.LoggerHead}]->HTTP-OUTPUT->{remoteIpAddr}->{context.Request.Method}->{context.Request.Path}->" +
                info);

            await context.Response.WriteAsync(error);
        }

        private async Task HandleException(HttpContext context, Exception e)
        {
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/json;charset=utf-8;";
            string error = "";
            if (environment.IsDevelopment())
            {
                var json = new { message = e.Message };
                error = JsonConvert.SerializeObject(json);
            }
            else error = "抱歉，出错了\r\n" + e.Message + "\r\n" + e.StackTrace;

            string info = $@"StatusCode:{context.Response.StatusCode}";
            string remoteIpAddr = context.Connection.RemoteIpAddress.ToString();
            info = $@"{info}  Body: {error}";
            GCommon.Logger.Error(
                $@"[{Common.LoggerHead}]->HTTP-OUTPUT->{remoteIpAddr}->{context.Request.Method}->{context.Request.Path}->" +
                info);
            await context.Response.WriteAsync(error);
        }
    }
}