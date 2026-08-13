#  AKStream介绍
## 技术交流QQ群：870526956(满员中,正在清理)
## 注意:本项目的相关资料与Wiki内容都只在GitHub更新,不会在Gitee更新(但是最新代码会同步提交到GitHub和Gitee),有需要最新文档和资料的朋友请移步GitHub

# 全新AKStream.Next 已经上线  官网地址 https://www.softnvr.com
## AKStream.next 免费授权永久离线运行，拥有AKStream的所有功能，更加支持了很多新特性（最新版的GB28181支持；Onvif设备的自动发现；RTC会议室等等新功能）
## AKStream.next 提供完整的二次开发接口对接能力，提供10万字的对接文档，助力二次开发顺利进行。
## AKStream.next 作为AKStream的商业项目，在继续秉持AKStream免费的基础上向非商业用途开放了免费授权，可离线激活，并永久内网使用。

-------
# 感谢DartNoder提供免费的服务器给AKStream
# Thanks to DartNoder for providing free servers to AKStream. Dear friends, if you have efficient server needs, you can consider DartNoder
<a href='https://dartnode.com' _target="blank"><img src="https://i.ibb.co/5cPHFS5/WX20240121-210752-2x.png" width="158" height="49" /></a>


![akstream](https://i.loli.net/2021/08/05/5IgjLfCoS9e7NRm.png)
* AKStream是一套全功能的软NVR接口平台，软NVR指的是软件定义的NVR（Network Video Recoder），AKStream经过长达一年半的开发，测试与调优，已经具备了一定的使用价值，在可靠性，实用性
方面都有着较为不错的表现，同时因为AKStream是一套完全开源的软件产品，在众多网友的一起加持下，AKStream的安全性也得到了验证。

* AKStream集成了ZLMediaKit作为其流媒体服务器，AKStream支持对ZLMediaKit的集群管理（通过AKStreamKeeper-流媒体治理组件），可以将分布在不同服务器的多个ZLMediaKit集群起来，统一管>>
理，统一调度。
* 得益于ZLMediaKit流媒体服务器的强大，AKStream全面支持H265/H264/AAC/G711/OPUS等音视频编码格式，支持GB28181的Rtp推流、GB28181-PTZ控制、内置流代理器的http、rtps、rtmp拉流（支持
H264,H265/ACC/G711）和ffmpeg流代理器的几乎所有形式的拉流（支持几乎所有格式及转码），将推拉流转换成RTSP/RTMP/HLS/HTTP-FLV/WebSocket-FLV/GB28181/HTTP-TS/WebSocket-TS/HTTP-fMP4//
WebSocket-fMP4/MP4等几乎全协议的互相转换以供第三方（APP,WEB,客户端等）调用播放。
* AKStream支持linux、macos、Windows,系统可运行在可基于x86_64,ARM CPU架构下。
* 支持画面秒开、极低延时(500毫秒内，最低可达100毫秒)。
* 提供完善的标准Restful WebApi接口,供其他语言调用。
* AKStream的GB28181 Sip信令网关重新编写，不再使用StreamNode方案中的那个Sip网关，网关更加稳定可靠。目前仅支持GB28181-2016标准（由于没有其他版本协议的设备，没有做过详细测试），
但由于新的Sip网关的高可扩展性，可以根据自己的需要进行功能扩展。
* AKStream使用.Net6框架，采用C#语言编写。
* 数据库部分使用开源项目freeSql数据库类库，支持数据库类型众多，如sqlite、mssql等，建议使用Mysql 5.7以及以上版本。
* AKStream将之前StreamNode的众多使用反馈做了集中处理与优化，使之更有适应性，可用性；比StreamNode在上体系更加完整，代码质量更高。
* 2020-2-5增加国内gitee同步仓库，方便国内下载
* AKStream是一个完善的接口平台,提供了几乎所有有关于NVR管理能力的API接口,有网友为AKStream实现了配套的UI,但仅仅只是Demo,是用来告诉你怎么调用AKStream相关接口,学习AKStream思想的>>
一个工具,要真和自己业务相结合,需要自己实现前端UI和业务逻辑功能.
* 请多多支持，多多Star,谢谢


# The new AKStream.Next has been launched. The official website address is https://www.softnvr.com

## AKStream.next is free and authorized to run offline permanently. It has all the functions of AKStream and supports many new features (the latest version of GB28181 support; automatic discovery of Onvif devices; RTC conference room and other new features)

## AKStream.next provides complete secondary development interface docking capabilities and provides 100,000-word docking documents to help secondary development proceed smoothly.

## AKStream.next As a commercial project of AKStream, it has opened up free authorization to non-commercial use on the basis of continuing to uphold AKStream's free status. It can be activated offline and used permanently on the intranet.

* AKStream is a full-featured soft NVR interface platform. Soft NVR refers to the software-defined NVR (Network Video Recoder). After a year and a half of development, testinn
g and tuning, AKStream has already possessed a certain value in use. It has a relatively good performance in terms of reliability and practicability. At the same time, becauss
e AKStream is a completely open source software product, with the support of many netizens, the safety of AKStream has also been verified.

* AKStream integrates ZLMediaKit as its streaming media server. AKStream supports cluster management of ZLMediaKit (through the AKStreamKeeper-streaming media management compp
onent), which can cluster multiple ZLMediaKits distributed on different servers for unified management and unified scheduling.

* Thanks to the power of the ZLMediaKit streaming media server, AKStream fully supports H265/H264/AAC/G711/OPUS and other audio and video encoding formats, supports GB28181 RR
tp streaming, GB28181-PTZ control, built-in streaming proxy http, rtps, rtmp Pull stream (support H264, H265/ACC/G711) and almost all forms of pull stream of ffmpeg streamingg
 agent (support almost all formats and transcoding), convert push-pull stream into RTSP/RTMP/HLS/HTTP-FLV/WebSocket -FLV/GB28181/HTTP-TS/WebSocket-TS/HTTP-fMP4/WebSocket-fMP44
/MP4 and other almost full protocol mutual conversion for the third party (APP, WEB, client, etc.) to call and play.

* AKStream supports Linux, macos, Windows, and the system can run on x86_64, ARM CPU architecture.

* Support the screen to open in seconds, very low delay (within 500 milliseconds, the lowest can reach 100 milliseconds).

* Provide a complete standard Restful WebApi interface for other languages to call.
