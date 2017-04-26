<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Platform.PageUserPassword" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server" autocomplete="off">
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <ul class="nav nav-pills">
            <li><a href="pageUserProfile.aspx">
                修改资料
            </a></li>
            <li class="active"><a href="pageUserPassword.aspx">
                更改密码
            </a></li>
        </ul>

        <div class="popover popover-static">
            <h3 class="popover-title">
                更改密码
            </h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="150">管理员登录名：</td>
                        <td>
                            <asp:Literal ID="UserName" runat="server"></asp:Literal></td>
                    </tr>
                    <tr>
                        <td>当前密码：</td>
                        <td>
                            <!--防止表单的自动填充功能-->
                            <input type="password" style="display: none" />
                            <!--防止表单的自动填充功能-->
                            <asp:TextBox ID="CurrentPassword" runat="server" MaxLength="50" Size="20" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="CurrentPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>新密码：</td>
                        <td>
                            <asp:TextBox ID="NewPassword" runat="server" MaxLength="50" Size="20" TextMode="Password"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="NewPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1"
                                runat="server"
                                ControlToValidate="NewPassword"
                                ValidationExpression="[^']+"
                                ErrorMessage="不能输入单引号"
                                foreColor="red"
                                Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td>重复输入新密码：</td>
                        <td>
                            <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password" MaxLength="50" Size="20"></asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="ConfirmNewPassword" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                            <asp:CompareValidator ID="NewPasswordCompare" runat="server" ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword" Display="Dynamic" ForeColor="red" ErrorMessage=" 两次输入的新密码不一致！请再输入一遍您上面填写的新密码。"></asp:CompareValidator>
                        </td>
                    </tr>
                </table>

                <hr />
                <table class="table noborder">
                    <tr>
                        <td class="center">
                            <asp:Button class="btn btn-primary" OnClick="Submit_Click" runat="server" Text="修 改" />
                        </td>
                    </tr>
                </table>

            </div>
        </div>

    </form>
</body>
</html>
