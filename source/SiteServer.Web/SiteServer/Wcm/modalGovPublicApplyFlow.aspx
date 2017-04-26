<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovPublicApplyFlow" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

	<table class="table table-noborder">
	  <asp:Literal ID="ltlFlows" runat="server"></asp:Literal>
	</table>

</form>
</body>
</html>
