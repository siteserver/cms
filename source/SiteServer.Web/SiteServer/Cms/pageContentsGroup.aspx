<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentsGroup" %>
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

        <div class="well well-small">
            <table class="table table-noborder">
                <tr>
                    <td>
                        <asp:Literal ID="ltlContentGroupName" runat="server"></asp:Literal>
                    </td>
                </tr>
            </table>
        </div>

        <table class="table table-bordered table-hover">
            <tr class="info thead">
                <td>内容标题(点击查看)
                </td>
                <td width="250">所属栏目
                </td>
                <td width="200">作者
                </td>
                <td width="100">添加日期
                </td>
                <td width="100">状态
                </td>
                <td></td>
                <td></td>
            </tr>
            <asp:Repeater ID="rptContents" runat="server">
                <ItemTemplate>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlItemTitle" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="ltlItemChannel" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="ltlItemAuthor" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlItemAddDate" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlItemStatus" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlItemEditUrl" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="ltlItemDeleteUrl" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>

        <bairong:SqlPager ID="spContents" runat="server" class="table table-pager" />

        <ul class="breadcrumb breadcrumb-button">
            <asp:Button class="btn" Text="返 回" OnClick="Return_OnClick" runat="server" />
        </ul>

    </form>
</body>
</html>

