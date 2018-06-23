<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageMain" Trace="False" EnableViewState="false" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
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
      <link href="assets/icons/favicon.png" rel="icon" type="image/png">
      <script src="assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
      <script type="text/javascript">
        if (window.top != self) {
          window.top.location = self.location;
        }
  
        function redirect(url) {
          $('#right').src = url;
        }
  
        var isDesktop = $(window).width() > 1010;
        var contentMargin = isDesktop ? 200 : 0;
        var isMenuWin = false;
  
        function openMenu() {
          isMenuWin = true;
          !isDesktop && $('#leftMenu').show();
        }
  
        function closeMenu() {
          isMenuWin = false;
          !isDesktop && $('#leftMenu').hide();
        }
  
        function toggleMenu() {
          if (isDesktop) {
            contentMargin = contentMargin === 200 ? 0 : 200;
            onresize();
          } else {
            isMenuWin = !isMenuWin;
            if (isMenuWin) {
              openMenu();
            } else {
              closeMenu();
            }
          }
        }
      </script>
    </head>

    <body class="fixed-left widescreen" style="background-color: #eee">

      <div id="wrapper">
        <header id="topnav">
          <div class="topbar-main">
            <div class="logo">
              <a href="http://www.siteserver.cn" target="_blank" class="logo">
                <img src="assets/icons/logo.png" />
              </a>
            </div>
            <asp:Literal id="LtlTopMenus" runat="server" />
            <asp:PlaceHolder id="PhSite" runat="server" visible="false">
              <div class="menu-extras">
                <ul class="nav navbar-nav navbar-right float-right">
                  <li id="newVersion" class="dropdown hidden-xs" style="display: none">
                    <a href="javascript:;" onclick="$('#newVersionCard').toggle();">
                      <i class="ion-email-unread text-warning"></i>
                    </a>
                    <div id="newVersionCard" class="card bg-light text-dark" style="width: 19rem; z-index: 11; position: absolute; display: none">
                      <div class="card-body" style="padding-bottom: 0;">
                        <h5 class="card-title text-success">发现 SiteServer CMS 新版本</h5>
                        <p class="card-text">
                          当前版本：
                          <%=CurrentVersion%>
                            <br /> 最新版本：
                            <span id="newVersionLast"></span>
                            <br /> 发布日期：
                            <span id="newVersionDate"></span>
                            <br />
                            <hr />
                            <span id="newVersionNotes"></span>
                            <a id="newVersionLink" class="card-link" href="javascript:;" target="_blank">查看发行说明</a>
                            <hr />
                            <div class="text-center">
                              <a href="<%=UpdateSystemUrl%>" class="card-link btn btn-primary">立即升级</a>
                              <a href="javascript:;" onclick="$('#newVersionCard').hide();" class="card-link btn btn-secondary">稍后再说</a>
                            </div>
                        </p>
                      </div>
                    </div>
                  </li>
                  <li class="dropdown hidden-xs">
                    <asp:Literal id="LtlCreateStatus" runat="server" />
                  </li>
                  <li>
                    <form id="search" role="search" class="navbar-left app-search float-left hidden-xs" action="cms/pagecontentsearch.aspx?siteId=<%=SiteId%>"
                      target="right" method="get">
                      <input name="siteId" type="hidden" value="<%=SiteId%>">
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

        <div id="leftMenu" class="left side-menu">
          <div class="sidebar-inner slimscrollleft">
            <div id="sidebar-menu">
              <ul>
                <ctrl:NavigationTree id="NtLeftManagement" title="站点管理" runat="server" />
                <ctrl:NavigationTree id="NtLeftFunctions" title="站点插件" runat="server" />
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

    
    <script src="assets/signalR/jquery.signalR-2.2.2.min.js" type="text/javascript"></script>
    <script src="assets/layer/layer.min.js" type="text/javascript"></script>
    <script src="<%=SignalrHubsUrl%>" type="text/javascript"></script>
    <script src="inc/script.js" type="text/javascript"></script>
    <script src="assets/jQuery-slimScroll/jquery.slimscroll.min.js" type="text/javascript"></script>
    <script src="assets/js/apiUtils.js"></script>
    <script src="assets/js/compareversion.js"></script>

    <script type="text/javascript">
      var siteId = <%=SiteId%>;
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

      window.onresize = function (event) {
        isDesktop = $(window).width() > 1010;
        if (!isDesktop) {
          $('#topMenus').hide();
          $('#leftMenu').width('100%').hide();
        } else {
          $('#leftMenu').width(200);
          $('#topMenus').show();
        }
        $('#frmMain').height($(window).height() - 62);
        $('#frmMain').width($(window).width() - contentMargin);
        $('#content').css({
          marginLeft: contentMargin + "px"
        });
        if (contentMargin === 200) {
          $('#btnLeftMenu').hide();
          $("#leftMenu").show();
        } else {
          $('#btnLeftMenu').show();
          $("#leftMenu").hide();
        }
      };

      var ssApi = new apiUtils.Api();
      var downloadApi = new apiUtils.Api('<%=DownloadApiUrl%>');
      var isNightly = <%=IsNightly%>;
      var version = '<%=Version%>';
      var packageIdSsCms = '<%=PackageIdSsCms%>';
      var packageList = <%=PackageList%>;
      var packageIds = [packageIdSsCms];
      for (var i = 0; i < packageList.length; i++) {
        var package = packageList[i];
        packageIds.push(package.id);
      }
      var updatePackages = 0;

      function packageUpdates() {
        ssApi.get({
          isNightly: isNightly,
          version: version,
          $filter: "id in '" + packageIds.join(',') + "'"
        }, function (err, res) {
          if (err || !res || !res.value) return;

          for (var i = 0; i < res.value.length; i++) {
            var package = res.value[i];
            if (!package || !package.version) continue;

            if (package.id == packageIdSsCms) {
              packageDownload(package);
            } else {
              var installedPackages = $.grep(packageList, function (e) {
                return e.id == package.id;
              });
              if (installedPackages.length == 1) {
                var installedPackage = installedPackages[0];
                if (installedPackage.version) {
                  if (compareversion(installedPackage.version, package.version) == -1) {
                    updatePackages++;
                  }
                } else {
                  updatePackages++;
                }
              }
            }
          }

          if (updatePackages > 0) {
            $('#updatePackagesLink').html(updatePackages);
            $('#updatePackagesLink').show();
          }
        }, 'packages');
      }

      function packageDownload(package) {
        if (compareversion('<%=CurrentVersion%>', package.version) != -1) return;

        var major = package.version.split('.')[0];
        var minor = package.version.split('.')[1];
        var updatesUrl = 'http://www.siteserver.cn/updates/v' + major + '_' + minor + '/index.html';
        $('#newVersionLink').attr('href', updatesUrl);
        $('#newVersionLast').html(package.version);
        $('#newVersionDate').html(package.published);
        if (package.releaseNotes) {
          $('#newVersionNotes').html(package.releaseNotes + '<br />');
        }

        downloadApi.post({
          packageId: packageIdSsCms,
          version: package.version
        }, function (err, res) {
          if (!err && res) {
            $('#newVersion').show();
          }
        });
      }

      $(document).ready(function () {
        onresize();

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

        if ('<%=IsConsoleAdministrator%>' === 'False' || '<%=CurrentVersion%>' === '0.0.0-dev') return;

        packageUpdates();
      });
    </script>
    <!--#include file="inc/foot.html"-->