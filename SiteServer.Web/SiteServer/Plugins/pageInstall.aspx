<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageInstall" %>
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
            插件安装向导
          </h4>
          <p class="text-muted m-b-25 font-13">
            欢迎来到插件安装向导！
          </p>

          <ctrl:alerts runat="server" />

          <ul class="nav nav-pills nav-fill bg-muted m-b-20">
            <li class="nav-item" v-bind:class="{ active: step == 1 }">
              <a class="nav-link" href="javascript:;">下载安装包</a>
            </li>
            <li class="nav-item" v-bind:class="{ active: step == 2 }">
              <a class="nav-link" href="javascript:;">安装插件</a>
            </li>
            <li class="nav-item" v-bind:class="{ active: step == 3 }">
              <a class="nav-link" href="javascript:;">安装完成</a>
            </li>
          </ul>

          <div class="alert alert-danger" v-bind:style="{ display: errorMessage ? '' : 'none' }" style="display: none">
            {{ errorMessage }}
          </div>

          <!-- step 1 place -->
          <div v-bind:style="{ display: step == 1 ? '' : 'none' }">

            <div class="panel panel-border panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title">下载安装包</h3>
                <p class="panel-sub-title font-13 text-muted">系统正在下载插件安装包，可能需要几分钟，请稍后...</p>
              </div>
              <div class="panel-body">

                <div v-bind:style="{ display: !isGetVersions ? '' : 'none' }" class="jumbotron text-center">
                  <img src="../pic/animated_loading.gif" />
                  <br />
                  <br />
                  <p class="lead">正在检查安装包版本，请稍后...</p>
                </div>

                <div class="table-responsive" v-bind:style="{ display: isGetVersions ? '' : 'none' }">

                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-center text-nowrap">状态</th>
                        <th class="text-nowrap">Id</th>
                        <th class="text-nowrap">名称</th>
                        <th class="text-nowrap">版本</th>
                        <th>简介</th>
                        <th class="text-center text-nowrap">发布时间</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="package in packages">
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
                          {{ package.version }}
                        </td>
                        <td>
                          {{ package.description }}
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

          <!-- step 2 place -->
          <div v-bind:style="{ display: step == 2 ? '' : 'none' }">

            <div class="panel panel-border panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title">安装插件</h3>
                <p class="panel-sub-title font-13 text-muted">系统正在安装插件，请稍后...</p>
              </div>
              <div class="panel-body">

                <div class="table-responsive">

                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-center text-nowrap">状态</th>
                        <th class="text-nowrap">Id</th>
                        <th class="text-nowrap">名称</th>
                        <th class="text-nowrap">版本</th>
                        <th>简介</th>
                        <th class="text-center text-nowrap">发布时间</th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="package in packages">
                        <td class="text-center text-nowrap font-13 text-muted">
                          <div class="text-success" v-bind:style="{ display: updatedIds.indexOf(package.id) !== -1 && updatingId != package.id ? '' : 'none' }">
                            安装完成
                          </div>
                          <div v-bind:style="{ display:updatingId == package.id ? '' : 'none' }">
                            <img src="../pic/animated_loading.gif" />安装中...
                          </div>
                          <div v-bind:style="{ display: updatedIds.indexOf(package.id) == -1 && updatingId != package.id ? '' : 'none' }">
                            等待安装
                          </div>
                        </td>
                        <td class="text-nowrap">
                          {{ package.id }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.title }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.version }}
                        </td>
                        <td>
                          {{ package.description }}
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

            <div class="alert alert-success" role="alert">
              <h4 class="alert-heading">安装完成！</h4>
              <p>
                恭喜，您已经完成了插件的安装，页面将在3秒之后重新载入...
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
      var api = new apiUtils.Api();
      var downloadApi = new apiUtils.Api('<%=DownloadApiUrl%>');
      var updateApi = new apiUtils.Api('<%=UpdateApiUrl%>');
      var clearCacheApi = new apiUtils.Api('<%=ClearCacheApiUrl%>');
      var isNightly = <%=IsNightly%>;
      var version = '<%=Version%>';

      var data = {
        step: 1,
        isGetVersions: false,
        packageId: '<%=PackageId%>',
        installedPackages: <%=PackagesIdAndVersionList%>,
        package: {},
        packages: [],
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
          getPackage: function () {
            var $this = this;

            api.get({
              isNightly: isNightly,
              version: version
            }, function (err, res) {
              if (!err && res) {
                $this.package = res;
                $this.packages.push(res);
                var packageIds = [];
                for (var i = 0; i < res.pluginReferences.length; i++) {
                  var reference = res.pluginReferences[i];
                  var installedPackage = $.grep($this.packages, function (e) {
                    return e.id == reference.id;
                  });
                  if (installedPackage.length == 0) {
                    packageIds.push(reference.id);
                  }
                }
                for (var i = 0; i < res.packageReferences.length; i++) {
                  var reference = res.packageReferences[i];
                  var installedPackage = $.grep($this.packages, function (e) {
                    return e.id == reference.id;
                  });
                  if (installedPackage.length == 0) {
                    packageIds.push(reference.id);
                  }
                }
                $this.getPackages(packageIds);
              }
            }, 'packages/' + this.packageId);
          },
          getPackages: function (packageIds) {
            var $this = this;

            if (packageIds.length == 0) {
              $this.isGetVersions = true;
              $this.download();
              return;
            }

            api.get({
              isNightly: isNightly,
              version: version,
              $filter: "id in '" + packageIds.join(",") + "'"
            }, function (err, res) {
              if (!err && res) {
                for (var i = 0; i < res.length; i++) {
                  var package = res[i];
                  for (var j = 0; j < $this.installedPackages.length; j++) {
                    var installedPackage = $this.installedPackages[j];
                    if (installedPackage === (package.id + '.' + package.version)) {
                      $this.downloadIds.push(package.id);
                      break;
                    }
                  }
                  $this.packages.push(package);
                }
                $this.isGetVersions = true;
                $this.download();
              }
            }, 'packages');
          },
          download: function () {
            for (var i = 0; i < this.packages.length; i++) {
              if (this.downloadIds.indexOf(this.packages[i].id) == -1) {
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
            this.step = 2;

            for (var i = 0; i < this.packages.length; i++) {
              if (this.updatedIds.indexOf(this.packages[i].id) == -1) {
                this.updatingId = this.packages[i].id;
                this.updatePackage(this.packages[i].id, this.packages[i].version, this.packages[i].packageType);
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
            this.step = 3;

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

      $vue.getPackage();
    </script>