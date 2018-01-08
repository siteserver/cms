<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageInitialization" Trace="False" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
    <title>SiteServer 管理后台</title>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/components.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/pages.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/menu.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
    <link href="images/siteserver_icon.png" rel="icon" type="image/png">
    <!--防止csrf start-->
    <style id="antiClickjack">
      body {
        display: none !important;
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
    <div class="wrapper-page">
      <div class="row">
        <div class="col-sm-12 text-center">
          <div class="home-wrapper">
            <br />
            <br />
            <br />
            <br />
            <img src="assets/layer/skin/default/xubox_loading0.gif" />
            <div class="help-block">载入中，请稍候...</div>
            <asp:Literal ID="LtlContent" runat="server"></asp:Literal>
          </div>
        </div>
      </div>
    </div>
  </body>

  </html>