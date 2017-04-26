<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCheckState" Trace="false" %>
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
                    <asp:Literal ID="ltlTitle" runat="server"></asp:Literal></td>
            </tr>
            <tr>
                <td>审核状态：</td>
                <td>
                    <asp:Literal ID="ltlState" runat="server"></asp:Literal></td>
            </tr>
        </table>

        <asp:PlaceHolder ID="phCheckReasons" runat="server" Visible="false">
            <table class="table table-noborder table-hover">
                <tr class="info">
                    <td>审核人</td>
                    <td>审核时间</td>
                    <td>原因</td>
                </tr>
                <asp:Repeater ID="rpContents" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
                            <td>
                                <asp:Literal ID="ltlCheckDate" runat="server"></asp:Literal></td>
                            <td>
                                <asp:Literal ID="ltlReasons" runat="server"></asp:Literal></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </asp:PlaceHolder>

        <asp:PlaceHolder ID="phCheck" runat="server">
            <ul class="breadcrumb center">
                <asp:Button class="btn btn-success" ID="Check" Text="审 核" OnClick="Submit_OnClick" runat="server" />
            </ul>
        </asp:PlaceHolder>

    </form>
</body>
</html>
