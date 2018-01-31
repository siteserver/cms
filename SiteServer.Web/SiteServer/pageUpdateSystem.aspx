<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageUpdateSystem" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <title>SiteServer CMS 升级向导</title>
      <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
      <link href="./images/siteserver_icon.png" rel="icon" type="image/png">
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
      <link href="./assets/showLoading/css/showLoading.css" rel="stylesheet" media="screen" />
      <script type="text/javascript" src="./assets/showLoading/js/jquery.showLoading.js"></script>
      <style>
        body {
          padding: 20px 0;
        }
      </style>
    </head>

    <body>
      <form class="m-l-15 m-r-15" runat="server">

        <div id="main" class="card-box">
          <h4 class="text-dark  header-title m-t-0">
            SiteServer CMS 升级向导
          </h4>
          <p class="text-muted m-b-25 font-13">
            欢迎来到SiteServer CMS 升级向导！
          </p>

          <ctrl:alerts runat="server" />

          <!-- step 1 place -->
          <div id="phStep1">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="jumbotron">
              <p class="lead">
                升级向导将在线检查并自动下载、安装 SiteServer CMS 产品最新版本。
              </p>
              <hr class="my-4" style="display: none">
              <p style="display: none">
                如果不希望升级到最新版，可以选择指定版本进行升级，请点击
                <asp:button id="BtnUpload" class="btn btn-success" Text="手动升级" runat="server"></asp:button>
              </p>
            </div>

            <hr />

            <div class="text-center">
              <input id="btnStep1" type="button" value="下一步" class="btn btn-primary">
            </div>

          </div>

          <!-- step 2 place -->
          <div id="phStep2" style="display: none">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="panel panel-border panel-primary">
              <div class="panel-heading">
                <h3 class="panel-title">检查更新</h3>
                <p class="panel-sub-title font-13 text-muted">检查 SiteServer CMS 新版本</p>
              </div>
              <div class="panel-body">

                <div id="phStep2_loading" class="jumbotron text-center">
                  <img src="./pic/animated_loading.gif" />
                  <br />
                  <br />
                  <p class="lead">正在检查系统更新，请稍后...</p>
                </div>

                <div id="phStep2_currentVersion" class="jumbotron" style="display: none">
                  <h4 class="display-5">当前版本已经是最新版本</h4>
                </div>

                <div id="phStep2_newVersion" class="table-responsive" style="display: none">

                  <div class="alert alert-success">
                    发现 SiteServer CMS 新版本，请选中复选框后点击下一步开始升级
                  </div>

                  <table class="table tablesaw table-hover m-0">
                    <thead>
                      <tr class="thead">
                        <th class="text-center" width="200">选择</th>
                        <th>版本</th>
                        <th>更新简介</th>
                        <th class="text-center">发布日期</th>
                        <th class="text-center"></th>
                      </tr>
                    </thead>
                    <tbody>
                      <tr>
                        <td class="text-center checkbox checkbox-primary text-nowrap">
                          <input type="checkbox" name="choose" id="choose_" value="" onChange="this.checked ? $('#phStep2_newVersionNext').show() : $('#phStep2_newVersionNext').hide();return false;"
                          />
                          <label for="choose_">选中</label>
                        </td>
                        <td>
                          当前版本：
                          <strong>
                            <%=CurrentVersion%>
                          </strong>
                          <br /> 新版本：
                          <strong id="phStep2_newVersionLast"></strong>
                        </td>
                        <td id="phStep2_newVersionNotes">

                        </td>
                        <td class="text-center" id="phStep2_newVersionDate">

                        </td>
                        <td class="text-center">
                          <a id="phStep2_newVersionLink" href="javascript:;" target="_blank">发行说明</a>
                        </td>
                      </tr>

                    </tbody>
                  </table>
                </div>
              </div>
            </div>

            <div id="phStep2_newVersionNext" style="display: none">

              <hr />

              <div class="text-center">
                <input id="btnStep2" type="button" value="下一步" class="btn btn-primary">
              </div>

            </div>

          </div>

          <!-- step 3 place -->
          <div id="phStep3" style="display: none">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="jumbotron text-center">
              <img src="./pic/animated_loading.gif" />
              <br />
              <br />
              <p class="lead">正在下载 SiteServer CMS 升级包，可能需要几分钟，请稍后...</p>
            </div>

          </div>

          <!-- step 4 place -->
          <div id="phStep4" style="display: none">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="jumbotron text-center">
              <img src="./pic/animated_loading.gif" />
              <br />
              <br />
              <p class="lead">正在升级系统，请稍后...</p>
            </div>

          </div>

          <!-- step 5 place -->
          <div id="phStep5" style="display: none">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">检查更新</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">下载升级包</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级系统</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="alert alert-success" role="alert">
              <h4 class="alert-heading">升级完成！</h4>
              <p>
                恭喜，您已经完成了 SiteServer CMS 的升级
                <a class="btn btn-success m-l-5" href="<%=AdminUrl%>">进入后台</a>
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
    <script type="text/javascript">
      // var validate = window.Page_ClientValidate;
      // $(function () {
      //   $('.btn-primary').click(function () {
      //     if (!validate || validate()) {
      //       $('#main').showLoading();
      //     }
      //     return true;
      //   });
      // });

      var version = '';

      function checkVersion() {
        $.ajax({
          url: "<%=VersionApiUrl%>",
          type: "GET",
          dataType: "json",
          success: function (data) {
            $('#phStep2_loading').hide();

            if (data && data.version && data.version != '<%=CurrentVersion%>') {
              version = data.version;
              $('#phStep2_newVersion').show();

              $('#phStep2_newVersionLast').html(data.version);
              $('#phStep2_newVersionDate').html(data.published);
              if (data.releaseNotes) {
                $('#phStep2_newVersionNotes').html(data.releaseNotes + '<br />');
              }
              $('#phStep2_newVersionLink').attr('href',
                'http://www.siteserver.cn/releasenotes/index.html?version=' +
                data.version);

            } else {
              $('#phStep2_currentVersion').show();
            }
          }
        });
      }

      function downloadPackage() {
        $.ajax({
          url: "<%=DownloadApiUrl%>",
          type: "POST",
          data: {
            pluginId: '<%=PackageId%>',
            version: version
          },
          success: function (data) {
            $('#phStep3').hide();
            $('#phStep4').show();

            updateSystem();
          }
        });
      }

      function updateSystem() {
        $.ajax({
          url: "<%=UpdateApiUrl%>",
          type: "POST",
          data: {
            pluginId: '<%=PackageId%>',
            version: version
          },
          success: function (data) {
            syncDatabase();
          }
        });
      }

      function syncDatabase() {
        $.ajax({
          url: "<%=SyncDatabaseApiUrl%>",
          type: "POST",
          success: function (data) {
            $('#phStep4').hide();
            $('#phStep5').show();
          }
        });
      }

      $(document).ready(function () {
        $('#btnStep1').click(function () {
          $('#phStep1').hide();
          $('#phStep2').show();

          checkVersion();
        });

        $('#btnStep2').click(function () {
          $('#phStep2').hide();
          $('#phStep3').show();

          downloadPackage();
        });
      });
    </script>