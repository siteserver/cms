# SSCMS

<div style="margin: 15px auto; text-align: center"><img src="https://sscms.com/docs/v7/logo.png" height="220" align="center"></div>

SSCMS 基于 .NET Core，能够以最低的成本、最少的人力投入在最短的时间内架设一个功能齐全、性能优异、规模庞大并易于维护的网站平台。

## 版本

项目发布的正式版本存放在 `master` 分支，最新版本存放在 `staging` 分支

| 编译状态                                                                                                                                                                                      | 版本号                                                         | 发布日期                                                                                     |
| --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| [![Build status](https://sscms.visualstudio.com/cms/_apis/build/status/siteserver.cms?branchName=master)](https://sscms.visualstudio.com/cms/_build/latest?definitionId=1&branchName=master)                           | ![Nuget version](https://img.shields.io/nuget/v/SSCMS.svg)    | ![master last commit](https://img.shields.io/github/last-commit/siteserver/cms/master.svg)   |

## 开发文档

[《STL 语言参考手册》](https://sscms.com/docs/stl/)

[《插件开发参考手册》](https://sscms.com/docs/plugins/)

[《CLI 命令行参考手册》](https://sscms.com/docs/cli/)

[《REST API 参考手册》](https://sscms.com/docs/api/)

[《数据结构参考手册》](https://sscms.com/docs/model/)

系统使用文档请点击 [SSCMS 文档中心](https://sscms.com/docs/)

## SSCMS 源码结构

```code
│ sscms.sln                  Visual Studio 项目文件
│
├─src/SS.CMS                   接口、基础类
├─src/SS.CMS.Cli               命令行工具
├─src/SS.CMS.Core              CMS核心代码
├─src/SS.CMS.Web               CMS App
└─tests                        测试
```

## 发布跨平台版本

### Window(64 位)：

```
npm install
npm run build-win-x64
dotnet build ./build-win-x64/build.sln -c Release
dotnet publish ./build-win-x64/src/SSCMS.Cli/SSCMS.Cli.csproj -r win-x64 -c Release -o ./publish/sscms-win-x64
dotnet publish ./build-win-x64/src/SSCMS.Web/SSCMS.Web.csproj -r win-x64 -c Release -o ./publish/sscms-win-x64
npm run copy-win-x64
```

> Note: 进入文件夹 `./publish/sscms-win-x64` 获取最终发布版本

### Window(32 位)：

```
npm install
npm run build-win-x32
dotnet build ./build-win-x32/build.sln -c Release
dotnet publish ./build-win-x32/src/SSCMS.Cli/SSCMS.Cli.csproj -r win-x32 -c Release -o ./publish/sscms-win-x32
dotnet publish ./build-win-x32/src/SSCMS.Web/SSCMS.Web.csproj -r win-x32 -c Release -o ./publish/sscms-win-x32
npm run copy-win-x32
```

> Note: 进入文件夹 `./publish/sscms-win-x32` 获取最终发布版本

### Linux：

```
npm install
npm run build-linux-x64
dotnet build ./build-linux-x64/build.sln -c Release
dotnet publish ./build-linux-x64/src/SSCMS.Cli/SSCMS.Cli.csproj -r linux-x64 -c Release -o ./publish/sscms-linux-x64
dotnet publish ./build-linux-x64/src/SSCMS.Web/SSCMS.Web.csproj -r linux-x64 -c Release -o ./publish/sscms-linux-x64
npm run copy-linux-x64
```

> Note: 进入文件夹 `./publish/sscms-linux-x64` 获取最终发布版本

### MacOS：

```
npm install
npm run build-osx-x64
dotnet build ./build-osx-x64/build.sln -c Release
dotnet publish ./build-osx-x64/src/SSCMS.Cli/SSCMS.Cli.csproj -r osx-x64 -c Release -o ./publish/sscms-osx-x64
dotnet publish ./build-osx-x64/src/SSCMS.Web/SSCMS.Web.csproj -r osx-x64 -c Release -o ./publish/sscms-osx-x64
npm run copy-osx-x64
```

> Note: 进入文件夹 `./publish/sscms-osx-x64` 获取最终发布版本

## 在 Docker 中运行

###  运行最新版本
```sh
docker pull sscms/core
docker run -d \
    --name my-sscms \
    -p 80:80 \
    --restart=always \
    -v volume-sscms:/app/wwwroot \
    -e SSCMS_SECURITY_KEY=e2a3d303-ac9b-41ff-9154-930710af0845 \
    -e SSCMS_DATABASE_TYPE=SQLite \
    sscms/core
```

## 贡献代码

代码贡献有很多形式，从提交问题，撰写文档，到提交代码，我们欢迎任何形式的贡献！

项目编译需要使用 Visual Studio 2019，你可以从这里下载 [Visual Studio Community 2019](https://www.visualstudio.com/downloads/)

- 1、Fork
- 2、创建您的特性分支 (`git checkout -b my-new-feature`)
- 3、提交您的改动 (`git commit -am 'Added some feature'`)
- 4、将您的修改记录提交到远程 `git` 仓库 (`git push origin my-new-feature`)
- 5、然后到 github 网站的该 `git` 远程仓库的 `my-new-feature` 分支下发起 Pull Request（请提交到 `dev` 分支，不要直接提交到 `master` 分支）

## 系统更新

SSCMS 产品将每隔两月发布新的正式版本，我们将在每次迭代中对核心功能、文档支持、功能插件以及网站模板四个方面进行持续改进。

## 问题与建议

如果发现任何 BUG 以及对产品使用的问题与建议，请提交至 [Github Issues](https://github.com/siteserver/cms/issues)。

## 关注最新动态

[![qrcode](https://sscms.com/assets/images/qrcode_for_wx.jpg)](https://sscms.com/)

## License

[GNU GENERAL PUBLIC LICENSE 3.0](LICENSE)

Copyright (C) 2003-2020 SSCMS
