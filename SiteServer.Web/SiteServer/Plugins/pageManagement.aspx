<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageManagement" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form id="main" class="m-l-15 m-r-15" runat="server">

        <template v-if="!pageLoad">
          <div class="text-center" style="margin-top: 100px">
            <img class="mt-3" src="../assets/images/loading.gif" />
            <p class="lead mt-3 text-nowrap">载入中，请稍后...</p>
          </div>
        </template>
        <template v-else>
          <div class="card-box">
            <div class="row">
              <div class="col-8">
                <ul class="nav nav-pills">
                  <li class="nav-item" v-bind:class="{ active: pageType == 1 }">
                    <a class="nav-link" href="javascript:;" @click="pageType = 1">
                      已启用
                      <span class="badge badge-secondary" v-bind:class="{ 'badge-light': pageType == 1 }">{{ enabledPackages.length }}</span>
                    </a>
                  </li>
                  <li class="nav-item" v-bind:class="{ active: pageType == 2 }">
                    <a class="nav-link" href="javascript:;" @click="pageType = 2">
                      已禁用
                      <span class="badge badge-secondary" v-bind:class="{ 'badge-light': pageType == 2 }">{{ disabledPackages.length }}</span>
                    </a>
                  </li>
                  <li class="nav-item" v-bind:class="{ active: pageType == 3 }" v-bind:style="{ display: errorPackages.length > 0 ? '' : 'none' }"
                    style="display: none">
                    <a class="nav-link" href="javascript:;" @click="pageType = 3">
                      运行错误
                      <span class="badge badge-danger" v-bind:style="{ color: (pageType == 3 ? '#fff' : '') }">{{ errorPackages.length }}</span>
                    </a>
                  </li>
                  <li class="nav-item" v-bind:class="{ active: pageType == 4 }" v-bind:style="{ display: updatePackages.length > 0 ? '' : 'none' }"
                    style="display: none">
                    <a class="nav-link" href="javascript:;" @click="pageType = 4">
                      发现新版本
                      <span class="badge badge-warning" v-bind:style="{ color: (pageType == 4 ? '#fff' : '') }">{{ updatePackages.length }}</span>
                    </a>
                  </li>
                </ul>
              </div>
              <div class="col-4">
                  <a class="btn btn-default m-l-5 float-right btn-md" href="pageAdd.aspx">添加插件</a>
                <asp:Button onClick="BtnReload_Click" class="btn btn-primary float-right btn-md" Text="重新加载所有插件" runat="server" />
              </div>
            </div>
          </div>

          <ctrl:alerts runat="server" />

          <div class="card-box">

            <div class="panel panel-default" v-bind:style="{ display: pageType == 1 ? '' : 'none' }">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-nowrap">LOGO</th>
                        <th class="text-nowrap">插件Id</th>
                        <th class="text-nowrap">插件名称</th>
                        <th class="text-nowrap">版本号</th>
                        <th class="text-nowrap">作者</th>
                        <th>插件介绍</th>
                        <th class="text-center text-nowrap">载入时间</th>
                        <th class="text-nowrap"></th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="package in enabledPackages">
                        <td class="text-center text-nowrap">
                          <img v-bind:src="package.metadata.iconUrl || '../assets/icons/favicon.png'" width="48" height="48">
                        </td>
                        <td class="text-nowrap">
                          <a v-bind:href="'pageview.aspx?pluginId=' + package.id + '&returnUrl=pageManagement.aspx'">{{ package.id }}</a>
                        </td>
                        <td class="text-nowrap">
                          {{ package.metadata.title }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.metadata.version }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.metadata.owners }}
                        </td>
                        <td>
                          {{ package.metadata.description }}
                        </td>
                        <td class="text-center text-nowrap">
                          {{ package.initTime }}毫秒
                        </td>
                        <td class="text-center text-nowrap">
                          <a href="javascript:;" @click="enablePackage(package)">{{ package.isDisabled ? '启用' : '禁用' }}</a>
                          &nbsp;&nbsp;
                          <a href="javascript:;" @click="deletePackage(package)">删除插件</a>
                        </td>
                      </tr>
                    </tbody>
                  </table>

                </div>
              </div>
            </div>

            <div class="panel panel-default" v-bind:style="{ display: pageType == 2 ? '' : 'none' }">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-nowrap">LOGO</th>
                        <th class="text-nowrap">插件Id</th>
                        <th class="text-nowrap">插件名称</th>
                        <th class="text-nowrap">版本号</th>
                        <th class="text-nowrap">作者</th>
                        <th>插件介绍</th>
                        <th class="text-center text-nowrap">载入时间</th>
                        <th class="text-nowrap"></th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="package in disabledPackages">
                        <td class="text-center text-nowrap">
                          <img v-bind:src="package.metadata.iconUrl || '../assets/icons/favicon.png'" width="48" height="48">
                        </td>
                        <td class="text-nowrap">
                          <a v-bind:href="'pageview.aspx?pluginId=' + package.id + '&returnUrl=pageManagement.aspx'">{{ package.id }}</a>
                        </td>
                        <td class="text-nowrap">
                          {{ package.metadata.title }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.metadata.version }}
                        </td>
                        <td class="text-nowrap">
                          {{ package.metadata.owners }}
                        </td>
                        <td>
                          {{ package.metadata.description }}
                        </td>
                        <td class="text-center text-nowrap">
                          {{ package.initTime }}毫秒
                        </td>
                        <td class="text-center text-nowrap">
                          <a href="javascript:;" @click="enablePackage(package)">{{ package.isDisabled ? '启用' : '禁用' }}</a>
                          &nbsp;&nbsp;
                          <a href="javascript:;" @click="deletePackage(package)">删除插件</a>
                        </td>
                      </tr>
                    </tbody>
                  </table>

                </div>
              </div>
            </div>

            <div class="panel panel-default" v-bind:style="{ display: pageType == 3 ? '' : 'none' }">
              <div class="panel-body p-0">
                <div class="table-responsive">
                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th>插件Id</th>
                        <th>错误详情</th>
                        <th></th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr v-for="package in errorPackages">
                        <td class="text-nowrap">
                          <a v-bind:href="'pageview.aspx?pluginId=' + package.id + '&returnUrl=pageManagement.aspx'">{{ package.id }}</a>
                        </td>
                        <td class="text-nowrap">
                          {{ package.errorMessage }}
                        </td>
                        <td class="text-center text-nowrap">
                          <a href="javascript:;" @click="deletePackage(package)">删除插件</a>
                        </td>
                      </tr>
                    </tbody>
                  </table>

                </div>
              </div>
            </div>

            <div v-bind:style="{ display: pageType == 4 ? '' : 'none' }">

              <div class="alert alert-success">
                发现以下插件发布了新版本，请点击升级插件按钮开始升级
              </div>

              <div class="panel panel-default">
                <div class="panel-body p-0">
                  <div class="table-responsive">
                    <table class="table tablesaw table-hover m-0">
                      <thead>
                        <tr class="thead">
                          <th class="text-nowrap">LOGO</th>
                          <th class="text-nowrap">插件Id</th>
                          <th class="text-nowrap">插件名称</th>
                          <th class="text-nowrap">已安装版本</th>
                          <th class="text-nowrap">新版本</th>
                          <th>更新说明</th>
                          <th class="text-center text-nowrap">发布时间</th>
                          <th></th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr v-for="package in updatePackages">
                          <td class="text-center text-nowrap">
                            <img v-bind:src="package.updatePackage.iconUrl || '../assets/icons/favicon.png'" width="48" height="48">
                          </td>
                          <td class="text-nowrap">
                            <a v-bind:href="'pageView.aspx?pluginId=' + package.id + '&returnUrl=pageManagement.aspx'">{{ package.id }}</a>
                          </td>
                          <td class="text-nowrap">
                            {{ package.updatePackage.title }}
                          </td>
                          <td class="text-nowrap">
                            {{ package.metadata ? package.metadata.version : '' }}
                          </td>
                          <td class="text-nowrap">
                            {{ package.updatePackage.version }}
                          </td>
                          <td>
                            {{ package.updatePackage.releaseNotes }}
                          </td>
                          <td class="text-center text-nowrap">
                            {{ package.updatePackage.published }}
                          </td>
                          <td class="text-center text-nowrap">
                            <a v-bind:href="'install.cshtml?isUpdate=true&packageIds=' + package.id" class="btn btn-warning btn-md">插件升级</a>
                          </td>
                        </tr>
                      </tbody>
                    </table>
                  </div>
                </div>
              </div>

              <hr />

              <a v-bind:href="'install.cshtml?isUpdate=true&packageIds=' + updatePackageIds.join(',')" class="btn btn-warning btn-md">一键升级所有插件</a>

            </div>

          </div>
        </template>
      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->
    <script src="../assets/vue/vue.min.js"></script>
    <script src="../assets/js/apiUtils.js"></script>
    <script src="../assets/js/compareversion.js"></script>
    <script type="text/javascript">
      var ssApi = new apiUtils.Api();
      var isNightly = <%=IsNightly%>;
      var version = '<%=Version%>';

      var data = {
        pageType: <%=PageType%> === 0 ? 1 : <%=PageType%>,
        pageLoad: false,
        allPackages: <%=Packages%>,
        enabledPackages: [],
        disabledPackages: [],
        errorPackages: [],
        updatePackages: [],
        updatePackageIds: [],
        packageIds: '<%=PackageIds%>',
        referencePackageIds: []
      };

      for (var i = 0; i < data.allPackages.length; i++) {
        var package = data.allPackages[i];
        if (package.isRunnable && package.metadata) {
          if (package.isDisabled) {
            data.disabledPackages.push(package);
          } else {
            data.enabledPackages.push(package);
          }
        } else {
          data.errorPackages.push(package);
        }
      }

      var $vue = new Vue({
        el: '#main',
        data: data,
        methods: {
          load: function () {
            var $this = this;

            ssApi.get({
              isNightly: isNightly,
              version: version,
              $filter: "id in '" + this.packageIds + "'"
            }, function (err, res) {
              if (err || !res || !res.value) return;

              for (var i = 0; i < res.value.length; i++) {
                var package = res.value[i];

                for (var j = 0; j < package.pluginReferences.length; j++) {
                  var reference = package.pluginReferences[j];
                  var installedPackages = $.grep($this.allPackages, function (e) {
                    return e.id == reference.id;
                  });
                  if (installedPackages.length > 0) {
                    $this.referencePackageIds.push(reference.id);
                  }
                }

                var installedPackages = $.grep($this.allPackages, function (e) {
                  return e.id == package.id;
                });
                if (installedPackages.length == 1) {
                  var installedPackage = installedPackages[0];
                  installedPackage.updatePackage = package;

                  if (installedPackage.metadata && installedPackage.metadata.version) {
                    if (compareversion(installedPackage.metadata.version, package.version) == -1) {
                      $this.updatePackages.push(installedPackage);
                      $this.updatePackageIds.push(installedPackage.id);
                    }
                  } else {
                    $this.updatePackages.push(installedPackage);
                    $this.updatePackageIds.push(installedPackage.id);
                  }
                }
              }
              $this.pageLoad = true;
            }, 'packages');
          },
          enablePackage: function (package) {
            var text = package.isDisabled ? '启用' : '禁用';
            var isReference = this.referencePackageIds.indexOf(package.id) !== -1;
            if (isReference) {
              return swal("无法" + text, "存在其他插件依赖此插件，需要删除依赖插件后才能进行" + text + "操作", "error");
            }
            swal({
              title: text + '插件',
              text: '此操作将会禁用“' + package.id + '”，确认吗？',
              icon: 'warning',
              buttons: {
                cancel: {
                  text: '取 消',
                  visible: true,
                  className: 'btn'
                },
                confirm: {
                  text: package.isDisabled ? '启 用' : '禁 用',
                  visible: true,
                  className: 'btn btn-danger'
                }
              }
            })
              .then(function (isConfirm) {
                if (isConfirm) {
                  if (package.isDisabled) {
                    location.href = 'pageManagement.aspx?enable=True&pluginId=' + package.id;
                  } else {
                    location.href = 'pageManagement.aspx?disable=True&pluginId=' + package.id;
                  }
                }
              });
          },
          deletePackage: function (package) {
            var isReference = this.referencePackageIds.indexOf(package.id) !== -1;
            if (isReference) {
              return swal("无法删除", "存在其他插件依赖此插件，需要删除依赖插件后才能进行删除操作", "error");
            }
            swal({
              title: '删除插件',
              text: '此操作将会删除“' + package.id + '”，确认吗？',
              icon: 'warning',
              buttons: {
                cancel: {
                  text: '取 消',
                  visible: true,
                  className: 'btn'
                },
                confirm: {
                  text: '确认删除',
                  visible: true,
                  className: 'btn btn-danger'
                }
              }
            })
              .then(function (isConfirm) {
                if (isConfirm) {
                  location.href = 'pageManagement.aspx?delete=True&pluginId=' + package.id;
                }
              });
          }
        }
      });

      $vue.load();
    </script>