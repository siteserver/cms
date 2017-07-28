<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageUserAdd" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
    <style type="text/css">
        .city {
            width: 75px;
        }
    </style>
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->

    <form class="form-inline" runat="server">
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />
        <script type="text/javascript" charset="utf-8" src="../assets/validate.js"></script>
        <div class="popover popover-static operation-area">
            <h3 class="popover-title">
                <asp:Literal ID="LtlPageTitle" runat="server" />
            </h3>
            <div class="popover-content">
                <div class="container-fluid" id="weixinactivate">
                    <div class="row-fluid">
                        <table class="table noborder table-hover">
                            <tr>
                                <td>账号：</td>
                                <td>
                                    <asp:TextBox ID="TbUserName" MaxLength="50" runat="server" />
                                    <asp:RequiredFieldValidator
                                        ControlToValidate="TbUserName"
                                        ErrorMessage=" *" ForeColor="red"
                                        Display="Dynamic"
                                        runat="server" /><span class="gray">（帐号用于登录系统，由字母、数字组成）</span>
                                </td>
                            </tr>
                            <tr>
                                <td>姓名：</td>
                                <td>
                                    <asp:TextBox ID="TbDisplayName" MaxLength="50" runat="server" />
                                    <asp:RequiredFieldValidator
                                        ControlToValidate="TbDisplayName"
                                        ErrorMessage=" *" ForeColor="red"
                                        Display="Dynamic"
                                        runat="server" />
                                </td>
                            </tr>

                            <asp:PlaceHolder ID="PhPassword" runat="server">
                                <tr>
                                    <td>密 码：</td>
                                    <td>
                                        <asp:TextBox ID="TbPassword" TextMode="Password" MaxLength="50" runat="server" />
                                        <asp:RequiredFieldValidator
                                            ControlToValidate="TbPassword"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic"
                                            runat="server" />
                                        <span class="gray"><asp:Literal ID="LtlPasswordTips" runat="server" /></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>确认密码：</td>
                                    <td>
                                        <asp:TextBox ID="tbConfirmPassword" TextMode="Password" MaxLength="50" runat="server" />
                                        <asp:RequiredFieldValidator
                                            ControlToValidate="tbConfirmPassword"
                                            ErrorMessage=" *" ForeColor="red"
                                            Display="Dynamic"
                                            runat="server" />
                                        <asp:CompareValidator ID="tbNewPasswordCompare" runat="server" ControlToCompare="TbPassword" ControlToValidate="tbConfirmPassword" Display="Dynamic" ErrorMessage=" 两次输入的密码不一致！请再输入一遍您上面填写的密码。" ForeColor="red"></asp:CompareValidator>
                                    </td>
                                </tr>
                            </asp:PlaceHolder>
                            <tr>
                                <td>电子邮箱：</td>
                                <td>
                                    <asp:TextBox ID="TbEmail" runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ControlToValidate="TbEmail"
                                        ValidationExpression="(\w[0-9a-zA-Z_-]*@(\w[0-9a-zA-Z_-]*\.)+\w{2,})"
                                        ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>手机号码：</td>
                                <td>
                                    <asp:TextBox ID="TbMobile" runat="server"></asp:TextBox>
                                    <asp:RegularExpressionValidator ControlToValidate="TbMobile"
                                        ValidationExpression="^(13|15|18)\d{9}$"
                                        ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </div>

                    <hr />
                    <table class="table table-noborder">
                        <tr>
                            <td class="center">
                                <asp:Button class="btn btn-primary" Text="确 定" OnClick="Submit_OnClick" runat="server" />
                                <asp:Button class="btn" ID="BtnReturn" Text="返 回" runat="server" />
                            </td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>
    </form>
</body>
</html>
