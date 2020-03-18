# SiteServer CMS

SiteServer CMS 基于 .NET Core，能够以最低的成本、最少的人力投入在最短的时间内架设一个功能齐全、性能优异、规模庞大并易于维护的网站平台。

![SiteServer CMS](https://www.siteserver.cn/assets/images/github-banner.png)

## 版本

项目发布的正式版本存放在 `master` 分支，最新版本存放在 `staging` 分支

| 版本   | 编译状态                                                                                                                                                              | 版本号                                                         | 发布日期                                                                                     |
| ------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| 正式版 | [![Build status](https://ci.appveyor.com/api/projects/status/plx37i94y9gsqkru/branch/master?svg=true)](https://ci.appveyor.com/project/starlying/cms/branch/master)   | ![Nuget version](https://img.shields.io/nuget/v/SS.CMS.svg)    | ![master last commit](https://img.shields.io/github/last-commit/siteserver/cms/master.svg)   |
| 开发版 | [![Build Status](https://sscms.visualstudio.com/cms/_apis/build/status/siteserver.cms?branchName=staging)](https://sscms.visualstudio.com/cms/_build/latest?definitionId=1&branchName=master) | ![Nuget version](https://img.shields.io/nuget/vpre/SS.CMS.svg) | ![staging last commit](https://img.shields.io/github/last-commit/siteserver/cms/staging.svg) |

## 迭代计划

[2019 年 11 月/12 月迭代计划](https://mp.weixin.qq.com/s?__biz=MjM5MTE5MzgyNQ==&mid=2257483825&idx=1&sn=80a92e39b7d01afaeec6926566ff1e2e&chksm=a5c397bf92b41ea9a726de088aee1e602bafc4e06361efd46c7b567106a91b531c0ecd596782&scene=0&xtrack=1&key=43d0094527578369e5e37f70d4e27a77a18ce3dffd39c4b2d5d16cfc4083ebeb3cd3bcbdc499d98b6d7744f48a8b396b2445954c24a6fbfb5b8db18d6a29d2c7022c40aa0ccc5b54232aa4b3510b744b&ascene=1&uin=MTUyMjE4MTU2NQ%3D%3D&devicetype=Windows+10&version=62070158&lang=zh_CN&exportkey=AQHgYNDocdO2A1vX7NK%2B5mg%3D&pass_ticket=Hk04xxXsh%2FN%2BU1mefcyvspcxStKd0omKv%2FebbdAj8eqA62VeGDvOTXWVmJVNs2DE)

[2019 年 9 月/10 月迭代计划](https://mp.weixin.qq.com/s?__biz=MjM5MTE5MzgyNQ==&mid=2257483819&idx=1&sn=5c7872d787dbdc33c20ff07ef62825b3&chksm=a5c397a592b41eb3fa1fb63c81991fca25e8774ecb6aa38c5dde8ee332aa858062459cc7f074&scene=0&xtrack=1&key=79a78721542791212f32b13a1e4813e5de2132c8fffd9a98e2d0b6a8c3c529f38b975ccf4c071d642f8bdee97f4df145374556f6e63ec09ef361632dc37e2e24ee1b7f40dea9c688f947d76acf4a043c&ascene=1&uin=MTUyMjE4MTU2NQ%3D%3D&devicetype=Windows+10&version=62060833&lang=zh_CN&pass_ticket=zEXWDQP%2BAmijF6pKkhJsqtyuWssR%2BYFwJzTqiW0TnwgcoTUqMxJH1Ki%2F0Wdf%2FDKu)

[2019 年 7 月/8 月迭代计划](https://mp.weixin.qq.com/s/c-khP44sahCG1phjl8ZHeg)

[2019 年 5 月/6 月迭代计划](https://github.com/siteserver/cms/issues/1879)

[2019 年 3 月/4 月迭代计划](https://github.com/siteserver/cms/issues/1790)

[2019 年 1 月/2 月迭代计划](https://github.com/siteserver/cms/issues/1683)

[2018 年 11 月/12 月迭代计划](https://github.com/siteserver/cms/issues/1521)

[2018 年 9 月/10 月迭代计划](https://github.com/siteserver/cms/issues/1280)

[2018 年 8 月迭代计划](https://github.com/siteserver/cms/issues/1138)

[2018 年 7 月迭代计划](https://github.com/siteserver/cms/issues/956)

[2018 年 6 月迭代计划](https://github.com/siteserver/cms/issues/719)

[2018 年 5 月迭代计划](https://github.com/siteserver/cms/issues/518)

[2018 年 4 月迭代计划](https://github.com/siteserver/cms/issues/412)

[2018 年 3 月迭代计划](https://github.com/siteserver/cms/issues/300)

[2018 年 2 月迭代计划](https://github.com/siteserver/cms/issues/239)

## 开发文档

[《STL 语言参考手册》](https://www.siteserver.cn/docs/stl/)

[《插件开发参考手册》](https://www.siteserver.cn/docs/plugins/)

[《CLI 命令行参考手册》](https://www.siteserver.cn/docs/cli/)

[《REST API 参考手册》](https://www.siteserver.cn/docs/api/)

[《数据结构参考手册》](https://www.siteserver.cn/docs/model/)

系统使用文档请点击 [SiteServer CMS 文档中心](https://www.siteserver.cn/docs/)

## SiteServer CMS 源码结构

```code
│ sscms.sln                  Visual Studio 项目文件
│
├─src/SS.CMS.Abstractions      接口、基础类
├─src/SS.CMS                   CMS核心代码
├─src/SS.CMS.Cli               命令行工具
├─src/SS.CMS.Web               .NET CORE 控制器
└─tests                         测试库
```

## 发布跨平台版本

Window(64位)：
```
dotnet publish ./src/SSCMS/SSCMS.csproj -r win-x64 --configuration Release -o dist/sscms-win-x64 /p:PublishSingleFile=true
```

Window(32位)：
```
dotnet publish ./src/SSCMS/SSCMS.csproj -r win-x86 --configuration Release -o dist/sscms-win-x86 /p:PublishSingleFile=true
```

Linux：
```
dotnet publish ./src/SSCMS/SSCMS.csproj -r linux-x64 --configuration Release -o dist/sscms-linux-x64 /p:PublishSingleFile=true
```

MacOS：
```
dotnet publish ./src/SSCMS/SSCMS.csproj -r osx-x64 --configuration Release -o dist/sscms-osx-x64 /p:PublishSingleFile=true
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

SiteServer CMS 产品将每隔两月发布新的正式版本，我们将在每次迭代中对核心功能、文档支持、功能插件以及网站模板四个方面进行持续改进。

## 问题与建议

如果发现任何 BUG 以及对产品使用的问题与建议，请提交至 [Github Issues](https://github.com/siteserver/cms/issues)。

## 关注最新动态

[![qrcode](https://www.siteserver.cn/assets/images/qrcode_for_wx.jpg)](https://www.siteserver.cn/)

## License

[GNU GENERAL PUBLIC LICENSE 3.0](LICENSE)

Copyright (C) 2003-2020 SiteServer CMS
