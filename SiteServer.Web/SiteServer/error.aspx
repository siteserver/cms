<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageError"%>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <title>SiteServer 管理后台</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/core.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/components.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/pages.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/menu.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/responsive.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
    <link href="images/siteserver_icon.png" rel="icon" type="image/png">
  </head>

  <body>
    <form class="container m-t-20" runat="server">

      <div class="card">
        <div class="card-header" style="border-top: 3px solid #ef5350 !important;border-radius: 3px;">
          <strong>错误信息</strong>
        </div>
        <div class="card-body">
          <blockquote class="blockquote mb-0">
            <p style="word-wrap: break-word;">
              <asp:Literal id="LtlMessage" runat="server" />
            </p>
            <p>
              <asp:Literal id="LtlStackTrace" runat="server" />
            </p>
          </blockquote>
        </div>
        <div class="card-footer text-muted">
          <span class="m-r-5">如果错误信息为列名无效，可以尝试升级系统，以确保数据库字段一致性</span>
        </div>
      </div>

    </form>
  </body>

  </html>
  <!--#include file="inc/foot.html"-->