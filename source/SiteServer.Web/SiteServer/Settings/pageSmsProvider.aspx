<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSmsProvider" %>
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
            <h3 class="popover-title">短信服务商管理</h3>
            <div class="popover-content">
                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td>短信服务商</td>
                        <td width="80">是否启用</td>
                        <td width="80"></td>
                        <td width="50"></td>
                        <td width="50"></td>
                    </tr>
                    <asp:Repeater ID="RptContents" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Literal ID="ltlName" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlIsEnabled" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlTemplates" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlIsEnabledUrl" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlConfigUrl" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
