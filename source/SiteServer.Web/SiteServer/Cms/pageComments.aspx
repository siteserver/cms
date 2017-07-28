<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageComments" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<%@ Register TagPrefix="stl" Namespace="SiteServer.CMS.StlControls" Assembly="SiteServer.CMS" %>
<!DOCTYPE html>
<html>

<head>
    <meta charset="utf-8">
    <!--#include file="../inc/header.aspx"-->
</head>

<body>
    <!--#include file="../inc/openWindow.html"-->
    <form class="form-inline" runat="server">
        <asp:Literal id="ltlBreadCrumb" runat="server" />
        <bairong:alerts runat="server" />

        <!--#include file="../inc/scripts.aspx"-->
        <bairong:Script src="~/sitefiles/assets/components/js.cookie.js" runat="server" />
        <stl:commentInput id="StlCommentInput" IsAnonymous="true" PageNum="20" runat="server" />

        <ul class="breadcrumb breadcrumb-button">
            <asp:Button class="btn" id="BtnExport" runat="server" Text="导出Excel"></asp:Button>
            <asp:Button class="btn" CausesValidation="false" OnClick="Return_OnClick" Text="返 回" runat="server" />
        </ul>
    </form>
</body>

</html>