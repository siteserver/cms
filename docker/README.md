## 关于

SS CMS 官方 Docker Image

## 如何使用

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