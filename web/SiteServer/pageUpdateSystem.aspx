<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageUpdateSystem" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <title>SiteServer CMS 升级向导</title>
  <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
  <link href="./assets/icons/favicon.png" rel="icon" type="image/png">
  <link href="./assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
  <link href="./assets/css/core.css" rel="stylesheet" type="text/css" />
  <link href="./assets/css/components.css" rel="stylesheet" type="text/css" />
  <link href="./assets/css/pages.css" rel="stylesheet" type="text/css" />
  <link href="./assets/css/menu.css" rel="stylesheet" type="text/css" />
  <link href="./assets/css/responsive.css" rel="stylesheet" type="text/css" />
  <link href="./assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  <script src="./assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
  <script src="./assets/sweetalert/sweetalert.min.js" type="text/javascript"></script>
  <script src="./assets/layer/layer.min.js" type="text/javascript"></script>
  <script src="./inc/script.js" type="text/javascript"></script>
  <style>
    body {
          padding: 20px 0;
        }
      </style>
</head>

<body>
  <form id="main" class="m-l-15 m-r-15" runat="server">

    <div class="card-box">
      <h4 class="text-dark  header-title m-t-0">
        SiteServer CMS 升级向导
      </h4>
      <p class="text-muted m-b-25 font-13">
        欢迎来到SiteServer CMS 升级向导！
      </p>

      <ctrl:alerts runat="server" />

      <ul class="nav nav-pills nav-fill bg-muted m-b-20">
        <li class="nav-item" v-bind:class="{ active: step == 1 }">
          <a class="nav-link" href="javascript:;">检查更新</a>
        </li>
        <li class="nav-item" v-bind:class="{ active: step == 2 }">
          <a class="nav-link" href="javascript:;">升级系统</a>
        </li>
        <li class="nav-item" v-bind:class="{ active: step == 3 }">
          <a class="nav-link" href="javascript:;">升级完成</a>
        </li>
      </ul>

      <div class="alert alert-danger" v-bind:style="{ display: errorMessage ? '' : 'none' }" style="display: none">
        升级遇到错误：{{ errorMessage }}
        <br /> 您可以选择下载升级包并手动升级，请点击
        <asp:button id="BtnUpload" class="btn btn-success" Text="手动升级" runat="server"></asp:button>
      </div>

      <!-- step 1 place -->
      <div v-bind:style="{ display: step == 1 && !errorMessage ? '' : 'none' }">

        <div class="panel panel-border panel-primary">
          <div class="panel-heading">
            <h3 class="panel-title">检查更新</h3>
            <p class="panel-sub-title font-13 text-muted">检查 SiteServer CMS 新版本</p>
          </div>
          <div class="panel-body">

            <div v-bind:style="{ display: !package || !package.version ? '' : 'none' }" class="jumbotron text-center">
              <img src="./pic/animated_loading.gif" />
              <br />
              <br />
              <p class="lead">正在检查系统更新，请稍后...</p>
            </div>

            <div v-bind:style="{ display: package && !isShouldUpdate ? '' : 'none' }" class="jumbotron" style="display: none">
              <h4 class="display-5">当前版本已经是最新版本
                <a class="btn btn-success m-l-5" href="<%=AdminUrl%>">进入后台</a>
              </h4>

            </div>

            <div v-bind:style="{ display: package && isShouldUpdate ? '' : 'none' }" class="table-responsive" style="display: none">

              <div class="alert alert-success">
                发现 SiteServer CMS 新版本，请选中复选框后点击下一步开始升级
              </div>

              <table class="table tablesaw table-hover m-0">
                <thead>
                  <tr class="thead">
                    <th class="text-center text-nowrap">选择</th>
                    <th class="text-nowrap">已安装版本</th>
                    <th class="text-nowrap">新版本</th>
                    <th>更新说明</th>
                    <th class="text-center">发布时间</th>
                    <th class="text-center"></th>
                  </tr>
                </thead>
                <tbody>
                  <tr>
                    <td class="text-center checkbox checkbox-primary text-nowrap">
                      <input type="checkbox" id="all" value="all" @change='check()' />
                      <label for="all">选择</label>
                    </td>
                    <td class="text-nowrap">
                      {{ installedVersion }}
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
                    <td class="text-center text-nowrap">
                      <a class="card-link" v-bind:href="updatesUrl" target="_blank">查看发行说明</a>
                    </td>
                  </tr>
                </tbody>
              </table>

              <hr />

              <div class="text-center">
                <input class="btn" @click="updateSsCms" v-bind:disabled="!isCheck" v-bind:class="{ 'btn-primary': isCheck }"
                  type="button" value="下一步">
              </div>
            </div>
          </div>
        </div>

      </div>

      <!-- step 2 place -->
      <div v-bind:style="{ display: step == 2 ? '' : 'none' }" style="display: none">

        <div class="jumbotron text-center">
          <img src="./pic/animated_loading.gif" />
          <br />
          <br />
          <p class="lead">正在升级系统，可能需要几分钟，请稍后...</p>
        </div>

      </div>

      <!-- step 3 place -->
      <div v-bind:style="{ display: step == 3 ? '' : 'none' }" style="display: none">

        <div class="alert alert-success" role="alert">
          <h4 class="alert-heading">升级完成！</h4>
          <p>
            恭喜，您已经完成了 SiteServer CMS 系统的升级，请点击按钮进入数据库升级向导
            <a class="btn btn-success m-l-5" href="pageSyncDatabase.aspx">进入数据库升级向导</a>
          </p>
          <hr>
          <p class="mb-0">
            获取更多使用帮助请访问
            <a href="http://docs.siteserver.cn" target="_blank">SiteServer CMS 文档中心</a>
          </p>
        </div>

      </div>

    </div>

  </form>
