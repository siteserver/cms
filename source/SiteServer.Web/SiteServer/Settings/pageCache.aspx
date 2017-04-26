<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageCache" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server">
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />
        <div class="popover popover-static">
            <h3 class="popover-title">清空缓存日志</h3>
            <div class="popover-content">
                <table class="table noborder table-hover">
                    <tr>
                        <td style="width: 30px;">当前缓存数量：</td>
                        <td style="width: 50px;"><%=CacheCount %> 个</td>
                        <td style="width: 55px;">当前已使用缓存百分比：</td>
                        <td style="width: 230px"><%=CachePercentStr %></td>
                    </tr>
                </table>
            </div>
        </div>

        <table class="table noborder">
            <tr>
                <td class="center">
                    <asp:Button class="btn btn-primary" ID="Submit" Text="清除缓存" OnClick="Submit_OnClick" runat="server" />
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
