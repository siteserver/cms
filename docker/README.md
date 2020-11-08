## 关于

SSCMS 官方镜像，跟随 SSCMS 版本同步更新。

## 获取 SSCMS 官方镜像

拉取最新版本的 [SS CMS 镜像](https://hub.docker.com/r/sscms/core)，运行命令：

``` bash
docker pull sscms/core:latest
```

如果需要获取指定版本的 [SS CMS 镜像](https://hub.docker.com/r/sscms/core)，可以运行命令：

``` bash
docker pull sscms/core:<版本号>
```

## 运行 SS CMS 容器

在当前文件夹下创建 `wwwroot` 目录：

```bash
mkdir wwwroot
```

接下来，我们使用 SQLite 本地数据库运行 SSCMS：

```bash
docker run -d \
    --name my-sscms \
    -p 80:80 \
    --restart=always \
    -v "$(pwd)"/wwwroot:/app/wwwroot \
    -e SSCMS_SECURITY_KEY=e2a3d303-ac9b-41ff-9154-930710af0845 \
    -e SSCMS_DATABASE_TYPE=SQLite \
    sscms/core:latest
```

- `-d` 参数让容器以后台任务形式运行
- `-name` 参数将容器实例命名为 my-sscms，可以更换为其他名称
- `-p` 参数映射容器的80端口到宿主机的80端口，如果希望使用8080端口访问可以设置 `-p 8080:80`
- `-restart` 参数使得容器能够自动重启，必须使用 `always` 选项，否则容器将无法安装及升级插件
- `-v` 参数将当前文件夹下的 `wwwroot` 目录作为网站跟目录，从而保存 SS CMS 站点数据，其中 `$(pwd)` 代表当前文件夹
- `-e` 参数设置容器运行环境变量，SS CMS 系统将读取环境变量，作为容器运行的参数，在此我们设置 `SecurityKey` 为随机的 GUID 值，数据库类型为 SQLite
- 最后我们将容器镜像设置为之前下载的 `sscms/core` 镜像

上面命令将运行 SSCMS 镜像，，接下来，可以通过 http://localhost 获取 http://host-ip 访问 SSCMS 系统了。

除了将当前文件夹下的 `wwwroot` 目录作为站点根目录存储数据，我们也可以将镜像数据持久化存储在 Volume 中：

```bash
docker run -d \
    --name my-sscms \
    -p 80:80 \
    --restart=always \
    -v volume-sscms:/app/wwwroot \
    -e SSCMS_SECURITY_KEY=e2a3d303-ac9b-41ff-9154-930710af0845 \
    -e SSCMS_DATABASE_TYPE=SQLite \
    sscms/core:latest
```

此命令将自动创建名称为 `volume-sscms` 的 Docker Volume。

## 环境变量配置

可以通过环境变量配置 SSCMS 运行参数：

**`SSCMS_SECURITY_KEY`** 必填项，SSCMS 客户端与服务器端加密通讯使用的秘钥，通常为 GUID 字符串

**`SSCMS_DATABASE_TYPE`** 必填项，SSCMS 使用的数据库类型，可以为以下取值中的一种：

- `MySQL` ： MySQL 数据库
- `SQLServer` ： Microsoft SQLServer 数据库
- `PostgreSQL` ： PostgreSQL 数据库
- `SQLite` ： SQLite 数据库

**`SSCMS_DATABASE_HOST`** 数据库主机地址

**`SSCMS_DATABASE_PORT`** 数据库访问端口

**`SSCMS_DATABASE_USER`** 数据库用户名

**`SSCMS_DATABASE_PASSWORD`** 数据库密码

**`SSCMS_DATABASE_NAME`** 数据库库名

**`SSCMS_DATABASE_CONNECTION_STRING`** 数据库连接字符串

**`SSCMS_REDIS_CONNECTION_STRING`** Redis 缓存连接字符串

如果 `SSCMS_DATABASE_TYPE` 设置为 **SQLite** 本地数据库，数据库将存储在 `wwwroot/sitefiles/database.sqlite` 文件中，如果 `SSCMS_DATABASE_TYPE` 设置为其他数据库类型，则还需要设置数据库环境变量。

数据库环境变量可以通过指定 `SSCMS_DATABASE_HOST`、`SSCMS_DATABASE_PORT`、`SSCMS_DATABASE_USER`、`SSCMS_DATABASE_PASSWORD` 以及 `SSCMS_DATABASE_NAME` 进行设置，也可以通过 `SSCMS_DATABASE_CONNECTION_STRING` 直接设置，两种方式选择其中一种。
