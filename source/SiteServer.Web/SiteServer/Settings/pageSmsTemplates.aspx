<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSmsTemplates" %>
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
            <h3 class="popover-title">短信模板管理</h3>
            <div class="popover-content">
                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td>模板类型</td>
                        <td>模板ID</td>
                        <td width="80"></td>
                        <td width="80"></td>
                    </tr>
                    <asp:Repeater ID="RptContents" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Literal ID="ltlType" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlTplId" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlTest" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlEdit" runat="server"></asp:Literal>
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
