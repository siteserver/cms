<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageMain" Trace="False" EnableViewState="false" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<!--
   _____  _  _          _____                               
  / ____|(_)| |        / ____|                              
 | (___   _ | |_  ___ | (___    ___  _ __ __   __ ___  _ __ 
  \___ \ | || __|/ _ \ \___ \  / _ \| '__|\ \ / // _ \| '__|
  ____) || || |_|  __/ ____) ||  __/| |    \ V /|  __/| |   
 |_____/ |_| \__|\___||_____/  \___||_|     \_/  \___||_|   
-->
<html>

<head>
  <link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
  <link href="assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
  <link href="assets/css/components.css" rel="stylesheet" type="text/css" />
  <link href="assets/css/pages.css" rel="stylesheet" type="text/css" />
  <link href="assets/css/menu.css" rel="stylesheet" type="text/css" />
  <link href="images/siteserver_icon.png" rel="icon" type="image/png">
  <meta charset="utf-8">
  <title>SiteServer 管理后台</title>
  <meta http-equiv="X-UA-Compatible" content="IE=edge" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
</head>

<body class="fixed-left widescreen">
  <div id="wrapper">
<header id="topnav">
    <div class="topbar-main">
      <div class="logo">
        <a href="http://www.siteserver.cn" target="_blank" class="logo"><img src="images/siteserver.png" /></a>
        <a href="javascript:;" class="toggle" id="top-toggle"><i class="icon-arrow-left"></i></a>
      </div>
      <ul class="navigation-menu">
        <asp:Literal id="LtlTopMenus" runat="server" />
      </ul>
      <div class="menu-extras">
        <ul class="nav navbar-nav navbar-right pull-right">
          <asp:Literal id="LtlExtras" runat="server" />
          <li class="dropdown">
            <a href="" class="dropdown-toggle waves-effect waves-light profile" data-toggle="dropdown" aria-expanded="false">
              <img src="images/avatar.jpg" alt="user-img" class="img-circle">
              <asp:Literal ID="LtlUserName" runat="server" />
            </a>
            <ul class="dropdown-menu">
              <li><a href="pageUserProfile.aspx" target="right"><i class="ion-person m-r-5"></i> 修改资料</a></li>
              <li><a href="pageUserPassword.aspx" target="right"><i class="ion-ios-refresh-outline m-r-5"></i> 更改密码</a></li>
              <li><a href="logout.aspx"><i class="ion-log-out m-r-5"></i> 退出系统</a></li>
            </ul>
          </li>
        </ul>
      </div>
      <div class="clearfix"></div>
    </div>
  </header>

  <div class="left side-menu">
      <div class="sidebar-inner slimscrollleft">
        <div id="sidebar-menu">
          <ul>
            <bairong:NavigationTree id="NtLeftManagement" title="站点管理" runat="server" />
            <bairong:NavigationTree id="NtLeftFunctions" title="站点插件" runat="server" />
            <li class="text-muted menu-title"></li>
            <li class="text-muted menu-title"></li>
            <li class="text-muted menu-title"></li>
          </ul>
          <div class="clearfix"></div>
        </div>
        <div class="clearfix"></div>
      </div>
    </div>

  <div class="content-page" id="content">
    <iframe id="frmMain" frameborder="0" id="right" name="right" src="pageRight.aspx" style="width:100%"></iframe>    
  </div>
  </div>
</body>
</html>

<script src="assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
<script src="assets/signalR/jquery.signalR-2.2.1.min.js" type="text/javascript"></script>
<script src="assets/layer/layer.min.js" type="text/javascript"></script>
<script src="/signalr/hubs" type="text/javascript"></script>
<script src="inc/script.js" type="text/javascript"></script>
<script src="assets/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>
<script src="assets/jQuery-slimScroll/jquery.slimscroll.min.js" type="text/javascript"></script>

<script type="text/javascript">
  function redirect(url) {
    $('#right').src = url;
  }

  var siteId = <%=PublishmentSystemId%>;

  var create = $.connection.createHub;
  create.client.next = function (total) {
    $('#progress').html(total);
    if (total) {
      create.server.execute(siteId);
    } else {
      setTimeout(function () {
        create.server.execute(siteId);
      }, 5000);
    }
  };
  $.connection.hub.start().done(function () {
    create.server.execute(siteId);
  });

  $(document).ready(function () {
    
    $('#frmMain').height($(window).height() - 70);

    // $('.waves-primary').click(function () {
    //   if ($(this).hasClass('subdrop')) {
    //     $(this).removeClass('subdrop');
    //     $(this).siblings('ul').hide();
    //   } else {
    //     $('.waves-primary').removeClass('subdrop');
    //     $('.list-unstyled').hide();
    //     $(this).addClass('subdrop');
    //     $(this).siblings('ul').show();
    //   }
    // });
    $('.waves-primary').click(function () {
      if ($(this).hasClass('subdrop')) {
        $(this).removeClass('subdrop');
        $(this).siblings('ul').hide();
      } else {
        $(this).addClass('subdrop');
        $(this).siblings('ul').show();
      }
    });

    $('.list-unstyled a').click(function () {
      $('#sidebar-menu li').removeClass('active');
      $(this).parent().addClass('active');
    });

    $('.slimscrollleft').slimScroll({
          height: 'auto',
          position: 'right',
          size: "5px",
          color: '#dcdcdc',
          wheelStep: 5
      });
  });
</script>