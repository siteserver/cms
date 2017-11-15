<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalCardCredits" Trace="false" %>
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
        <asp:Button id="BtnSubmit" UseSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" Style="display: none" />
        <bairong:Alerts runat="server"></bairong:Alerts>
        <bairong:Code Type="ajaxupload" runat="server" />

        <table class="table table-noborder">
            <asp:PlaceHolder ID="PhKeyWord" runat="server">
                <tr>
                    <td width="130">
                        <bairong:Help HelpText="选择会员卡" Text="选择会员卡：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:DropDownList ID="DdlCard" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td width="130">
                        <bairong:Help HelpText="选择方式" Text="选择方式：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:DropDownList ID="DdlKeyWordType" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>
                        <bairong:Help HelpText="卡号/手机号" Text="卡号/手机号：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:TextBox ID="TbKeyWord" MaxLength="50" Size="20" runat="server" />
                        <asp:RequiredFieldValidator
                            ControlToValidate="TbKeyWord"
                            ErrorMessage=" *" ForeColor="red"
                            Display="Dynamic"
                            runat="server" /></td>
                </tr>
            </asp:PlaceHolder>
            <tr>
                <td>
                    <bairong:Help HelpText="选择操作" Text="选择操作：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:DropDownList ID="DdlOperatType" runat="server"></asp:DropDownList>
                </td>
            </tr>

            <tr>
                <td>
                    <bairong:Help HelpText="积分值" Text="积分值：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:TextBox ID="TbCredits" MaxLength="50" Size="20" runat="server" />
                    <asp:RequiredFieldValidator
                        ControlToValidate="TbCredits"
                        ErrorMessage=" *" ForeColor="red"
                        Display="Dynamic"
                        runat="server" />
                    <asp:RegularExpressionValidator
                        runat="server"
                        ControlToValidate="TbCredits"
                        ValidationExpression="^[0-9]*$"
                        ErrorMessage="不合法" ForeColor="red"
                        Display="Dynamic" />
                </td>
            </tr>

        </table>

    </form>
</body>
</html>

