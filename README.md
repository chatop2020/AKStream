#  AKStream介绍 
## 技术交流QQ群：870526956   
## 注意:本项目的相关资料与Wiki内容都只在GitHub更新,不会在Gitee更新(但是最新代码会同步提交到GitHub和Gitee),有需要最新文档和资料的朋友请移步GitHub 
-------
# 感谢DartNoder提供免费的服务器给AKStream
# Thanks to DartNoder for providing free servers to AKStream. Dear friends, if you have efficient server needs, you can consider DartNoder
<a href='https://dartnode.com' _target="blank"><img src="https://i.ibb.co/5cPHFS5/WX20240121-210752-2x.png" width="158" height="49" /></a>
# 感谢jetbrains提供免费的开发者授权证书!
# Thanks to jetbrains for providing a free developer license
<img src="https://i.ibb.co/cbvPfgM/jb-beam.png" width="200" height="200" />


![akstream](https://i.loli.net/2021/08/05/5IgjLfCoS9e7NRm.png)
* AKStream是一套全功能的软NVR接口平台，软NVR指的是软件定义的NVR（Network Video Recoder），AKStream经过长达一年半的开发，测试与调优，已经具备了一定的使用价值，在可靠性，实用性方面都有着较为不错的表现，同时因为AKStream是一套完全开源的软件产品，在众多网友的一起加持下，AKStream的安全性也得到了验证。

* AKStream集成了ZLMediaKit作为其流媒体服务器，AKStream支持对ZLMediaKit的集群管理（通过AKStreamKeeper-流媒体治理组件），可以将分布在不同服务器的多个ZLMediaKit集群起来，统一管理，统一调度。
* 得益于ZLMediaKit流媒体服务器的强大，AKStream全面支持H265/H264/AAC/G711/OPUS等音视频编码格式，支持GB28181的Rtp推流、GB28181-PTZ控制、内置流代理器的http、rtps、rtmp拉流（支持H264,H265/ACC/G711）和ffmpeg流代理器的几乎所有形式的拉流（支持几乎所有格式及转码），将推拉流转换成RTSP/RTMP/HLS/HTTP-FLV/WebSocket-FLV/GB28181/HTTP-TS/WebSocket-TS/HTTP-fMP4/WebSocket-fMP4/MP4等几乎全协议的互相转换以供第三方（APP,WEB,客户端等）调用播放。
* AKStream支持linux、macos、Windows,系统可运行在可基于x86_64,ARM CPU架构下。
* 支持画面秒开、极低延时(500毫秒内，最低可达100毫秒)。
* 提供完善的标准Restful WebApi接口,供其他语言调用。
* AKStream的GB28181 Sip信令网关重新编写，不再使用StreamNode方案中的那个Sip网关，网关更加稳定可靠。目前仅支持GB28181-2016标准（由于没有其他版本协议的设备，没有做过详细测试），但由于新的Sip网关的高可扩展性，可以根据自己的需要进行功能扩展。
* AKStream使用.Net6框架，采用C#语言编写。
* 数据库部分使用开源项目freeSql数据库类库，支持数据库类型众多，如sqlite、mssql等，建议使用Mysql 5.7以及以上版本。
* AKStream将之前StreamNode的众多使用反馈做了集中处理与优化，使之更有适应性，可用性；比StreamNode在上体系更加完整，代码质量更高。
* 2020-2-5增加国内gitee同步仓库，方便国内下载
* AKStream是一个完善的接口平台,提供了几乎所有有关于NVR管理能力的API接口,有网友为AKStream实现了配套的UI,但仅仅只是Demo,是用来告诉你怎么调用AKStream相关接口,学习AKStream思想的一个工具,要真和自己业务相结合,需要自己实现前端UI和业务逻辑功能.
* 请多多支持，多多Star,谢谢


* AKStream is a full-featured soft NVR interface platform. Soft NVR refers to the software-defined NVR (Network Video Recoder). After a year and a half of development, testing and tuning, AKStream has already possessed a certain value in use. It has a relatively good performance in terms of reliability and practicability. At the same time, because AKStream is a completely open source software product, with the support of many netizens, the safety of AKStream has also been verified.

* AKStream integrates ZLMediaKit as its streaming media server. AKStream supports cluster management of ZLMediaKit (through the AKStreamKeeper-streaming media management component), which can cluster multiple ZLMediaKits distributed on different servers for unified management and unified scheduling.

* Thanks to the power of the ZLMediaKit streaming media server, AKStream fully supports H265/H264/AAC/G711/OPUS and other audio and video encoding formats, supports GB28181 Rtp streaming, GB28181-PTZ control, built-in streaming proxy http, rtps, rtmp Pull stream (support H264, H265/ACC/G711) and almost all forms of pull stream of ffmpeg streaming agent (support almost all formats and transcoding), convert push-pull stream into RTSP/RTMP/HLS/HTTP-FLV/WebSocket -FLV/GB28181/HTTP-TS/WebSocket-TS/HTTP-fMP4/WebSocket-fMP4/MP4 and other almost full protocol mutual conversion for the third party (APP, WEB, client, etc.) to call and play.

* AKStream supports Linux, macos, Windows, and the system can run on x86_64, ARM CPU architecture.

* Support the screen to open in seconds, very low delay (within 500 milliseconds, the lowest can reach 100 milliseconds).

* Provide a complete standard Restful WebApi interface for other languages to call.

* AKStream's GB28181(China GOV Video Rtp Stream Protocol Standard) Sip signaling gateway is rewritten, and the Sip gateway in the StreamNode solution is no longer used, and the gateway is more stable and reliable. Currently it only supports the GB28181-2016 standard (because there is no equipment with other versions of the protocol, no detailed testing has been done), but due to the high scalability of the new SIP gateway, it can be expanded according to your needs.

* AKStream uses the .Net6 framework and is written in C# language.

* The database part uses the open source project freeSql database library, which supports many types of databases, such as sqlite, mssql, etc. It is recommended to use Mysql 5.7 and above.

* AKStream has done centralized processing and optimization of many previous feedbacks of StreamNode to make it more adaptable and usable; it has a more complete system and higher code quality than StreamNode.

* 2020-2-5 Increase the domestic gitee synchronization warehouse to facilitate domestic downloads

* AKStream is a comprehensive API platform that offers nearly all API interfaces related to NVR management capabilities. Some enthusiasts have created a complementary UI for AKStream, but it's merely a demo intended to guide you on how to invoke AKStream APIs and serve as a tool for understanding the AKStream philosophy. To seamlessly integrate it with your own business, you'll need to develop the frontend UI and implement the business logic yourself.

* Please support a lot, star a lot, thank you

# 网友创建维护的Docker容器版本
- 包含了AKStreamWeb,AKStreamKeeper,ZLMediaKit,AKStreamWebNVR 开箱即用
- https://github.com/scottzhang/AKStreamDocker

# 强烈推荐AKStream生态圈中又一个开源Web管理平台(新作-基于React纯前端框架)
- 基于React的纯前端AKStream Web UI
- 非常简单的部署方式，非常简单的运行方式
- https://gitee.com/sscboshi/AKStreamNVR
- https://github.com/langmansh/AKStreamNVR

# 推荐为AKStream量身定制的开源Web管理平台（新作）
- 基于.net 5和vue2
- https://github.com/langmansh/AKStreamUI
- https://gitee.com/sscboshi/AKStreamUI

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

# 授权协议
------
本项目自有代码使用宽松的MIT协议，在保留版权信息的情况下可以自由应用于各自商用、非商业的项目。 但是本项目也零碎的使用了一些其他的开源代码，在商用的情况下请自行替代或剔除； 由于使用本项目而产生的商业纠纷或侵权行为一概与本项目及开发者无关，请自行承担法律风险。 在使用本项目代码时，也应该在授权协议中同时表明本项目依赖的第三方库的协议。
申明:本开源项目的代码,思想,结构,文档等相关内容不允许做为专利及其他知识产权申请,报备,注册等形式使用.
 

