<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageThirdLoginConfiguration" %>
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
            <h3 class="popover-title">配置登录方式</h3>
            <div class="popover-content">

                <table class="table noborder table-hover">
                    <tr>
                        <td width="180">登录方式名称：</td>
                        <td>
                            <asp:TextBox ID="tbThirdLoginName" runat="server" />
                            <asp:RequiredFieldValidator ControlToValidate="tbThirdLoginName" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="tbThirdLoginName" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                        <td>类型：</td>
                        <td>
                            <asp:Literal ID="ltlThirdLoginType" runat="server" />
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phLoginAuth" Visible="false" runat="server">
                        <tr>
                            <td>第三方认证ID（Key）：</td>
                            <td>
                                <asp:TextBox ID="tbLoginAuthAppKey" runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="tbLoginAuthAppKey" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbLoginAuthAppKey" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                            </td>
                        </tr>
                        <tr>
                            <td>第三方认证秘钥（Sercet）：</td>
                            <td>
                                <asp:TextBox ID="tbLoginAuthAppSercet" runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="tbLoginAuthAppSercet" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbLoginAuthAppSercet" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                            </td>
                        </tr>
                        <tr>
                            <td>第三方回调地址（CallBackUrl）：</td>
                            <td>
                                <asp:TextBox ID="tbLoginAuthCallBackUrl" runat="server" />
                                <asp:RequiredFieldValidator ControlToValidate="tbLoginAuthCallBackUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                                <asp:RegularExpressionValidator runat="server" ControlToValidate="tbLoginAuthCallBackUrl" ValidationExpression="[^']+" ErrorMessage=" *" ForeColor="red" Display="Dynamic" />
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td>第三方回调地址（CallBackUrl）:</td>
                        <td><asp:Literal ID="ltlLoginAuthCallBackUrl" runat="server"></asp:Literal>
                            <br />
                            <span>请将以下这个地址复制到您第三方登录接口的回调地址申请,这样用户将能够通过第三方登录自动注册网站</span>
                        </td>
                    </tr>
                    <tr>
                        <td>登录方式描述：</td>
                        <td>
                            <bairong:UEditor ID="breDescription" runat="server"></bairong:UEditor>
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

