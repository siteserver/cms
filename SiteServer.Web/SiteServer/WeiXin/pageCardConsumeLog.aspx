<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageCardConsumeLog" %>
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
        <div class="well well-small">
            <table class="table table-noborder">
                <tr>
                    <td>会员卡：<asp:DropDownList ID="DdlCard" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Search_OnClick"></asp:DropDownList>&nbsp;&nbsp;
                        会员卡号：<asp:TextBox ID="TbCardSN" Width="120" runat="server" />&nbsp;&nbsp;
                        用户名：<asp:TextBox ID="TbUserName" Width="120" runat="server" />&nbsp;&nbsp;  
                        手机号：<asp:TextBox ID="TbMobile" Width="120" runat="server" />&nbsp;&nbsp;
               
                        <asp:Button class="btn" OnClick="Search_OnClick" ID="Search" Text="搜 索" runat="server" />
                    </td>
                </tr>
            </table>
        </div>

        <table id="contents" class="table table-bordered table-hover">
            <tr class="info thead">
                <td width="20"></td>
                <td>卡号</td>
                <td>姓名</td>
                <td>手机</td>
                <td>消费类型</td>
                <td>消费金额</td>
                <td>消前余额</td>
                <td>消后余额</td>
                <td>消费时间</td>
                 <td>操作员</td>
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
                            <asp:Literal ID="LtlSN" runat="server"></asp:Literal>
                        </td>
                        <td>
                            <asp:Literal ID="LtlUserName" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlMobile" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlType" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlAmount" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlBeforeAmount" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlAfterAmount" runat="server"></asp:Literal>
                        </td>
                        <td class="center">
                            <asp:Literal ID="LtlAddDate" runat="server"></asp:Literal>
                        </td>
                         <td class="center">
                            <asp:Literal ID="LtlOperator" runat="server"></asp:Literal>
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
            <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
            <asp:Button class="btn" id="BtnReturn" Text="返 回" runat="server" />
        </ul>

    </form>
</body>
</html>
