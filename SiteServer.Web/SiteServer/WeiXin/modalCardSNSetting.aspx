<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalCardSnSetting" Trace="false" %>
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

        <link href="css/emotion.css" rel="stylesheet">

        <table class="table table-noborder">
            <tr id="IsDisabledRow" runat="server">
                <td width="120">使用状态：</td>
                <td>
                    <asp:DropDownList ID="DdlIsDisabled" class="input-medium" runat="server" />
                </td>
            </tr>
            <tr id="IsBindingRow" runat="server">
                <td width="120">绑定状态：</td>
                <td>
                    <asp:DropDownList ID="DdlIsBinding" class="input-medium" runat="server" />
                </td>
            </tr>
        </table>

    </form>
</body>
</html>

