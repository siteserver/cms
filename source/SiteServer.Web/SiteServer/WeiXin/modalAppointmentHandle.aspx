<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalAppointmentHandle" Trace="false" %>
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

        <table class="table table-noborder table-hover">
            <tr>
                <td width="80">预约状态</td>
                <td>
                    <asp:DropDownList ID="DdlStatus" autoPostBack="true" onSelectedIndexChanged="DdlStatus_SelectedIndexChanged" runat="server"></asp:DropDownList>
                </td>
            </tr>
            <asp:PlaceHolder id="PhMessage" runat="server">
            <tr>
                <td>留言</td>
                <td>
                    <asp:TextBox ID="TbMessage" TextMode="Multiline" class="textarea" Rows="3" Style="width: 95%; padding: 5px;" runat="server" />
                </td>
            </tr>
            </asp:PlaceHolder>
        </table>

    </form>
</body>
</html>

