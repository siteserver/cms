<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageAppointmentContent" %>
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
        <asp:Literal ID="LtlBreadCrumb" runat="server" />
        <bairong:Alerts runat="server" />

        <script type="text/javascript">
            $(document).ready(function () {
                loopRows(document.getElementById('contents'), function (cur) { cur.onclick = chkSelect; });
                $(".popover-hover").popover({ trigger: 'hover', html: true });
            });
        </script>

        <table id="contents" class="table table-bordered table-hover">
            <tr class="info thead">
                <td width="20"></td>
                <td>预约名称</td>
                <td>姓名</td>
                <td>电话</td>
                <td>邮箱</td>
                <td>预约时间</td>
                <td>预约状态</td>
                <td>留言</td>
                <%--<asp:Literal ID="LtlExtendTitle" runat="server"></asp:Literal>--%>
                <td></td>
                <td></td>
                <td width="20">
                    <input type="checkbox" onclick="selectRows(document.getElementById('contents'), this.checked);" /></td>
            </tr>
            <asp:Repeater ID="RptContents" runat="server">
                <ItemTemplate>
                    <tr>
                        <td class="center">
                            <asp:Literal ID="LtlItemIndex" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlAppointementTitle" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlRealName" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlMobile" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlEmail" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlAddDate" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlStatus" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="LtlMessage" runat="server"></asp:Literal>
                        </td>
                        <%-- <asp:Literal ID="LtlExtendVal" runat="server"></asp:Literal>--%>
                        <td class="center">
                            <asp:Literal ID="LtlEditUrl" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlSelectUrl" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        </table>
        <bairong:SqlPager ID="SpContents" runat="server" class="table table-pager" />

        <ul class="breadcrumb breadcrumb-button">
            <asp:Button class="btn btn-success" id="BtnHandle" Text="预约处理" runat="server" />
            <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
            <asp:Button class="btn" id="BtnExport" Text="导出CSV" runat="server" />
            <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
        </ul>

    </form>
</body>
</html>
