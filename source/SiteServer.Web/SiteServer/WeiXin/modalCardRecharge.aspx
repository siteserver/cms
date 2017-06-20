<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalCardRecharge" Trace="false" %>
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
                        <asp:DropDownList ID="DdlCard" runat="server" OnSelectedIndexChanged="Refrush" AutoPostBack="true"></asp:DropDownList>
                    </td>
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
                    <bairong:Help HelpText="充值金额" Text="充值金额：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:TextBox ID="TbRechargeAmount" MaxLength="50" Size="20" runat="server" />
                    <asp:RequiredFieldValidator
                        ControlToValidate="TbRechargeAmount"
                        ErrorMessage=" *" ForeColor="red"
                        Display="Dynamic"
                        runat="server" />
                    <asp:RegularExpressionValidator
                        runat="server"
                        ControlToValidate="TbRechargeAmount"
                        ValidationExpression="^(([1-9]\d*)|\d)(\.\d{1,2})?$"
                        ErrorMessage="不合法" ForeColor="red"
                        Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>
                    <bairong:Help HelpText="操作员" Text="操作员：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:DropDownList ID="DdlOperator"  runat="server"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <bairong:Help HelpText="备 注" Text="备 注：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:TextBox ID="TbDescription" TextMode="MultiLine" Columns="20" runat="server" />
                </td>
            </tr>
        </table>

    </form>
</body>
</html>

