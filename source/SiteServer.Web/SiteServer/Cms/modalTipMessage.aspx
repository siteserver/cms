<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTipMessage" %>
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

<h4 style="margin-top: 100px;margin-bottom: 20px;text-align: center;color: #1d9e74;">
	<asp:Literal id="ltlTips" runat="server" />
</h4>

<div style="margin-top: 40px;text-align: center;">
	<a href="javascript:;" onclick="window.parent.layer.closeAll();return false;" class="btn btn-success" style="width: 120px;">确 定</a>
</div>


</form>
</body>
</html>
