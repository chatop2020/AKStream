# AKStream介绍 
## 技术交流QQ群：870526956
-------
* AKStream是一套全功能的软NVR接口平台，软NVR指的是软件定义的NVR（Network Video Recoder），AKStream经过长达一年半的开发，测试与调优，已经具备了一定的使用价值，在可靠性，实用性方面都有着较为不错的表现，同时因为AKStream是一套完全开源的软件产品，在众多网友的一起加持下，AKStream的安全性也得到了验证。

* AKStream集成了ZLMediaKit作为其流媒体服务器，AKStream支持对ZLMediaKit的集群管理（通过AKStreamKeeper-流媒体治理组件），可以将分布在不同服务器的多个ZLMediaKit集群起来，统一管理，统一调度。
* 得益于ZLMediaKit流媒体服务器的强大，AKStream全面支持H265/H264/AAC/G711/OPUS等音视频编码格式，支持GB28181的Rtp推流、GB28181-PTZ控制、内置流代理器的http、rtps、rtmp拉流（支持H264,H265/ACC/G711）和ffmpeg流代理器的几乎所有形式的拉流（支持几乎所有格式及转码），将推拉流转换成RTSP/RTMP/HLS/HTTP-FLV/WebSocket-FLV/GB28181/HTTP-TS/WebSocket-TS/HTTP-fMP4/WebSocket-fMP4/MP4等几乎全协议的互相转换以供第三方（APP,WEB,客户端等）调用播放。
* AKStream支持linux、macos、Windows,系统可运行在可基于x86_64,ARM CPU架构下。
* 支持画面秒开、极低延时(500毫秒内，最低可达100毫秒)。
* 提供完善的标准Restful WebApi接口,供其他语言调用。
* AKStream的GB28181 Sip信令网关重新编写，不再使用StreamNode方案中的那个Sip网关，网关更加稳定可靠。目前仅支持GB28181-2016标准（由于没有其他版本协议的设备，没有做过详细测试），但由于新的Sip网关的高可扩展性，可以根据自己的需要进行功能扩展。
* AKStream使用.net core 3.1框架（等后续.Net 5成熟后转到.Net 5框架），采用C#语言编写。
* 数据库部分使用开源项目freeSql数据库类库，支持数据库类型众多，如sqlite、mssql等，建议使用Mysql 5.7以及以上版本。
* AKStream将之前StreamNode的众多使用反馈做了集中处理与优化，使之更有适应性，可用性；比StreamNode在上体系更加完整，代码质量更高。
* 2020-2-5增加国内gitee同步仓库，方便国内下载
* 请多多支持，多多Start,谢谢
# 使用AKStream实现的Web管理平台（开源）
-------
![流媒体服务管理列表](https://i.loli.net/2021/01/19/YjCUMHVdFzSuBXe.png)
![在线设备列表](https://i.loli.net/2021/01/19/oUpFMBk5NELdYaT.png)
![设备激活](https://i.loli.net/2021/01/19/ZBMPfUnuLgVDelz.png)
![设备列表](https://i.loli.net/2021/01/19/OZDH1fP7j9TmIqS.png)
![设备预览](https://i.loli.net/2021/01/19/DcqgMmp6kTvNURK.png)

## 开源地址 
- https://gitee.com/sscboshi/AKStreamWebUI
- https://github.com/langmansh/AKStreamWebUI
- 感谢网友倾情打造


# AKStream结构
-------
* 放一张AKStream的完整结构图，以供理解AKStream的动作原理（采用StreamNode的图，原理是一样的）
* 具体详细说明请阅读项目 **WIKI（正在完善中...）**
![68747470733a2f2f692e6c6f6c692e6e65742f323032302f30392f32392f78776b6557386167597370484b55742e6a7067](https://i.loli.net/2021/01/06/RPEOJKbCcxkuViA.jpg)

