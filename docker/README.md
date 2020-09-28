## 关于

SSCMS 官方镜像，跟随 SSCMS 版本同步更新。

## 如何使用

获取最新版本 SSCMS 官方镜像：

```bash
docker pull sscms/core:latest
```

使用 SQLite 本地数据库运行 SSCMS：

```bash
docker run -d \
    --name my-sscms \
    -p 80:80 \
    --restart=always \
    -v volume-sscms:/app/wwwroot \
    -e SSCMS_SECURITY_KEY=e2a3d303-ac9b-41ff-9154-930710af0845 \
    -e SSCMS_DATABASE_TYPE=SQLite \
    sscms/core
```

- `-d` 参数让容器以后台任务形式运行
- `-name` 参数将容器实例命名为 my-sscms
- `-p` 参数映射容器的80端口到宿主机的80端口，如果希望使用8080端口访问可以设置 `-p 8080:80`
- `-restart` 参数使得容器能够自动重启，必须使用 `always` 选项，否则容器将无法安装及升级插件
- `-v` 参数让容器使用 Volume 作为持久化存储，从而保存 SS CMS 系统数据
- `-e` 参数设置容器运行环境变量，SS CMS 系统将读取环境变量，作为容器运行的参数，在此我们设置 `SecurityKey` 为随机的 GUID 值，数据库类型为 SQLite
- 最后我们将容器镜像设置为之前下载的 `sscms/core` 镜像

上面命令将运行 SSCMS 镜像，镜像数据持久化存储在 Volume 中，接下来，可以通过 http://localhost 获取 http://host-ip 访问 SSCMS 系统了。

## 环境变量配置

可以通过环境变量配置 SSCMS 运行参数：

**`SSCMS_SECURITY_KEY`** 必填项，SSCMS 客户端与服务器端加密通讯使用的秘钥，通常为 GUID 字符串

**`SSCMS_DATABASE_TYPE`** 必填项，SSCMS 使用的数据库类型，可以为以下取值中的一种：

- `MySQL` ： MySQL 数据库
- `SQLServer` ： Microsoft SQLServer 数据库
- `PostgreSQL` ： PostgreSQL 数据库
- `SQLite` ： SQLite 数据库

**`DATABASE_HOST`** 数据库主机地址

**`DATABASE_PORT`** 数据库访问端口

**`DATABASE_USER`** 数据库用户名

**`DATABASE_PASSWORD`** 数据库密码

**`DATABASE_NAME`** 数据库库名

**`DATABASE_CONNECTION_STRING`** 数据库连接字符串

**`REDIS_CONNECTION_STRING`** Redis 缓存连接字符串

如果 `SSCMS_DATABASE_TYPE` 设置为 **SQLite** 本地数据库，数据库将存储在 `wwwroot/sitefiles/database.sqlite` 文件中，如果 `SSCMS_DATABASE_TYPE` 设置为其他数据库类型，则还需要设置数据库环境变量。

数据库环境变量可以通过指定 `DATABASE_HOST`、`DATABASE_PORT`、`DATABASE_USER`、`DATABASE_PASSWORD` 以及 `DATABASE_NAME` 进行设置，也可以通过 `DATABASE_CONNECTION_STRING` 直接设置，两种方式选择其中一种。

