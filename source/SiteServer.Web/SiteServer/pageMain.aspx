<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.PageMain" Trace="False" EnableViewState="false" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <link href="assets/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/components.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/icons.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/pages.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/menu.css" rel="stylesheet" type="text/css" />
    <link href="assets/css/responsive.css" rel="stylesheet" type="text/css" />
    <link href="assets/font-awesome/css/font-awesome.css" rel="stylesheet">
    <link href="css/style.css" rel="stylesheet" type="text/css" media="all" />
    <link href="images/siteserver_icon.png" rel="icon" type="image/png" >

    <script src="assets/jquery/jquery-1.9.1.min.js" type="text/javascript"></script>
    <script src="assets/signalR/jquery.signalR-2.2.1.min.js" type="text/javascript"></script>
    <script src="assets/layer/layer.min.js" type="text/javascript"></script>
    <script src="/signalr/hubs" type="text/javascript"></script>
    <script src="inc/script.js" type="text/javascript"></script>
    <script src="assets/main.js" type="text/javascript"></script>
    <script src="assets/bootstrap/js/bootstrap.min.js" type="text/javascript"></script>

    <meta charset="utf-8">
    <title>SiteServer 管理后台</title>
    <base target="right" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    
</head>

<body>
<script type="text/javascript">
    $(function () {
        var create = $.connection.createHub;
        create.client.next = function(isWait) {
          if (isWait) {
            $('#progress').hide();
            setTimeout(function() {
              create.server.execute();
            }, 1000);
          } else {
            $('#progress').show();
            create.server.execute();
          }
        };
        $.connection.hub.start().done(function () {
          create.server.execute();
        });

        $('#right').height($(window).height() - 40);
    });

    function redirect(url) {
      $('#right').src = url;
    }
</script>

    <div id="navigation">
        <div class="container-fluid">
            <div>
                <a href="http://www.siteserver.cn" target="_blank" id="siteserver"></a>
                <a href="javascript:;" target="_self" class="toggle-nav" rel="tooltip" data-placement="bottom" title="显示/隐藏左侧菜单"><i class="icon-arrow-left"></i></a>
            </div>
            <ul class='main-nav'>
                <asp:Repeater ID="RptTopMenu" runat="server">
                    <ItemTemplate>
                        <asp:Literal ID="ltlMenuLi" runat="server" />

                        <a href='javascript:;' data-toggle='dropdown' class='dropdown-toggle' data-hover='dropdown' style='<%#Container.ItemIndex ==2 ?"padding-top: 0;": ""%>'>
                            <asp:Literal ID='ltlMenuName' runat='server' />
                            <%#Container.ItemIndex <=1 ? "<span class='caret'></span>" :string.Empty %>
                        </a>

                        <asp:Literal ID="ltlMenues" runat="server" />
                        </li>

                    </ItemTemplate>
                </asp:Repeater>
            </ul>

            <div class="user">

                <div class="dropdown">
                    <a href="#" class="dropdown-toggle user-name" data-toggle="dropdown">
                        <img src="images/avatar.jpg">
                        <span class="name">
                            <asp:Literal ID="LtlUserName" runat="server" /></span>
                        <span class="caret"></span>
                    </a>
                    <ul class="dropdown-menu pull-right">
                        <li><a href="pageRight.aspx"><i class="icon-dashboard"></i> 系统面板</a></li>
                        <li><a href="platform/pageUserProfile.aspx"><i class="icon-user"></i> 修改资料</a></li>
                        <li><a href="platform/pageUserPassword.aspx"><i class="icon-key"></i> 更改密码</a></li>
                    </ul>
                </div>

                <ul class="icon-nav">

                    <li>
                        <form action="logout.aspx" method="post" target="_self">
                            <a href="javascript:;" title="退出系统" onclick="$(this).parent().submit();"><i class="icon-signout"></i></a>
                            <style>
                                .icon-nav li > form {
                                    display: inline;
                                }
                                .icon-nav li > form > a {
                                    padding:11px 10px 9px 10px;display:block;color:#fff;position:relative;
                                }
                                .icon-nav li > form > a:hover {
                                    background:#1d9d74;text-decoration:none;
                                }
                            </style>
                        </form>
                    </li>

                </ul>

            </div>
        </div>
    </div>

    <div id="progress" class="progress" style="margin: 0px;padding: 0;height: 8px;display: none">
      <div role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: 25%;height: 8px" class="progress-bar progress-bar-success progress-bar-striped active"></div>
      <div role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: 25%;height: 8px" class="progress-bar progress-bar-info progress-bar-striped active"></div>
      <div role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: 25%;height: 8px" class="progress-bar progress-bar-warning progress-bar-striped active"></div>
      <div role="progressbar" aria-valuenow="60" aria-valuemin="0" aria-valuemax="100" style="width: 25%;height: 8px" class="progress-bar progress-bar-danger progress-bar-striped active"></div>
    </div>

    <div class="container-fluid" id="content">
        <div class="right">
            <div class="main">
                <iframe frameborder="0" id="right" name="right" src="pageRight.aspx"></iframe>
            </div>
        </div>

        <div id="left">
            <form runat="server">
                <table class="table table-condensed table-hover left-table">
                    <bairong:NodeNaviTree ID="NtLeftMenuSite" runat="server" />
                    <bairong:NavigationTree ID="NtLeftMenuSystem" runat="server" />
                    <tr><td></td></tr>
                </table>
            </form>
        </div>

    </div>
</body>
</html>
