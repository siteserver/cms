<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTipMessage" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<form runat="server">
				<ctrl:alerts runat="server" />

				<h4 class="text-center" style="margin-top: 20px; margin-bottom: 40px; color: #1d9e74;">
					<asp:Literal id="LtlTips" runat="server" />
				</h4>

				<hr />

				<div class="text-right mr-1">
					<button type="button" class="btn btn-primary m-l-5" onclick="window.parent.layer.closeAll()">确 定</button>
				</div>

			</form>
		</body>

		</html>
		<!--#include file="../inc/foot.html"-->