一、插件、工具与上架流程

1.开发到上架流程

插件与工具：
①InstantGame：Unity中国官方插件，用于对微信小游戏的环境适配、性能优化
②微信小游戏转换工具：微信所出的Unity插件，用于将UnityWebGL项目转为微信小游戏项目
③微信开发者工具：微信小程序、小游戏项目的IDE
④UOS：Unity在线服务平台，本案例主要使用CDN（内容分发网络）功能来实现资源的按需加载

注意事项：
①.安装com.unity.instantgame，但并不是所有版本都可以安装，建议使用2021.2.5f1c302
②.不能使用Unity默认的字体显示中文 C:Windows/fonts中有系统字体
③.使用UOS，需要在微信小游戏-开发管理-开发设置中设置服务器域名白名单
④.纯广告盈利的话，不需要进行 网络游戏实名认证、软著申请等，在上架流程时依然会弹窗提醒，但流程上并不是必须的。
⑤.申请广告（流量主）需要累计1000独立访客，就是累计1000个用户访问过你的小游戏

地址：
①Unity InstantGame：https://unity.cn/instantgame/docs/WechatMinigame/InstallUnity/
②微信小游戏转换工具：https://github.com/wechat-miniprogram/minigame-unity-webgl-transform/tree/main
③UOS：https://uos.unity.cn/
④微信小游戏文档：https://developers.weixin.qq.com/minigame/dev/guide/
⑤微信小游戏提交审核指引：https://developers.weixin.qq.com/community/develop/doc/000e047c2486e8355396d42fe5bc01

2.正式开发

①首先新建一个项目之后，首先应该在Package Manager中清除掉项目中不需要的包，因为Unity新建项目时会默认安装一些包文件，将不需要的Remove，极致的减小包体大小。

②在UOS（Unity Online Servers）授权Unity ID账号，并创建一个线上服务器项目对应Unity项目。然后免费试用CDN服务。
在CDN服务器中首先创建一个Buckets（桶）用来存放需要上传到服务器的资源，并且Buckets可以创建分支来分离正式线上版本的服务器资源和测试版本的服务器资源文件。

③在创建了一个新的UOS线上服务器之后，在“设置”选项中会自动生成“APP ID”和“APP Secret”两个字段，即服务器ID和服务器密钥，通过这两个字段来输入到Instant Game AutoStreaming配置文件中将服务器和前端项目进行绑定。
在绑定正确的ID和密钥之后，点击“Refresh”后，系统就会自动获取你创建的桶和默认的最后桶分支（标识）。在绑定完成之后，首先应该新建一个桶的开发分支，即“Badge to Use”属性新建一个分支，推荐名称：“1.0”。

④随后设置微信小游戏转换插件，当在Auto Streaming插件中绑定好UOS线上CDN服务器时，微信小游戏转换插件会自动默认识别填写一些的字段。
先输入一些项目的基本信息，游戏AppID在微信公众平台中获取；游戏资源CDN插件自动识别；小游戏项目名称随意填写；游戏方向横屏还是竖屏等。

⑤由于我们是将WebGL项目转化为微信小程序，所以先将项目平台在Build Setting中转化为WebGL平台，同时将打包压缩方式（Texture Compression）切换为“ASTC”压缩格式，减小包体大小。
详细解读Instant插件的官方文档，它会有详细的性能优化指引，根据指引步骤可以最大程度优化性能。（重要！）
