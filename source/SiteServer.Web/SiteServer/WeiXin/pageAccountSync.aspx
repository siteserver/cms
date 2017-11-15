<%@ Page Language="C#" Inherits="SiteServer.WeiXin.BackgroundPages.ConsoleAccountSync" enableViewState = "false" %>
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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <asp:Button id="BtnSync" class="btn btn-success" text="同步公众账号" runat="server" OnClick="btnSync_Click" />

</form>
</body>
</html>
