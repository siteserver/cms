<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageSyncDatabase" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <title>SiteServer CMS 数据库升级向导</title>
      <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
      <link href="./assets/icons/favicon.png" rel="icon" type="image/png">
      <link href="./assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/core.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/components.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/pages.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/menu.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/responsive.css" rel="stylesheet" type="text/css" />
      <link href="./assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
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
            SiteServer CMS 数据库升级向导
          </h4>
          <p class="text-muted m-b-25 font-13">
            欢迎来到SiteServer CMS 数据库升级向导！
          </p>

          <ctrl:alerts runat="server" />

          <!-- step 1 place -->
          <div id="phStep1">

            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级数据库</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="jumbotron">
              <h4 class="display-5">欢迎来到SiteServer CMS 数据库升级向导！</h4>
              <p class="lead">
                升级向导将逐一检查数据库字段、将数据库结构更新至最新版本。
              </p>
            </div>

            <hr />

            <div class="text-center">
              <input id="btnStep1" type="button" value="开始升级" class="btn btn-primary">
            </div>

          </div>

          <!-- step 2 place -->
          <div id="phStep2" style="display: none">
            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级数据库</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="jumbotron text-center">
              <img src="./pic/animated_loading.gif" />
              <br />
              <br />
              <p class="lead">正在升级数据库，请稍后...</p>
            </div>
          </div>

          <!-- step 3 place -->
          <div id="phStep3" style="display: none">
            <ul class="nav nav-pills nav-fill bg-muted m-b-20">
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级准备</a>
              </li>
              <li class="nav-item">
                <a class="nav-link" href="javascript:;">升级数据库</a>
              </li>
              <li class="nav-item active">
                <a class="nav-link" href="javascript:;">升级完成</a>
              </li>
            </ul>

            <div class="alert alert-success" role="alert">
              <h4 class="alert-heading">数据库升级完成！</h4>
              <p>
                恭喜，您已经完成了 SiteServer CMS 数据库的升级
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
    <script src="./assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script type="text/javascript">
      function updateDatabase() {
        $.ajax({
          url: "<%=UpdateDatabaseApiUrl%>",
          type: "POST",
          success: function (data) {
            $('#phStep2').hide();
            $('#phStep3').show();
          }
        });
      }

      $(document).ready(function () {
        $('#btnStep1').click(function () {
          $('#phStep1').hide();
          $('#phStep2').show();

          updateDatabase();
        });
      });
    </script>
    <!--#include file="./inc/foot.html"-->