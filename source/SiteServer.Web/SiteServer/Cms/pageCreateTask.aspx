<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageCreateTask" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>

<html>
<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
    <script>
        $(function () {
            var inteval = $("#<%=ddlInteval.ClientID%>").val();
            setInterval(function () {
                $("#<%=btnFresh.ClientID%>").click();
            }, inteval * 1000);
        });
    </script>
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server">
        <asp:Literal ID="ltlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <div class="well well-small">
            <div id="contentSearch" style="margin-top: 10px;">
                自动刷新时间：
                <asp:DropDownList ID="ddlInteval" runat="server" OnSelectedIndexChanged="ddlInteval_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem Value="5" Text="5秒"></asp:ListItem>
                    <asp:ListItem Value="10" Text="10秒"></asp:ListItem>
                    <asp:ListItem Value="20" Text="20秒"></asp:ListItem>
                </asp:DropDownList>
                <asp:Button class="btn btn-primary" runat="server" ID="btnFresh" Text="刷新" OnClick="btnFresh_Click" />
            </div>
        </div>
        <div class="popover popover-static">
            <h3 class="popover-title">生成队列</h3>
            <div class="popover-content">
                <table class="table table-bordered table-hover">
                    <tr class="info thead">
                        <td class="center" style="width: 5%">任务序列</td>
                        <td class="center" style="width: 20%">描述</td>
                        <td class="center" style="width: 10%">已完成</td>
                        <td class="center" style="width: 15%">进度</td>
                        <td class="center" style="width: 15%">状态</td>
                        <td class="center" style="width: 10%">创建时间</td>
                        <td class="center" style="width: 5%">前置任务</td>
                        <td class="center" style="width: 10%">创建人</td>
                        <td class="center" style="width: 10%">操作</td>
                    </tr>
                    <asp:Repeater ID="rptContents" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td class="center">
                                    <asp:Literal ID="ltlItemIndex" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlSummary" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlProcessCount" runat="server"></asp:Literal>
                                </td>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlProcess" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlState" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlAddDate" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlQueuing" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlUserName" runat="server"></asp:Literal>
                                </td>
                                <td class="center">
                                    <asp:Literal ID="ltlCancel" runat="server"></asp:Literal>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
        </div>
    </form>
</body>
</html>
