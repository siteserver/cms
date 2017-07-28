<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageSmsProviderAliDaYu" %>
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
            <h3 class="popover-title">配置短信服务商</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="180">短信服务商：</td>
                        <td>
                            <asp:Literal ID="LtlType" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>App Key：</td>
                        <td>
                            <asp:TextBox ID="TbAppKey" width="320" runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="TbAppKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppKey" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td>App Secret：</td>
                        <td>
                            <asp:TextBox ID="TbAppSecret" width="320" runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="TbAppSecret" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAppSecret" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td>短信签名：</td>
                        <td>
                            <asp:TextBox ID="TbSignName" width="320" runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="TbSignName" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbSignName" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
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
