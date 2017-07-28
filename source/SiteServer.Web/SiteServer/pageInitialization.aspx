<%@ Page Language="c#" Inherits="SiteServer.BackgroundPages.PageInitialization" Trace="False" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <title>系统初始化...</title>
    <bairong:Code Type="JQuery" runat="server" />
    <bairong:Code Type="bootstrap" runat="server" />
    <bairong:Code Type="html5shiv" runat="server" />
    <link rel="stylesheet" href="inc/style.css" type="text/css" />
    <script language="JavaScript">
        if (window.top != self) {
            window.top.location = self.location;
        }
    </script>
    <style type="text/css">
        .well img {
            padding-bottom: 10px;
        }
    </style>
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
    <form class="form-inline" runat="server">
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <div class="well" style="margin-top: 20px;">
            <table class="table table-noborder">
                <tr>
                    <td class="center">
                      <img src="pic/animated_loading.gif" align="absmiddle">
                      &nbsp;
                      正在加载数据，请稍候...
                      <asp:Literal ID="LtlContent" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </div>

    </form>
</body>
</html>
