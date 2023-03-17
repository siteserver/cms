# SSCMS

<img src="https://sscms.com/docs/v7/logo.png" height="220" align="center">
<br /><br />

SSCMS 基于 .NET Core，能够以最低的成本、最少的人力投入在最短的时间内架设一个功能齐全、性能优异、规模庞大并易于维护的网站平台。

## 版本

项目发布的正式版本存放在 `master` 分支

| 编译状态                                                                                                                                                                                     | 版本号                                                     | 发布日期                                                                                   |
| -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------- | ------------------------------------------------------------------------------------------ |
| [![Build status](https://sscms.visualstudio.com/cms/_apis/build/status/siteserver.cms?branchName=master)](https://sscms.visualstudio.com/cms/_build/latest?definitionId=1&branchName=master) | ![Nuget version](https://img.shields.io/nuget/v/SSCMS.svg) | ![master last commit](https://img.shields.io/github/last-commit/siteserver/cms/master.svg) |

## 开发文档

[《SSCMS 使用指南》](https://sscms.com/docs/v7/getting-started/)

[《SSCMS 系统更新》](https://sscms.com/docs/v7/updates/)

[《SSCMS STL 语言》](https://sscms.com/docs/v7/stl/)

[《SSCMS 插件开发》](https://sscms.com/docs/v7/plugin/)

[《SSCMS 官方插件》](https://sscms.com/docs/v7/official/)

[《SSCMS 命令行》](https://sscms.com/docs/v7/cli/)

[《SSCMS REST API》](https://sscms.com/docs/v7/api/)

[《SSCMS 数据结构》](https://sscms.com/docs/v7/model/)

## SSCMS 源码结构

```code
│ sscms.sln                  Visual Studio 项目文件
│
├─docker                      Docker 配置文件
├─src/Datory                  数据库基础类
├─src/SSCMS                   接口、基础类
├─src/SSCMS.Cli               命令行工具
├─src/SSCMS.Core              CMS核心代码
├─src/SSCMS.Web               CMS App
└─tests                       测试
```

## 发布跨平台版本

### Window(x64)：

```
npm install
npm run build-win-x64
dotnet build ./build-win-x64/build.sln -c Release
dotnet publish ./build-win-x64/src/SSCMS.Cli/SSCMS.Cli.csproj -r win-x64 -c Release -o ./publish/sscms-win-x64
dotnet publish ./build-win-x64/src/SSCMS.Web/SSCMS.Web.csproj -r win-x64 -c Release -o ./publish/sscms-win-x64
npm run copy-win-x64
```

> Note: 进入文件夹 `./publish/sscms-win-x64` 获取最终发布版本

### Window(x32)：

```
npm install
npm run build-win-x32
dotnet build ./build-win-x32/build.sln -c Release
dotnet publish ./build-win-x32/src/SSCMS.Cli/SSCMS.Cli.csproj -r win-x32 -c Release -o ./publish/sscms-win-x32
dotnet publish ./build-win-x32/src/SSCMS.Web/SSCMS.Web.csproj -r win-x32 -c Release -o ./publish/sscms-win-x32
npm run copy-win-x32
```

> Note: 进入文件夹 `./publish/sscms-win-x32` 获取最终发布版本

### Linux(x64)：

```
npm install
npm run build-linux-x64
dotnet build ./build-linux-x64/build.sln -c Release
dotnet publish ./build-linux-x64/src/SSCMS.Cli/SSCMS.Cli.csproj -r linux-x64 -c Release -o ./publish/sscms-linux-x64
dotnet publish ./build-linux-x64/src/SSCMS.Web/SSCMS.Web.csproj -r linux-x64 -c Release -o ./publish/sscms-linux-x64
npm run copy-linux-x64
```

> Note: 进入文件夹 `./publish/sscms-linux-x64` 获取最终发布版本

### Linux(arm64)：

```
npm install
npm run build-linux-arm64
dotnet build ./build-linux-arm64/build.sln -c Release
dotnet publish ./build-linux-arm64/src/SSCMS.Cli/SSCMS.Cli.csproj -r linux-arm64 -c Release -o ./publish/sscms-linux-arm64
dotnet publish ./build-linux-arm64/src/SSCMS.Web/SSCMS.Web.csproj -r linux-arm64 -c Release -o ./publish/sscms-linux-arm64
npm run copy-linux-arm64
```

> Note: 进入文件夹 `./publish/sscms-linux-arm64` 获取最终发布版本

## 在 Docker 中运行

拉取最新版本的 SSCMS 镜像

```sh
docker pull sscms/core:latest
```

运行 SS CMS 容器

```sh
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

项目编译需要使用 Visual Studio 2019，你可以从这里下载 [Visual Studio Community 2019](https://www.visualstudio.com/downloads/)

代码贡献有很多形式，从提交问题，撰写文档，到提交代码，我们欢迎任何形式的贡献！

## 系统更新

SSCMS 产品将每隔两月发布新的正式版本，我们将在每次迭代中对核心功能、文档支持、功能插件以及网站模板四个方面进行持续改进。

## 问题与建议

如果发现任何 BUG 以及对产品使用的问题与建议，请提交至 [Github Issues](https://github.com/siteserver/cms/issues) 或者 [Gitee Issues](https://gitee.com/siteserver/cms/issues)。

## 关注最新动态

[![qrcode](https://sscms.com/assets/images/qrcode_for_wx.jpg)](https://sscms.com/)

## 特别声明

SSCMS 项目已加入 [dotNET China](https://gitee.com/dotnetchina)  组织。<br/>

![dotnetchina](https://gitee.com/dotnetchina/home/raw/master/assets/dotnetchina-raw.png "dotNET China LOGO")

## License

[GNU Affero General Public License v3.0](LICENSE)

Copyright (C) 2003-2023 SSCMS
