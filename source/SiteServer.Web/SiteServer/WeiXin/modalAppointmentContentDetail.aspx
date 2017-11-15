<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalAppointmentContentDetail" Trace="false" %>
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
        <bairong:Alerts runat="server"></bairong:Alerts>
        <table class="table table-noborder">
            <tr>
                <td>预约名称：</td>
                <td>
                    <asp:Literal ID="LtlAppointementTitle" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>姓名：</td>
                <td>
                    <asp:Literal ID="LtlRealName" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>电话：</td>
                <td>
                    <asp:Literal ID="LtlMobile" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>邮箱：</td>
                <td>
                    <asp:Literal ID="LtlEmail" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>预约时间：</td>
                <td>
                    <asp:Literal ID="LtlAddDate" runat="server"></asp:Literal>
                </td>
            </tr>
            <asp:Literal ID="LtlExtendVal" runat="server"></asp:Literal>
        </table>
    </form>
</body>
</html>


