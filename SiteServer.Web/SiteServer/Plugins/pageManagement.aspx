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

        <div class="card-box">
          <div class="row">
            <div class="col-8">
              <ul class="nav nav-pills">
                <li class="nav-item" v-bind:class="{ active: pageType == 1 }">
                  <a class="nav-link" href="javascript:;" @click="pageType = 1">
                    已启用
                    <span class="badge badge-secondary" v-bind:class="{ 'badge-light': pageType == 1 }">{{ countEnabled }}</span>
                  </a>
                </li>
                <li class="nav-item" v-bind:class="{ active: pageType == 2 }">
                  <a class="nav-link" href="javascript:;" @click="pageType = 2">
                    已禁用
                    <span class="badge badge-secondary" v-bind:class="{ 'badge-light': pageType == 2 }">{{ countDisabled }}</span>
                  </a>
                </li>
                <li class="nav-item" v-bind:class="{ active: pageType == 3 }" v-bind:style="{ display: countError > 0 ? '' : 'none' }" style="display: none">
                  <a class="nav-link" href="javascript:;" @click="pageType = 3">
                    运行错误
                    <span class="badge badge-danger" v-bind:style="{ color: (pageType == 3 ? '#fff' : '') }">{{ countError }}</span>
                  </a>
                </li>
                <li class="nav-item" v-bind:class="{ active: pageType == 4 }" v-bind:style="{ display: countUpdate > 0 ? '' : 'none' }" style="display: none">
                  <a class="nav-link" href="javascript:;" @click="pageType = 4">
                    发现新版本
                    <span class="badge badge-warning" v-bind:style="{ color: (pageType == 4 ? '#fff' : '') }">{{ countUpdate }}</span>
                  </a>
                </li>
              </ul>
            </div>
            <div class="col-4">
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
                    <asp:Repeater ID="RptEnabled" runat="server">
                      <itemtemplate>
                        <tr>
                          <td class="text-center align-middle text-nowrap">
                            <asp:Literal ID="ltlLogo" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle text-nowrap">
                            <asp:Literal ID="ltlPluginId" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle text-nowrap">
                            <asp:Literal ID="ltlPluginName" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle text-nowrap">
                            <asp:Literal ID="ltlVersion" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle text-nowrap">
                            <asp:Literal ID="ltlOwners" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle">
                            <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center align-middle text-nowrap">
                            <asp:Literal ID="ltlInitTime" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center align-middle text-nowrap">
                            <asp:Literal ID="ltlCmd" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </itemtemplate>
                    </asp:Repeater>
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
                    <asp:Repeater ID="RptDisabled" runat="server">
                      <itemtemplate>
                        <tr>
                          <td class="text-center align-middle text-nowrap">
                            <asp:Literal ID="ltlLogo" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle text-nowrap">
                            <asp:Literal ID="ltlPluginId" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle text-nowrap">
                            <asp:Literal ID="ltlPluginName" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle text-nowrap">
                            <asp:Literal ID="ltlVersion" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle text-nowrap">
                            <asp:Literal ID="ltlOwners" runat="server"></asp:Literal>
                          </td>
                          <td class="align-middle">
                            <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center align-middle text-nowrap">
                            <asp:Literal ID="ltlInitTime" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center align-middle text-nowrap">
                            <asp:Literal ID="ltlCmd" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </itemtemplate>
                    </asp:Repeater>
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
                    <asp:Repeater ID="RptError" runat="server">
                      <itemtemplate>
                        <tr>
                          <td>
                            <asp:Literal ID="ltlPluginId" runat="server"></asp:Literal>
                          </td>
                          <td>
                            <asp:Literal ID="ltlErrorMessage" runat="server"></asp:Literal>
                          </td>
                          <td class="text-center">
                            <asp:Literal ID="ltlCmd" runat="server"></asp:Literal>
                          </td>
                        </tr>
                      </itemtemplate>
                    </asp:Repeater>
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

            <hr />

            <div class="text-center">
              <a href="pageUpdate.aspx" class="btn btn-primary btn-md">升级插件</a>
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
      var isNightly = <%=IsNightly%>;
      var version = '<%=Version%>';

      var data = {
        pageType: <%=PageType%>,
        countEnabled: <%=CountEnabled%>,
        countDisabled: <%=CountDisabled%>,
        countError: <%=CountError%>,
        countUpdate: 0,
        isGetVersions: false,
        packages: <%=Packages%>,
        packageIds: '<%=PackageIds%>'
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
              packageIds: this.packageIds
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
                      $this.countUpdate++;
                    }
                  }
                }
                $this.packages = packages;
                $this.isGetVersions = true;
              }
            }, 'packages/actions/list');
          }
        }
      });

      $vue.version();
    </script>