<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageError"%>
  <!DOCTYPE html>
  <html>

  <head>
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/core.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/components.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/pages.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/menu.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/responsive.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="images/siteserver_icon.png" rel="icon" type="image/png">
    <meta charset="utf-8">
    <title>SiteServer 管理后台</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
  </head>

  <body>
    <form class="container m-t-20" runat="server">

      <div class="panel panel-border panel-danger">
        <div class="panel-heading">
          <h3 class="panel-title">错误信息</h3>
        </div>
        <div class="panel-body">
          <p style="word-wrap: break-word;">
            <asp:Literal id="LtlMessage" runat="server" />
          </p>
          <p>
            <asp:Literal id="LtlStackTrace" runat="server" />
          </p>
        </div>
        <div class="panel-footer">
          <span class="m-r-5">如果错误信息为列名无效，可以尝试升级系统，以确保数据库字段一致性</span>
          <a href="upgrade/default.aspx" target="_top" class="btn btn-primary">立即升级</a>
        </div>
      </div>

    </form>
  </body>

  </html>