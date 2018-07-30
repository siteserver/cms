# SiteServer CMS

SiteServer CMS 基于.NET平台，能够以最低的成本、最少的人力投入在最短的时间内架设一个功能齐全、性能优异、规模庞大并易于维护的网站平台。

![SiteServer CMS](https://www.siteserver.cn/assets/github-banner.png)


## 版本

项目发布的正式版本存放在 `master` 分支，当前的开发版本存放在 `dev` 分支

版本  | 编译状态 | 版本号 | 发布日期
------  | ------ | ------ | ------
正式版 | [![Build status](https://ci.appveyor.com/api/projects/status/plx37i94y9gsqkru/branch/master?svg=true)](https://ci.appveyor.com/project/starlying/cms/branch/master) | ![Nuget version](https://img.shields.io/nuget/v/SS.CMS.svg) | ![master last commit](https://img.shields.io/github/last-commit/siteserver/cms/master.svg)
开发版 | [![Build status](https://ci.appveyor.com/api/projects/status/plx37i94y9gsqkru/branch/dev?svg=true)](https://ci.appveyor.com/project/starlying/cms/branch/dev) | ![Myget version](https://img.shields.io/myget/siteserver/v/SS.CMS.svg) | ![dev last commit](https://img.shields.io/github/last-commit/siteserver/cms/dev.svg)

## 迭代计划

[2018年7月迭代计划](https://github.com/siteserver/cms/issues/956)

[2018年6月迭代计划](https://github.com/siteserver/cms/issues/719)

[2018年5月迭代计划](https://github.com/siteserver/cms/issues/518)

[2018年4月迭代计划](https://github.com/siteserver/cms/issues/412)

[2018年3月迭代计划](https://github.com/siteserver/cms/issues/300)

[2018年2月迭代计划](https://github.com/siteserver/cms/issues/239)

[SiteServer CMS 路线图 2018](https://github.com/siteserver/cms/issues/718)

## 开发文档

[《插件开发参考手册》](https://docs.siteserver.cn/plugins)

[《STL 语言参考手册》](https://stl.siteserver.cn)

[《CLI 命令行参考手册》](https://docs.siteserver.cn/cli)

[《REST API 参考手册》](https://docs.siteserver.cn/api)

[《数据结构参考手册》](https://docs.siteserver.cn/model)

系统使用文档请点击 [SiteServer CMS 文档中心](https://docs.siteserver.cn)

## SiteServer CMS 源码结构

```code
│ siteserver.sln                  Visual Studio 项目文件
│
├─SiteServer.BackgroundPages      ASP.NET 页面源文件
├─SiteServer.Cli                  命令行工具
├─SiteServer.CMS                  CMS 源文件
├─SiteServer.Utils                基础类库
└─SiteServer.Web                  API 源文件及aspx页面
```

## 生成安装包

```code
一、Visual Studio 切换解决方案配置到Release，编译
二、安装NodeJs
三、打开命令行，运行 npm install gulp -g
四、命令行，转到根目录，运行 npm install
五、命令行，运行 gulp build
六、命令行，运行 gulp zip
```

结束后会在根目录看到siteserver_install.zip，这就是安装包了。
以上步骤是第一次生成安装包所需要执行的操作，如果已经生成过安装包：

```code
一、命令行，转到根目录，运行 gulp build
二、命令行，运行 gulp zip
```

## 贡献代码

代码贡献有很多形式，从提交问题，撰写文档，到提交代码，我们欢迎任何形式的贡献！

项目编译需要使用 Visual Studio 2017，你可以从这里下载 [Visual Studio Community 2017](https://www.visualstudio.com/downloads/)

- 1、Fork
- 2、创建您的特性分支 (`git checkout -b my-new-feature`)
- 3、提交您的改动 (`git commit -am 'Added some feature'`)
- 4、将您的修改记录提交到远程 `git` 仓库 (`git push origin my-new-feature`)
- 5、然后到 github 网站的该 `git` 远程仓库的 `my-new-feature` 分支下发起 Pull Request（请提交到 `dev` 分支，不要直接提交到 `master` 分支）

## 系统更新

SiteServer CMS 产品将在每个月底发布新的正式版本，我们将在每次迭代中对核心功能、文档支持、功能插件以及网站模板四个方面进行持续改进。

## 问题与建议

我们的目标是系统零BUG，如果发现任何BUG请提交至[Github Issues](https://github.com/siteserver/cms/issues)。

## 关注最新动态

[![qrcode](https://www.siteserver.cn/images/qrcode_for_wx.jpg)](https://www.siteserver.cn/)

## License

[GNU GENERAL PUBLIC LICENSE 3.0](LICENSE)

Copyright (C) 2003-2018 SiteServer CMS