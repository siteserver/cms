# SiteServer CMS 用户中心代码

## Setup
安装依赖
```
npm install g3 -g
npm install
```
打包生成
```
rm -rf .g3 build
g3 build
```
运行服务器
```
g3 run
```
打开浏览器输入http://localhost:8801/

## Directory

```
│  .babelrc           babel配置
│  .gitignore         git忽略
│  index.html         入口页面
│  package.json
│  README.md
│
├─assets              资源文件
│
└─src                 TypeScript源文件
```