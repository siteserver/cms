<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageUserConfigLogin" %>
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
            <h3 class="popover-title">用户登录设置</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="200">是否记录用户IP：</td>
                        <td>
                            <asp:RadioButtonList ID="rblIsRecordIP" runat="server" RepeatDirection="Horizontal">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td>是否记录登录来源：</td>
                        <td>
                            <asp:RadioButtonList ID="rblIsRecordSource" runat="server" RepeatDirection="Horizontal">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td>是否开启失败锁定：</td>
                        <td>
                            <asp:RadioButtonList ID="rblIsFailToLock" OnSelectedIndexChanged="rblIsFailToLock_SelectedIndexChanged" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phFailToLock" runat="server">
                        <tr>
                            <td>失败次数锁定：</td>
                            <td>
                                <asp:TextBox ID="loginFailCount" runat="server"></asp:TextBox> 次
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" ControlToValidate="loginFailCount" runat="server" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                <br />
                                <span class="gray">一旦登录失败达到指定次数之后用户就会被锁定</span>
                            </td>
                        </tr>
                        <tr>
                            <td>用户锁定类型：</td>
                            <td>
                                <asp:DropDownList ID="ddlLockType" OnSelectedIndexChanged="ddlLockType_SelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="phLockingTime" runat="server">
                            <tr>
                                <td>锁定时间：</td>
                                <td>
                                    <asp:TextBox ID="lockingTime" runat="server"></asp:TextBox> 小时
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="lockingTime" runat="server" ErrorMessage="*" ForeColor="Red"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>
                </table>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" ID="Submit" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>
