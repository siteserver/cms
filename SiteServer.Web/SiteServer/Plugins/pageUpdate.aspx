<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageUpdate" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form id="main" class="m-l-15 m-r-15" runat="server">

        <div class="card-box">
          <h4 class="header-title m-t-0">
            插件升级向导
          </h4>
          <p class="text-muted m-b-25 font-13">
            欢迎来到插件升级向导！
          </p>

          <ctrl:alerts runat="server" />

          <ul class="nav nav-pills nav-fill bg-muted m-b-20">
            <li class="nav-item" v-bind:class="{ active: step == 1 }">
              <a class="nav-link" href="javascript:;">检查更新</a>
            </li>
            <li class="nav-item" v-bind:class="{ active: step == 2 }">
              <a class="nav-link" href="javascript:;">下载升级包</a>
            </li>
            <li class="nav-item" v-bind:class="{ active: step == 3 }">
              <a class="nav-link" href="javascript:;">升级插件</a>
            </li>
            <li class="nav-item" v-bind:class="{ active: step == 4 }">
              <a class="nav-link" href="javascript:;">升级完成</a>
            </li>
          </ul>

          <div class="alert alert-danger" v-bind:style="{ display: errorMessage ? '' : 'none' }" style="display: none">
            {{ errorMessage }}
          </div>

          <!-- step 1 place -->
          <div v-bind:style="{ display: step == 1 ? '' : 'none' }">

            <div class="panel panel-border panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title">检查更新</h3>
                <p class="panel-sub-title font-13 text-muted">检查插件新版本</p>
              </div>
              <div class="panel-body">

                <div v-bind:style="{ display: !isGetVersions ? '' : 'none' }" class="jumbotron text-center">
                  <img src="../pic/animated_loading.gif" />
                  <br />
                  <br />
                  <p class="lead">正在检查插件更新，请稍后...</p>
                </div>

                <div v-bind:style="{ display: isGetVersions && packages.length == 0 ? '' : 'none' }" class="jumbotron" style="display: none">
                  <h4 class="display-5">未发现插件新版本</h4>
                </div>

                <div v-bind:style="{ display: isGetVersions && packages.length > 0 ? '' : 'none' }" class="table-responsive" style="display: none">

                  <div class="alert alert-success">
                    发现以下插件发布了新版本，请选中需要更新的插件后点击下一步开始升级
                  </div>

                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-center checkbox checkbox-primary text-nowrap">
                          <input type="checkbox" id="all" value="all" @change='checkAll()' />
                          <label for="all">全选</label>
                        </th>
                        <th class="text-nowrap">插件Id</th>
                        <th class="text-nowrap">插件名称</th>
                        <th class="text-nowrap">已安装版本</th>
                        <th class="text-nowrap">新版本</th>
                        <th>更新说明</th>
                        <th class="text-center text-nowrap">发布时间</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="package in packages">
                        <td class="text-center checkbox checkbox-primary text-nowrap">
                          <input type="checkbox" v-bind:id="package.id" v-bind:value="package.id" v-model="checkedIds" />
                          <label v-bind:for="package.id">选中</label>
                        </td>
                        <td class="text-nowrap">
                          {{ package.id }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.title }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.installedVersion }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.version }}
                        </td>
                        <td>
                          {{ package.releaseNotes }}
                        </td>
                        <td class="text-center text-nowrap">
                          {{ package.published }}
                        </td>
                      </tr>
                    </tbody>
                  </table>

                  <hr />

                  <div class="text-center">
                    <input class="btn" @click="download" v-bind:disabled="(checkedIds.length == 0)" v-bind:class="{ 'btn-primary': checkedIds.length > 0 }"
                      type="button" value="下一步">
                  </div>

                </div>
              </div>
            </div>

          </div>

          <!-- step 2 place -->
          <div v-bind:style="{ display: step == 2 ? '' : 'none' }">

            <div class="panel panel-border panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title">下载升级包</h3>
                <p class="panel-sub-title font-13 text-muted">系统正在下载插件升级包，可能需要几分钟，请稍后...</p>
              </div>
              <div class="panel-body">

                <div class="table-responsive">

                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-center text-nowrap">状态</th>
                        <th class="text-nowrap">插件Id</th>
                        <th class="text-nowrap">插件名称</th>
                        <th class="text-nowrap">已安装版本</th>
                        <th class="text-nowrap">新版本</th>
                        <th>更新说明</th>
                        <th class="text-center text-nowrap">发布时间</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="package in packages" v-bind:style="{ display: checkedIds.indexOf(package.id) !== -1 ? '' : 'none' }">
                        <td class="text-center text-nowrap font-13 text-muted">
                          <div class="text-success" v-bind:style="{ display: downloadIds.indexOf(package.id) !== -1 && downloadingId != package.id ? '' : 'none' }">
                            下载完成
                          </div>
                          <div v-bind:style="{ display:downloadingId == package.id ? '' : 'none' }">
                            <img src="../pic/animated_loading.gif" />下载中...
                          </div>
                          <div v-bind:style="{ display: downloadIds.indexOf(package.id) == -1 && downloadingId != package.id ? '' : 'none' }">
                            等待下载
                          </div>
                        </td>
                        <td class="text-nowrap">
                          {{ package.id }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.title }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.installedVersion }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.version }}
                        </td>
                        <td>
                          {{ package.releaseNotes }}
                        </td>
                        <td class="text-center text-nowrap">
                          {{ package.published }}
                        </td>
                      </tr>

                    </tbody>
                  </table>

                </div>
              </div>
            </div>

          </div>

          <!-- step 3 place -->
          <div v-bind:style="{ display: step == 3 ? '' : 'none' }">

            <div class="panel panel-border panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title">升级插件</h3>
                <p class="panel-sub-title font-13 text-muted">系统正在升级插件，请稍后...</p>
              </div>
              <div class="panel-body">

                <div class="table-responsive">

                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-center text-nowrap">状态</th>
                        <th class="text-nowrap">插件Id</th>
                        <th class="text-nowrap">插件名称</th>
                        <th class="text-nowrap">已安装版本</th>
                        <th class="text-nowrap">新版本</th>
                        <th>更新说明</th>
                        <th class="text-center text-nowrap">发布时间</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="package in packages" v-bind:style="{ display: checkedIds.indexOf(package.id) !== -1 ? '' : 'none' }">
                        <td class="text-center text-nowrap font-13 text-muted">
                          <div class="text-success" v-bind:style="{ display: updatedIds.indexOf(package.id) !== -1 && updatingId != package.id ? '' : 'none' }">
                            升级完成
                          </div>
                          <div v-bind:style="{ display:updatingId == package.id ? '' : 'none' }">
                            <img src="../pic/animated_loading.gif" />升级中...
                          </div>
                          <div v-bind:style="{ display: updatedIds.indexOf(package.id) == -1 && updatingId != package.id ? '' : 'none' }">
                            等待升级
                          </div>
                        </td>
                        <td class="text-nowrap">
                          {{ package.id }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.title }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.installedVersion }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.version }}
                        </td>
                        <td>
                          {{ package.releaseNotes }}
                        </td>
                        <td class="text-center text-nowrap">
                          {{ package.published }}
                        </td>
                      </tr>

                    </tbody>
                  </table>

                </div>
              </div>
            </div>

          </div>

          <!-- step 4 place -->
          <div v-bind:style="{ display: step == 4 ? '' : 'none' }">

            <div class="alert alert-success" role="alert">
              <h4 class="alert-heading">升级完成！</h4>
              <p>
                恭喜，您已经完成了插件的升级，页面将在3秒之后重新载入...
              </p>
            </div>

          </div>

        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->
    <script src="../assets/vue/vue.min.js"></script>
    <script src="../assets/js/apiUtils.js"></script>
    <script src="../assets/js/compareversion.js"></script>
    <script type="text/javascript">
      var versionApi = new apiUtils.Api();
      var downloadApi = new apiUtils.Api('<%=DownloadApiUrl%>');
      var updateApi = new apiUtils.Api('<%=UpdateApiUrl%>');
      var clearCacheApi = new apiUtils.Api('<%=ClearCacheApiUrl%>');
      var isNightly = <%=IsNightly%>;
      var version = '<%=Version%>';

      var data = {
        step: 1,
        isGetVersions: false,
        packages: <%=Packages%>,
        packageIds: '<%=PackageIds%>',
        checkedIds: [],
        isCheckAll: false,
        downloadingId: 0,
        downloadIds: [],
        updatingId: 0,
        updatedIds: [],
        errorMessage: null
      };

      var $vue = new Vue({
        el: '#main',
        data: data,
        methods: {
          version: function () {
            var $this = this;

            versionApi.get({
              isNightly: isNightly,
              version: version,
              $filter: "id in '" + this.packageIds + "'"
            }, function (err, res) {
              if (!err && res) {
                var packages = [];
                for (var i = 0; i < res.length; i++) {
                  var package = res[i];
                  var installedPackage = $.grep($this.packages, function (e) {
                    return e.id == package.id;
                  });
                  if (installedPackage.length == 1) {
                    package.installedVersion = installedPackage[0].version;
                    if (compareversion(package.installedVersion, package.version) == -1) {
                      packages.push(package);
                    }
                  }
                }
                $this.packages = packages;
                $this.isGetVersions = true;
              }
            }, 'packages');
          },
          checkAll: function () {
            this.isCheckAll = !this.isCheckAll;
            this.checkedIds = [];
            if (this.isCheckAll) {
              for (var i = 0; i < this.packages.length; i++) {
                this.checkedIds.push(this.packages[i].id);
              }
            }
          },
          download: function () {
            if (this.checkedIds.length == 0) return;

            this.step = 2;

            for (var i = 0; i < this.packages.length; i++) {
              if (this.downloadIds.indexOf(this.packages[i].id) == -1 && this.checkedIds.indexOf(this.packages[i]
                  .id) !== -1) {
                this.downloadingId = this.packages[i].id;
                this.downloadPackage(this.packages[i].id, this.packages[i].version)
                return;
              }
            }

            this.update();
          },
          downloadPackage: function (packageId, version) {
            var $this = this;

            downloadApi.post({
              packageId: packageId,
              version: version
            }, function (err, res) {
              if (err) {
                $this.errorMessage = err.message;
              } else if (res) {
                $this.downloadingId = 0;
                $this.downloadIds.push(packageId);
                $this.download();
              }
            });
          },
          update: function () {
            if (this.checkedIds.length == 0) return;

            this.step = 3;

            for (var i = 0; i < this.packages.length; i++) {
              if (this.updatedIds.indexOf(this.packages[i].id) == -1 && this.checkedIds.indexOf(this.packages[i].id) !==
                -1) {
                this.updatingId = this.packages[i].id;
                this.updatePackage(this.packages[i].id, this.packages[i].version, this.packages[i].packageType)
                return;
              }
            }

            this.clearCache();
          },
          updatePackage: function (packageId, version, packageType) {
            var $this = this;

            updateApi.post({
              packageId: packageId,
              version: version,
              packageType: packageType
            }, function (err, res) {
              if (err) {
                $this.errorMessage = err.message;
              } else if (res) {
                $this.updatingId = 0;
                $this.updatedIds.push(packageId);
                $this.update();
              }
            });
          },
          clearCache: function () {
            this.step = 4;

            clearCacheApi.post(null, function (err, res) {
              if (err) {
                $this.errorMessage = err.message;
              } else {
                setTimeout(function () {
                  window.top.location.reload();
                }, 3000);
              }
            });
          }
        }
      });

      $vue.version();
    </script>