using System;
using System.Collections.Generic;

namespace LibCommon
{
    /// <summary>
    /// 错误代码
    /// </summary>
    [Serializable]
    public enum ErrorNumber : int
    {
        None = 0, //成功
        Sys_GetMacAddressExcept = -1000, //获取Mac地址异常
        Sys_GetIpAddressExcept = -1001, //获取IP地址异常
        Sys_JsonWriteExcept = -1002, //Json写入异常
        Sys_JsonReadExcept = -1003, //Json读取异常
        Sys_ConfigDirNotExists = -1004, //配置文件目录不存在
        Sys_ConfigFileNotExists = -1005, //配置文件不存在
        Sys_ParamsNotEnough = -1006, //参数不足
        Sys_ParamsIsNotRight = -1007, //参数不正确
        Sys_WebApi_Except = -1008, //WebApi异常
        Sys_ConfigNotReady = -1009, //配置文件没有就绪
        Sys_DataBaseNotReady = -1010, //数据库没有就绪
        Sys_NetworkPortExcept = -1011, //端口不可用
        Sys_DiskInfoExcept = -1012, //磁盘不可用
        Sys_UrlExcept = -1013, //参数中URL异常
        Sys_ReadIniFileExcept = -1014, //读取ini文件异常
        Sys_WriteIniFileExcept = -1015, //写入ini文件异常
        Sys_SocketPortForRtpExcept = -1016, //查找可用rtp端口时异常，可能已无可用端口
        Sys_SpecifiedFileNotExists = -1017, //指定文件不存在
        Sys_InvalidAccessKey = -1018, //访问密钥失效
        Sys_AKStreamKeeperNotRunning = -1019, //AKStreamKeeper流媒体服务器治理程序没有运行
        Sys_DataBaseLimited = -1020, //数据库操作受限，请检查相关参数，如分页查询时每页不能超过10000行
        Sys_DB_VideoChannelNotExists = -1021, //数据库中不存在指定音视频通道
        Sys_DataBaseExcept = -1022, //数据库执行异常
        Sys_DB_VideoChannelAlRedayExists = -1023, //数据库中已经存在指定音视频通道
        Sys_DB_RecordNotExists = -1024, //数据库中指定记录不存在
        Sys_VideoChannelNotActived = -1025, //音视频通实例没有激活
        Sys_HttpClientTimeout = -1026, //http客户端请求超时
        Sys_DB_RecordPlanNotExists = -1027, //录制计划不存在
        Sys_RecordPlanTimeLimitExcept = -1028, //录制计划时间间隔异常
        Sys_DB_RecordPlanAlreadyExists = -1029, //数据库中指定录制计划已经存在
        Sys_DvrCutMergeTimeLimit = -1030, //裁剪时间限制，超过120分钟任务不允许执行
        Sys_DvrCutMergeFileNotFound = -1031, //时间周期内没有找到相关视频文件
        Sys_DvrCutProcessQueueLimit = -1032, //处理队列已满，请稍后再试
        Sys_AutoResetEventExcept = -1033, //AutoResetEventExcept
        Sip_StartExcept = -2000, //启动Sip服务异常
        Sip_StopExcept = -2001, //停止Sip服务异常
        Sip_Except_DisposeSipDevice = -2002, //Sip网关内部异常(销毁Sip设备时)
        Sip_Except_RegisterSipDevice = -2003, //Sip网关内部异常(注册Sip设备时)
        Sip_ChannelNotExists = -2004, //Sip音视频通道不存在
        Sip_DeviceNotExists = -2005, //Sip设备不存在
        Sip_OperationNotAllowed = -2006, //该设备类型下不允许这个操作
        Sip_DeInviteExcept = -2007, //结束推流时异常
        Sip_InviteExcept = -2008, //推流时异常
        Sip_SendMessageExcept = -2009, //发送sip消息时异常
        Sip_AlredayPushStream = -2010, //sip通道已经在推流状态
        Sip_NotOnPushStream = -2011, //Sip通道没有在推流状态
        Sip_Channel_StatusExcept = -2012, //Sip通道设备状态异常
        Sip_VideoLiveExcept = -2013, //Sip通道推流请求异常
        Sip_CallBackExcept = -2014, //sip回调时异常
        Sip_SipClient_InitExcept=-2015,//sip客户端启动异常
        Sip_SipClient_ShareDeviceIdAlRedayExists=-2016,//SIP共享通道设备ID已经存在,ShareDeviceId不能重复
        MediaServer_WebApiExcept = -3000, //访问流媒体服务器WebApi时异常
        MediaServer_WebApiDataExcept = -3001, //访问流媒体服务器WebApi接口返回数据异常
        MediaServer_TimeExcept = -3002, //服务器时间异常，建议同步
        MediaServer_BinNotFound = -3003, //流媒体服务器可执行文件不存在
        MediaServer_ConfigNotFound = -3004, //流媒体服务器配置文件不存在，建议手工运行一次流媒体服务器使其自动生成配置文件模板
        MediaServer_InstanceIsNull = -3005, //流媒体服务实例为空，请先创建流媒体服务实例
        MediaServer_StartUpExcept = -3006, //启动流媒体服务器失败
        MediaServer_ShutdownExcept = -3007, //停止流媒体服务器失败
        MediaServer_RestartExcept = -3008, //重启流媒体服务器失败
        MediaServer_ReloadExcept = -3009, //流媒体服务器配置热加载失败
        MediaServer_NotRunning = -3010, //流媒体服务器没有运行
        MediaServer_OpenRtpPortExcept = -3011, //申请rtp端口失败，申请端口可能已经存在
        MediaServer_WaitWebHookTimeOut = -3012, //等待流媒体服务器回调时超时
        MediaServer_StreamTypeExcept = -3013, //流类型不正确
        MediaServer_GetStreamTypeExcept = -3014, //指定拉流方法不正确
        MediaServer_VideoSrcExcept = -3015, //源流地址异常
        MediaServer_InputObjectAlredayExists=-3016,//传入对象已经存在
        MediaServer_ObjectNotExists=-3017,//对象不存在
        

