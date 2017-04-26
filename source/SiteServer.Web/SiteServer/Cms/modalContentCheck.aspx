<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentCheck" Trace="false" %>
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
        <asp:Button ID="btnSubmit" UseSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" Style="display: none" />
        <bairong:Alerts runat="server"></bairong:Alerts>

        <table class="table table-noborder table-hover">
            <tr>
                <td width="120">内容标题：</td>
                <td>
                    <asp:Literal ID="ltlTitles" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>设置审核状态：</td>
                <td>
                    <asp:RadioButtonList ID="rblCheckType" runat="server"></asp:RadioButtonList></td>
            </tr>
            <tr>
                <td>转移到栏目：</td>
                <td>
                    <asp:DropDownList ID="ddlTranslateNodeID" runat="server"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>原因：</td>
                <td>
                    <asp:TextBox ID="tbCheckReasons" TextMode="MultiLine" Width="98%" Rows="3" runat="server" />
                </td>
            </tr>
        </table>

    </form>
</body>
</html>
