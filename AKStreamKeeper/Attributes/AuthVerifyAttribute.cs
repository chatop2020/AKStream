using System;
using System.Net;
using LibCommon;
using LibLogger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AKStreamKeeper.Attributes
{
    /// <summary>
    /// 验证session和allowkey的类
    /// 暂时没有启用
    /// </summary>
    public class AuthVerifyAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 请求结束后
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        /// 请求进行中，判断用户session及allowkey的合法性
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Method.Trim().ToUpper().Equals("OPTIONS")) //为兼容ajax的options请求
            {
                var res = new EmptyResult();
                context.Result = res;
                return;
            }

            if (Common.IsDebug)
            {
                return;
            }

            if (string.IsNullOrEmpty(Common.AkStreamKeeperConfig.AccessKey))
            {
                return;
            }


            string accessKey = context.HttpContext.Request.Headers["AccessKey"];

            if (Common.AkStreamKeeperConfig.AccessKey.Trim().Equals(accessKey))
            {
                return;
            }

            ResponseStruct rs = new ResponseStruct()
            {
                Code = ErrorNumber.Sys_InvalidAccessKey,
                Message = ErrorMessage.ErrorDic![ErrorNumber.Sys_InvalidAccessKey],
            };
            string remoteIpAddr = context.HttpContext.Connection.RemoteIpAddress.ToString();
            GCommon.Logger.Error(
                $@"[{Common.LoggerHead}]->HTTP-AuthVerify    {remoteIpAddr}    {context.HttpContext.Request.Method}    {context.HttpContext.Request.Path} ->授权访问失败，访问密钥不正确");
            var result = new JsonResult(rs);
            result.StatusCode = (int) HttpStatusCode.BadRequest;
            context.Result = result;
        }
    }
}