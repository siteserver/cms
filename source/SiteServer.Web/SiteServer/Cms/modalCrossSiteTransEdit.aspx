<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCrossSiteTransEdit" Trace="false" %>
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
                <td width="150">
                    <bairong:Help HelpText="跨站转发类型" Text="跨站转发类型：" runat="server"></bairong:Help>
                </td>
                <td>
                    <asp:DropDownList ID="TransType" AutoPostBack="true" OnSelectedIndexChanged="TransType_OnSelectedIndexChanged" runat="server"></asp:DropDownList></td>
            </tr>
            <asp:PlaceHolder ID="PlaceHolder_PublishmentSystem" Visible="false" runat="server">
                <tr>
                    <td width="150">
                        <bairong:Help HelpText="指定跨站转发站点" Text="指定跨站转发站点：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:DropDownList ID="PublishmentSystemIDCollection" AutoPostBack="true" OnSelectedIndexChanged="PublishmentSystemIDCollection_OnSelectedIndexChanged" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td width="150">
                        <bairong:Help HelpText="选择可跨站转发的栏目" Text="选择可跨站转发的栏目：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:ListBox ID="NodeIDCollection" SelectionMode="Multiple" Rows="12" Style="width: auto;" runat="server"></asp:ListBox></td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PlaceHolder_NodeNames" Visible="false" runat="server">
                <tr>
                    <td width="150">
                        <bairong:Help HelpText="设置可跨站转发的栏目名称" Text="设置可跨站转发的栏目名称：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:TextBox ID="NodeNames" TextMode="MultiLine" Columns="45" MaxLength="200" runat="server"></asp:TextBox>
                        <br />
                        多个栏目以,分隔，不添加栏目代表可以对所有栏目进行跨站转发。 </td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PlaceHolder_IsAutomatic" Visible="false" runat="server">
                <tr>
                    <td width="150">
                        <bairong:Help HelpText="设置是否自动转发内容" Text="是否自动转发内容：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="IsAutomatic" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                </tr>
                <tr>
                    <td width="150">
                        <bairong:Help HelpText="设置转发内容类型" Text="设置转发内容类型：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="ddlTranslateDoneType" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                </tr>
            </asp:PlaceHolder>
            <asp:PlaceHolder ID="PlaceHolder_Checked" Visible="false" runat="server">
                <tr>
                    <td width="150">
                        <bairong:Help HelpText="状态" Text="状态：" runat="server"></bairong:Help>
                    </td>
                    <td>
                        <asp:RadioButtonList ID="ddlCheckedStaus" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
                </tr>
            </asp:PlaceHolder>
        </table>

    </form>
</body>
</html>

