<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalCardEntitySnAdd" Trace="false" %>
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
             <tr>
                <td>卡号：</td>
                <td>
                    <asp:TextBox ID="TbCardSN" MaxLength="50" runat="server" />
                    <asp:RequiredFieldValidator
                        ControlToValidate="TbCardSN"
                        ErrorMessage=" *" ForeColor="red"
                        Display="Dynamic"
                        runat="server" />
                </td>
            </tr>
             <tr>
                <td>姓名：</td>
                <td>
                    <asp:TextBox ID="TbUserName" MaxLength="50" runat="server" />
                    <asp:RequiredFieldValidator
                        ControlToValidate="TbUserName"
                        ErrorMessage=" *" ForeColor="red"
                        Display="Dynamic"
                        runat="server" />
                </td>
            </tr>
            <tr>
                <td>余额：</td>
                <td>
                    <asp:TextBox ID="TbAmount" MaxLength="50" runat="server" />
                    <asp:RequiredFieldValidator
                        ControlToValidate="TbAmount"
                        ErrorMessage=" *" ForeColor="red"
                        Display="Dynamic"
                        runat="server" />
                    <asp:RegularExpressionValidator
                        runat="server"
                        ControlToValidate="TbAmount"
                        ValidationExpression="^(([1-9]\d*)|\d)(\.\d{1,2})?$"
                        ErrorMessage="不合法" ForeColor="red"
                        Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>积分：</td>
                <td>
                    <asp:TextBox ID="TbCredits" MaxLength="50" runat="server" />
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
            <tr>
                <td>手机号码：</td>
                <td>
                    <asp:TextBox ID="TbMobile" runat="server"></asp:TextBox>
                    <asp:RegularExpressionValidator ControlToValidate="TbMobile"
                        ValidationExpression="^(13|15|18)\d{9}$"
                        ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                </td>
            </tr>
            <tr>
                <td>电子邮箱：</td>
                <td>
                    <asp:TextBox ID="TbEmail" runat="server"></asp:TextBox>
                    <asp:RegularExpressionValidator ControlToValidate="TbEmail"
                        ValidationExpression="(\w[0-9a-zA-Z_-]*@(\w[0-9a-zA-Z-]*\.)+\w{2,})"
                        ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
                </td>
            </tr>
            <tr>
                <td>详细地址：</td>
                <td>
                    <asp:TextBox ID="TbAddress" MaxLength="50" runat="server" />
                    <asp:RequiredFieldValidator
                        ControlToValidate="TbAddress"
                        ErrorMessage=" *" ForeColor="red"
                        Display="Dynamic"
                        runat="server" />
                </td>
            </tr>
        </table>

    </form>
</body>
</html>

