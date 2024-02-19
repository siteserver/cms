# Datory 介绍

Datory 在流行的 [Dapper 框架](https://github.com/DapperLib/Dapper) 以及 [SqlKata 框架](https://github.com/sqlkata/querybuilder) 基础上开发而来，简化了操作接口并增加了默认设置，作为 SSCMS 默认的 ORM 数据库操作框架，Datory 能够以优雅的方式执行数据库查询以及执行操作。

Datory 版本号与 SSCMS 系统版本号一致，每次 SSCMS 系统更新时 Datory 版本也将随之更新，Datory 源码与 SSCMS 源码位于同一个[源码仓库](https://github.com/siteserver/cms/tree/master/src/Datory)，NuGet 托管地址：[https://www.nuget.org/packages/Datory](https://www.nuget.org/packages/Datory)。

Datory 框架使用参数绑定技术来保护 SSCMS 系统及其插件免受 SQL 注入攻击，无需清理作为绑定传递的字符串。

除了防止 SQL 注入攻击之外，Datory 框架还集成了 Redis 支持，通过让 SQL 引擎缓存和重用相同的查询计划来加快查询执行速度，同时 Datory 框架支持 MySQL、SQLServer、PostgreSql、达梦、人大金仓以及 Sqlite 等多种数据库类型。

示例：

```csharp
var repository = new Repository<DataModel>(settingsManager.Database, settingsManager.Redis);

await repository.GetAllAsync<string>(Q
    .Select("Name")
    .Where("GroupId", 100)
    .Limit(10)
    .OrderByDesc("Id")
);
```

以上代码首先从 `settingsManager` 中获取数据库链接信息并创建数据仓库 `repository`，仓库的数据源映射至 `DataModel` 实体类，然后设置 Where 查询条件，按 `Id` 字段倒序排序并限制最高获取10条数据，最后返回 `Name` 字段的列表值。

## 安装

SSCMS API 默认包含了 Datory 框架依赖，如果开发插件使用 Datory 框架，只需要安装 SSCMS 依赖包，无需单独安装 Datory 依赖包。

如果独立使用 Datory 框架，可以从终端运行以下命令以将其包含在您的项目中。

```
dotnet add package Datory
```
