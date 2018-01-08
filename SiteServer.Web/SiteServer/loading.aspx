<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageLoading" Trace="False" %>
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
          </div>
        </div>
      </div>
    </div>

  </body>

  </html>
  <script src="assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
  <script language="javascript">
    $(function () {
      var url = "<%=GetRedirectUrl()%>";
      if (url && url.length > 0) {
        setTimeout(function () {
          location.href = url;
        }, 200);
      }
    });
  </script>