</body>

</html>
<!--#include file="./inc/foot.html"-->
<script type="text/javascript" src="assets/vue/vue.min.js"></script>
<script type="text/javascript" src="assets/js/apiUtils.js"></script>
<script type="text/javascript" src="assets/js/es6-promise.auto.min.js"></script>
<script type="text/javascript" src="assets/js/axios-0.17.1.min.js"></script>
<script type="text/javascript" src="assets/js/utils.js"></script>
<script type="text/javascript" src="assets/js/compareversion.js"></script>
<script type="text/javascript">
  var updateSsCmsApi = new apiUtils.Api('<%=UpdateSsCmsApiUrl%>');
  var isNightly = <%=IsNightly%>;
  var version = '<%=Version%>';
  var packageId = '<%=PackageId%>';

  var data = {
    step: 1,
    package: {},
    isCheck: false,
    isShouldUpdate: false,
    installedVersion: '<%=InstalledVersion%>',
    updatesUrl: '',
    errorMessage: null
  };

  var $vue = new Vue({
    el: '#main',
    data: data,
    methods: {
      version: function () {
        var $this = this;

        $apiCloud.get('updates', {
          params: {
            isNightly: isNightly,
            pluginVersion: version,
            packageIds: packageId
          }
        }).then(function (response) {
          var res = response.data;

          $this.package = res.value[0];
          $this.isShouldUpdate = compareversion($this.installedVersion, $this.package.version) == -1;
          var major = $this.package.version.split('.')[0];
          var minor = $this.package.version.split('.')[1];
          $this.updatesUrl = 'http://www.siteserver.cn/updates/v' + major + '_' + minor + '/index.html';

        }).catch(function (error) {
          $this.pageAlert = utils.getPageAlert(error);
        }).then(function () {
          $this.pageLoad = true;
        });

        // ssApi.get({
        //   isNightly: isNightly,
        //   version: version
        // }, function (err, res) {
        //   if (err || !res || !res.value) return;

        //   $this.package = res.value;
        //   $this.isShouldUpdate = compareversion($this.installedVersion, $this.package.version) == -1;
        //   var major = $this.package.version.split('.')[0];
        //   var minor = $this.package.version.split('.')[1];
        //   $this.updatesUrl = 'http://www.siteserver.cn/updates/v' + major + '_' + minor + '/index.html';
        // }, 'packages', packageId);
      },
      check: function () {
        this.isCheck = !this.isCheck;
      },
      updateSsCms: function () {
        if (!this.package) return;
        this.step = 2;
        var $this = this;

        updateSsCmsApi.post({
          version: $this.package.version
        }, function (err, res) {
          if (err) {
            $this.errorMessage = err.message;
          } else if (res) {
            $this.step = 3;
          }
        });
      }
    }
  });

  $vue.version();
</script>