        Other = -6000 //其他异常
    }

    /// <summary>
    /// 错误代码描述
    /// </summary>
    [Serializable]
    public static class ErrorMessage
    {
        public static Dictionary<ErrorNumber, string>? ErrorDic;

        public static void Init()
        {
            ErrorDic = new Dictionary<ErrorNumber, string>();
            ErrorDic[ErrorNumber.None] = "无错误";
            ErrorDic[ErrorNumber.Sys_GetMacAddressExcept] = "获取Mac地址异常";
            ErrorDic[ErrorNumber.Sys_GetIpAddressExcept] = "获取IP地址异常";
            ErrorDic[ErrorNumber.Sys_JsonWriteExcept] = "Json写入异常";
            ErrorDic[ErrorNumber.Sys_JsonReadExcept] = "Json读取异常";
            ErrorDic[ErrorNumber.Sys_ConfigDirNotExists] = "配置文件目录不存在";
            ErrorDic[ErrorNumber.Sys_ConfigFileNotExists] = "配置文件不存在";
            ErrorDic[ErrorNumber.Sys_ParamsNotEnough] = "传入参数不足";
            ErrorDic[ErrorNumber.Sys_ParamsIsNotRight] = "传入参数不正确";
            ErrorDic[ErrorNumber.Sys_ConfigNotReady] = "配置文件没有就绪,请检查配置文件是否正确无误";
            ErrorDic[ErrorNumber.Sys_DataBaseNotReady] = "数据库没有就绪，请检查数据库是否可以正常连接";
            ErrorDic[ErrorNumber.Sys_NetworkPortExcept] = "端口不可用，请检查配置文件";
            ErrorDic[ErrorNumber.Sys_WebApi_Except] = "WebApi异常";
            ErrorDic[ErrorNumber.Sys_DiskInfoExcept] = "磁盘不可用，请检查配置文件";
            ErrorDic[ErrorNumber.Sys_UrlExcept] = "参数中URL异常";
            ErrorDic[ErrorNumber.Sys_ReadIniFileExcept] = "读取ini文件异常";
            ErrorDic[ErrorNumber.Sys_WriteIniFileExcept] = "写入ini文件异常";
            ErrorDic[ErrorNumber.Sys_SocketPortForRtpExcept] = "查找可用rtp端口时异常，可能已无可用端口";
            ErrorDic[ErrorNumber.Sys_SpecifiedFileNotExists] = "指定文件不存在";
            ErrorDic[ErrorNumber.Sys_InvalidAccessKey] = "访问密钥失效";
            ErrorDic[ErrorNumber.Sys_AKStreamKeeperNotRunning] = "AKStreamKeeper流媒体服务器治理程序没有运行";
            ErrorDic[ErrorNumber.Sys_DataBaseLimited] = "数据库操作受限，请检查相关参数，如分页查询时每页不能超过10000行,第一页从1开始而不是从0开始";
            ErrorDic[ErrorNumber.Sys_DB_VideoChannelNotExists] = "数据库中不存在指定音视频通道";
            ErrorDic[ErrorNumber.Sys_DB_VideoChannelAlRedayExists] = "数据库中已经存在指定音视频通道";
            ErrorDic[ErrorNumber.Sys_DB_RecordNotExists] = "数据库中指定记录不存在";
            ErrorDic[ErrorNumber.Sys_DB_RecordPlanNotExists] = "数据库中指定录制计划不存在";
            ErrorDic[ErrorNumber.Sys_RecordPlanTimeLimitExcept] = "录制计划中时间间隔太小异常";
            ErrorDic[ErrorNumber.Sys_DataBaseExcept] = "数据库执行异常";
            ErrorDic[ErrorNumber.Sys_VideoChannelNotActived] = "音视频通道实例没有激活,或音视频通道数据库配置有异常，请激活并检查";
            ErrorDic[ErrorNumber.Sys_HttpClientTimeout] = "http客户端请求超时或服务不可达";
            ErrorDic[ErrorNumber.Sys_DB_RecordPlanAlreadyExists] = "数据库中指定录制计划已经存在";
            ErrorDic[ErrorNumber.Sys_DvrCutMergeTimeLimit] = "裁剪时间限制，超过120分钟任务不允许执行";
            ErrorDic[ErrorNumber.Sys_DvrCutMergeFileNotFound] = "时间周期内没有找到相关视频文件";
            ErrorDic[ErrorNumber.Sys_DvrCutProcessQueueLimit] = "处理队列已满，请稍后再试";
            ErrorDic[ErrorNumber.Sys_AutoResetEventExcept] = "异步等待元子异常";
            ErrorDic[ErrorNumber.Sip_StartExcept] = "启动Sip服务异常";
            ErrorDic[ErrorNumber.Sip_StopExcept] = "停止Sip服务异常";
            ErrorDic[ErrorNumber.Sip_Except_DisposeSipDevice] = "Sip网关内部异常(销毁Sip设备时)";
            ErrorDic[ErrorNumber.Sip_Except_RegisterSipDevice] = "Sip网关内部异常(注册Sip设备时)";
            ErrorDic[ErrorNumber.Sip_ChannelNotExists] = "Sip音视频通道不存在";
            ErrorDic[ErrorNumber.Sip_DeviceNotExists] = "Sip设备不存在";
            ErrorDic[ErrorNumber.Sip_OperationNotAllowed] = "该类型的设备不允许做这个操作";
            ErrorDic[ErrorNumber.Sip_DeInviteExcept] = "结束推流时发生异常";
            ErrorDic[ErrorNumber.Sip_InviteExcept] = "推流时发生异常";
            ErrorDic[ErrorNumber.Sip_SendMessageExcept] = "发送Sip消息时异常";
            ErrorDic[ErrorNumber.Sip_AlredayPushStream] = "Sip通道(回放录像)已经在推流状态";
            ErrorDic[ErrorNumber.Sip_NotOnPushStream] = "Sip通道(回放录像)没有在推流状态";
            ErrorDic[ErrorNumber.Sip_Channel_StatusExcept] = "Sip通道状态异常";
            ErrorDic[ErrorNumber.Sip_VideoLiveExcept] = "Sip通道推流请求异常";
            ErrorDic[ErrorNumber.Sip_CallBackExcept] = "sip回调时异常";
            ErrorDic[ErrorNumber.Sip_SipClient_InitExcept] = "sip客户端启动异常";
            ErrorDic[ErrorNumber.Sip_SipClient_ShareDeviceIdAlRedayExists] = "sip共享通道设备ID已经存在";
            ErrorDic[ErrorNumber.MediaServer_WebApiExcept] = "访问流媒体服务器WebApi接口时异常";
            ErrorDic[ErrorNumber.MediaServer_WebApiDataExcept] = "访问流媒体服务器WebApi接口返回数据异常";
            ErrorDic[ErrorNumber.MediaServer_TimeExcept] = "流媒体服务器时间异常，建议同步";
            ErrorDic[ErrorNumber.MediaServer_BinNotFound] = "流媒体服务器可执行文件不存在";
            ErrorDic[ErrorNumber.MediaServer_ConfigNotFound] = "流媒体服务器配置文件不存在，建议手工运行一次流媒体服务器使其自动生成配置文件模板";
            ErrorDic[ErrorNumber.MediaServer_InstanceIsNull] = "流媒体服务实例为空，请先创建流媒体服务实例";
            ErrorDic[ErrorNumber.MediaServer_StartUpExcept] = "启动流媒体服务器失败";
            ErrorDic[ErrorNumber.MediaServer_ShutdownExcept] = "停止流媒体服务器失败";
            ErrorDic[ErrorNumber.MediaServer_RestartExcept] = "重启流媒体服务器失败";
            ErrorDic[ErrorNumber.MediaServer_ReloadExcept] = "流媒体服务器配置热加载失败";
            ErrorDic[ErrorNumber.MediaServer_NotRunning] = "流媒体服务器没有运行";
            ErrorDic[ErrorNumber.MediaServer_OpenRtpPortExcept] = "申请rtp端口失败，申请端口可能已经存在";
            ErrorDic[ErrorNumber.MediaServer_WaitWebHookTimeOut] = "等待流媒体服务器回调响应超时";
            ErrorDic[ErrorNumber.MediaServer_StreamTypeExcept] = "流类型不正确，GB28181Rtp流不能使用此功能拉流";
            ErrorDic[ErrorNumber.MediaServer_GetStreamTypeExcept] = "指定拉流方法不正确，请指定SelfMethod后再试";
            ErrorDic[ErrorNumber.MediaServer_VideoSrcExcept] = "源流地址异常，请检查数据库中VideoSrcUrl字段是否正确";
            ErrorDic[ErrorNumber.MediaServer_InputObjectAlredayExists] = "传入对象已经存在";
            ErrorDic[ErrorNumber.MediaServer_ObjectNotExists] = "指定对象不存在";
            ErrorDic[ErrorNumber.Other] = "未知错误";
        }
    }
}