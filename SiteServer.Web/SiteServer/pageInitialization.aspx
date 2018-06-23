<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageInitialization" Trace="False" %>
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
    <!--防止csrf start-->
    <style id="antiClickjack">
      body {
        display: none !important;
        padding: 20px 0;
      }
    </style>
    <script type="text/javascript">
      if (self === top) {
        var antiClickjack = document.getElementById("antiClickjack");
        antiClickjack.parentNode.removeChild(antiClickjack);
      } else {
        top.location = self.location;
      }
    </script>
    <!--防止csrf end-->
  </head>

  <body>
    <div class="m-l-15 m-r-15">
      <div class="text-center" style="margin-top: 100px" v-bind:style="{ display: recentlyPlugins ? '' : 'none' }">
        <img class="mt-3" src="assets/images/loading.gif" />
        <p class="lead mt-3 text-nowrap">载入中，请稍后...</p>
      </div>
    </div>
    <asp:Literal ID="LtlContent" runat="server"></asp:Literal>
  </body>

  </html>
  <!--#include file="inc/foot.html"-->