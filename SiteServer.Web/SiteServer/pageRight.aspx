<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageRight" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <title>SiteServer 管理后台</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/core.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/components.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/pages.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/menu.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/responsive.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
    <style>
      body {
        padding: 20px 0;
      }
    </style>
  </head>

  <body>
    <form class="m-l-15 m-r-15" runat="server">

      <div class="row">
        <div class="col-sm-12">
          <h4 class="page-title">
            欢迎使用 SiteServer 管理后台
          </h4>
        </div>
      </div>

      <div class="card-box">
        <div class="widget-chart text-center">

          <ul class="list-inline m-t-15">
            <li>
              <h5 class="text-muted">当前版本</h5>
              <h4 class="m-b-0">
                <asp:Literal ID="LtlVersionInfo" runat="server" />
              </h4>
            </li>
            <li>
              <h5 class="text-muted">最近升级时间</h5>
              <h4 class="m-b-0">
                <asp:Literal ID="LtlUpdateDate" runat="server"></asp:Literal>
              </h4>
            </li>
            <li>
              <h5 class="text-muted">上次登录时间</h5>
              <h4 class="m-b-0">
                <asp:Literal ID="LtlLastLoginDate" runat="server"></asp:Literal>
              </h4>
            </li>
          </ul>
        </div>
      </div>

      <div class="card-box" id="checkbox" style="display: none">
        <div class="header-title m-t-0">待审核内容</div>
        <p class="text-muted m-b-25 font-13">
          共有
          <span id="checkTotalCount" style="color:#f00"></span> 篇内容待审核
        </p>

        <div class="table-responsive">
          <table class="table">
            <tbody id="checkList">

            </tbody>
          </table>
        </div>
      </div>

    </form>

  </body>

  </html>
  <script src="assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
  <script src="assets/sweetalert/sweetalert.min.js" type="text/javascript"></script>
  <script src="assets/layer/layer.min.js" type="text/javascript"></script>
  <script type="text/javascript">
    $(function () {
      $.ajax({
        url: "<%=SiteCheckListApiUrl%>",
        type: "GET",
        dataType: "json",
        success: function (data) {
          renderList(data)
        }
      });
    })

    function renderList(list) {
      if (!list || list.length == 0) return;
      var html = '';
      var totalCount = 0;
      for (i = 0; i < list.length; i++) {
        html += '<tr><td><a href="' + list[i].url + '">' + list[i].siteName + ' 有 <span style="color:#f00">' + list[i]
          .count + '</span> 篇</a></td></tr>';
        totalCount += list[i].count;
      }
      $('#checkTotalCount').text(totalCount);
      $('#checkList').html(html);
      $('#checkbox').show();
    }
  </script>
  <!--#include file="inc/foot.html"-->