<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageView" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <!--#include file="../inc/head.html"-->
    <link href="../assets/showLoading/css/showLoading.css" rel="stylesheet" />
    <script type="text/javascript" src="../assets/showLoading/js/jquery.showLoading.js"></script>
  </head>

  <body>
    <form id="main" class="m-l-15 m-r-15" runat="server">

      <div class="text-center" style="margin-top: 100px" v-bind:style="{ display: package.id ? 'none' : '' }">
        <img class="mt-3" src="../assets/images/loading.gif" />
        <p class="lead mt-3 text-nowrap">载入中，请稍后...</p>
      </div>

      <div v-bind:style="{ display: package.id ? '' : 'none' }" style="display: none">

        <div class="card-box widget-icon">
          <div>
            <img v-bind:src="package.iconUrl" style="height: 100px; width: 100px;" class="img-responsive float-left">
            <div class="wid-icon-info" style="margin-left: 120px;">
              <p class="text-muted m-b-5 font-13">
                {{ package.id }}.{{ package.version }}
              </p>
              <h4 class="m-t-0 m-b-5 counter">{{ package.title }}</h4>
              <hr />
              <p class="lead">
                {{ package.description }}
              </p>

              <div class="alert alert-warning" v-bind:style="{ display: installed && isShouldUpdate ? '' : 'none' }">
                系统检测到插件新版本，当前版本：{{ installedVersion }}，新版本：{{ package.version }}
                <input v-on:click="location.href='install.cshtml?isUpdate=true&packageIds=' + package.id;return false;" type="button" value="升级插件"
                  class="btn btn-primary">
              </div>

              <div>
                <input v-on:click="location.href='install.cshtml?packageIds=' + package.id;return false;"
                  type="button" value="安装插件" class="btn btn-primary" v-bind:style="{ display: !installed ? '' : 'none' }">
                <input type="button" disabled="disabled" value="插件已安装" class="btn m-l-5" v-bind:style="{ display: installed && installedVersion == package.version ? '' : 'none' }">

                <a class="btn btn-success m-l-5" v-bind:style="{ display: package.projectUrl ? '' : 'none' }" target="_blank" v-bind:href="package.projectUrl">插件主页</a>
                <asp:Button class="btn m-l-5" onClick="Return_Click" Text="返 回" runat="server" />
              </div>
            </div>
          </div>
        </div>

        <!-- <div class="card-box">
          <div v-html="package.readme" class="readme m-b-10"></div>
        </div> -->

        <div class="card-box">
          <div class="page-title-box">
            <h4 class="page-title">插件详情</h4>
          </div>

          <table class="table m-0 m-t-25">
            <tbody>
              <tr>
                <th scope="row">版本发行说明</th>
                <td>{{ package.releaseNotes }}</td>
              </tr>
              <tr>
                <th scope="row">更新日期</th>
                <td>{{ package.published }}</td>
              </tr>
              <tr>
                <th scope="row">插件Id</th>
                <td>{{ package.id }}</td>
              </tr>
              <tr>
                <th scope="row">版本号</th>
                <td>{{ package.version }}</td>
              </tr>
              <tr>
                <th scope="row">作者</th>
                <td>{{ package.authors ? package.authors.join(',') : '' }}</td>
              </tr>
              <tr>
                <th scope="row">标签</th>
                <td>{{ package.tags }}</td>
              </tr>
              <tr>
                <th scope="row">插件项目链接</th>
                <td>
                  <a v-bind:style="{ display: package.projectUrl ? '' : 'none' }" target="_blank" v-bind:href="package.projectUrl">
                    {{ package.projectUrl }}
                  </a>
                </td>
              </tr>
              <tr>
                <th scope="row">插件许可链接</th>
                <td>
                  <a v-bind:style="{ display: package.licenseUrl ? '' : 'none' }" target="_blank" v-bind:href="package.licenseUrl">
                    {{ package.licenseUrl }}
                  </a>
                </td>
              </tr>
              <tr>
                <th scope="row">版权</th>
                <td>{{ package.copyright }}</td>
              </tr>
            </tbody>
          </table>
        </div>

        <div class="card-box" v-if="(package.pluginReferences && package.pluginReferences.length > 0) || (package.packageReferences && package.packageReferences.length > 0)">
          <div class="page-title-box">
            <h4 class="page-title">
              依赖项
            </h4>
          </div>

          <p class="text-muted font-13 m-b-25">
            此插件依赖的类库以及其他插件
          </p>
          <table class="table m-0">
            <thead>
              <tr>
                <th>依赖项</th>
                <th>版本</th>
                <th>类型</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="reference in package.pluginReferences">
                <td>{{ reference.id }}</td>
                <td>{{ reference.version }}</td>
                <td>插件</td>
              </tr>
              <tr v-for="reference in package.packageReferences">
                <td>{{ reference.id }}</td>
                <td>{{ reference.version }}</td>
                <td>类库</td>
              </tr>
            </tbody>
          </table>
        </div>

      </div>
    </form>
  </body>

  </html>

  <script src="../assets/vue/vue.min.js"></script>
  <script src="../assets/js/apiUtils.js"></script>
  <script src="../assets/js/compareversion.js"></script>
  <script>
    var ssApi = new apiUtils.Api();
    var isNightly = <%=IsNightly%>;
    var version = '<%=Version%>';
    var pluginId = ssApi.getQueryStringByName('pluginId');

    var data = {
      installed: <%=Installed%>,
      installedVersion: '<%=InstalledVersion%>',
      package: <%=Package%> || {},
      isShouldUpdate: false
    };

    ssApi.get({
      isNightly: isNightly,
      version: version
    }, function (err, res) {
      if (err || !res || !res.value) return;

      data.package = res.value;
      data.isShouldUpdate = compareversion('<%=InstalledVersion%>', data.package.version) == -1;
    }, 'packages/' + pluginId);

    new Vue({
      el: '#main',
      data: data
    });
  </script>
  <!--#include file="../inc/foot.html"-->
  <script>
    var validate = window.Page_ClientValidate;
    $(function () {
      $('.btn-primary').click(function () {
        if (!validate || validate()) {
          $('#main').showLoading();
        }
        return true;
      });
    });
  </script>