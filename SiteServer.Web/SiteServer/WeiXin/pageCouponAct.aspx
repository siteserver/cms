<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageCouponAct" %>
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
                <td>活动主题</td>
                <td>关键词</td>
                <td>开始时间</td>
                <td>结束时间</td>
                <td>参与人数</td>
                <td>总浏览量</td>
                <td>是否启用</td>
                <td>关联优惠劵</td>
                <td></td>
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
                        <td>
                            <asp:Literal ID="LtlTitle" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlKeywords" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlStartDate" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlEndDate" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlUserCount" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlPVCount" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlIsEnabled" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlCoupons" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlRelate" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlPreviewUrl" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlEditUrl" runat="server"></asp:Literal>
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
            <asp:Button class="btn btn-success" id="BtnAdd" Text="添加活动" runat="server" />
            <asp:Button class="btn btn-info" id="BtnCouponAdd" Text="添加优惠券" runat="server" />
            <asp:Button class="btn btn-info" id="BtnCoupon" Text="管理优惠券" runat="server" />
            <asp:Button class="btn" id="BtnDelete" Text="删除活动" runat="server" />
        </ul>

    </form>
</body>
</html>
