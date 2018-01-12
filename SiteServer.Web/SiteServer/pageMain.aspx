<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageMain" Trace="False" EnableViewState="false" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <title>SiteServer 管理后台</title>
      <meta http-equiv="X-UA-Compatible" content="IE=edge" />
      <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
      <link href="assets/bootstrap/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
      <link href="assets/css/components.css" rel="stylesheet" type="text/css" />
      <link href="assets/css/pages.css" rel="stylesheet" type="text/css" />
      <link href="assets/css/menu.css" rel="stylesheet" type="text/css" />
      <link href="assets/css/ionicons.min.css" rel="stylesheet" type="text/css" />
      <link href="images/siteserver_icon.png" rel="icon" type="image/png">
    </head>

    <body class="fixed-left widescreen">
      <div id="wrapper">
        <header id="topnav">
          <div class="topbar-main">
            <div class="logo">
              <a href="http://www.siteserver.cn" target="_blank" class="logo">
                <img src="images/siteserver.png" />
              </a>
              <a href="javascript:;" class="toggle" id="top-toggle">
                <i class="icon-arrow-left"></i>
              </a>
            </div>
            <ul class="navigation-menu">
              <asp:Literal id="LtlTopMenus" runat="server" />
            </ul>
            <asp:PlaceHolder id="PhPublishmentSystem" runat="server" visible="false">
              <div class="menu-extras">
                <ul class="nav navbar-nav navbar-right float-right">
                  <li class="dropdown hidden-xs">
                    <asp:Literal id="LtlCreateStatus" runat="server" />
                  </li>
                  <li>
                    <form id="search" role="search" class="navbar-left app-search float-left hidden-xs" action="cms/pagecontentsearch.aspx?publishmentSystemId=<%=PublishmentSystemId%>"
                      target="right" method="get">
                      <input name="publishmentSystemId" type="hidden" value="<%=PublishmentSystemId%>">
                      <input name="keyword" type="text" placeholder="内容搜索..." class="form-control">
                      <a href="javascript:;" onclick="$('#search').submit();">
                        <i class="ion-search"></i>
                      </a>
                    </form>
                  </li>

                </ul>
              </div>
            </asp:PlaceHolder>
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
          <iframe id="frmMain" frameborder="0" id="right" name="right" src="pageRight.aspx" style="width:100%; height: 100%"></iframe>
        </div>
      </div>
    </body>

    </html>

    <script src="assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="assets/signalR/jquery.signalR-2.2.1.min.js" type="text/javascript"></script>
    <script src="assets/layer/layer.min.js" type="text/javascript"></script>
    <script src="/signalr/hubs" type="text/javascript"></script>
    <script src="inc/script.js" type="text/javascript"></script>
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
        $('#frmMain').height($(window).height() - 62);
        $('#frmMain').width($(window).width() - 200);

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

      window.onresize = function (event) {
        $('#frmMain').height($(window).height() - 62);
        $('#frmMain').width($(window).width() - 200);
      };
    </script>
    <!--#include file="inc/foot.html"-->