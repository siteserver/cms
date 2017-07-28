<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.PageConfigurationAdministrator" %>
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
        <bairong:Alerts runat="server"></bairong:Alerts>

        <div class="popover popover-static">
            <h3 class="popover-title">管理员设置</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                      <td width="260">管理员用户名最小长度：</td>
                      <td>
                        <asp:TextBox ID="tbLoginUserNameMinLength" class="input-mini" runat="server"></asp:TextBox>
                        <span class="gray">0代表不限制</span>
                      </td>
                    </tr>
                    <tr>
                      <td>管理员密码最小长度：</td>
                      <td>
                        <asp:TextBox ID="tbLoginPasswordMinLength" class="input-mini" runat="server"></asp:TextBox>
                        <span class="gray">0代表不限制</span>
                      </td>
                    </tr>
                    <tr>
                      <td>管理员密码规则限制：</td>
                      <td>
                        <asp:DropDownList ID="ddlLoginPasswordRestriction" runat="server"></asp:DropDownList>
                      </td>
                    </tr>

                    <tr>
                        <td>是否开启登录失败锁定：</td>
                        <td>
                            <asp:RadioButtonList ID="rblIsLoginFailToLock" OnSelectedIndexChanged="rblIsLoginFailToLock_SelectedIndexChanged" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phFailToLock" runat="server">
                        <tr>
                            <td>登录失败次数锁定：</td>
                            <td>
                                <asp:TextBox ID="tbLoginFailToLockCount" runat="server"></asp:TextBox> 次
                                <asp:RequiredFieldValidator ControlToValidate="tbLoginFailToLockCount" runat="server" ErrorMessage="*" foreColor="Red"></asp:RequiredFieldValidator>
                                <br />
                                <span class="gray">一旦登录失败达到指定次数之后管理员就会被锁定</span>
                            </td>
                        </tr>
                        <tr>
                            <td>管理员锁定类型：</td>
                            <td>
                                <asp:DropDownList ID="ddlLoginLockingType" OnSelectedIndexChanged="ddlLoginLockingType_SelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
                            </td>
                        </tr>
                        <asp:PlaceHolder ID="phLoginLockingHours" runat="server">
                            <tr>
                                <td>管理员锁定时间：</td>
                                <td>
                                    <asp:TextBox ID="tbLoginLockingHours" runat="server"></asp:TextBox> 小时
                                    <asp:RequiredFieldValidator ControlToValidate="tbLoginLockingHours" runat="server" ErrorMessage="*" foreColor="Red"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </asp:PlaceHolder>
                    </asp:PlaceHolder>

                    <tr>
                        <td>管理员是否可以查看其他人添加的内容：</td>
                        <td>
                            <asp:RadioButtonList ID="rblIsViewContentOnlySelf" runat="server" class="radiobuttonlist" RepeatDirection="Horizontal"></asp:RadioButtonList>
                            <span class="gray">注意：超级管理员、站点管理员、具有审核权限的管理员，此设置无效。</span>
                        </td>
                    </tr>
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
