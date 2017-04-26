<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageThirdLogin" %>
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
            <h3 class="popover-title">第三方登录管理（已安装）</h3>
            <div class="popover-content">
                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td width="200">登录方式名称</td>
                        <td>登录方式描述</td>
                        <td width="80">是否启用</td>
                        <td width="30"></td>
                        <td width="30"></td>
                        <td width="30"></td>
                        <td width="30"></td>
                        <td width="30"></td>
                    </tr>
                    <asp:Repeater ID="rptInstalled" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Literal ID="ltlThirdLoginName" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="ltlDescription" runat="server"></asp:Literal></td>
                                <td class="center">
                                    <asp:Literal ID="ltlIsEnabled" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:HyperLink ID="hlUpLink" runat="server"><img src="../Pic/icon/up.gif" border="0" alt="上升" /></asp:HyperLink>
                                </td>
                                <td class="center">
                                    <asp:HyperLink ID="hlDownLink" runat="server"><img src="../Pic/icon/down.gif" border="0" alt="下降" /></asp:HyperLink>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlConfigUrl" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlIsEnabledUrl" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
        <div class="popover popover-static">
            <h3 class="popover-title">第三方登录管理（未安装）</h3>
            <div class="popover-content">

                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td width="200">第三方登录名称</td>
                        <td>第三方登录描述</td>
                        <td width="50"></td>
                    </tr>
                    <asp:Repeater ID="rptUnInstalled" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:Literal ID="ltlThirdLoginName" runat="server"></asp:Literal></td>
                                <td>
                                    <asp:Literal ID="ltlDescription" runat="server"></asp:Literal></td>
                                <td class="center">
                                    <asp:Literal ID="ltlInstallUrl" runat="server"></asp:Literal>
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

