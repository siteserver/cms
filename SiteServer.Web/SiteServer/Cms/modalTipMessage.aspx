<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTipMessage" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<!--#include file="../inc/openWindow.html"-->

			<form runat="server">
				<ctrl:alerts runat="server" />

				<div class="form-horizontal">

					<h4 class="text-center" style="margin-top: 20px; margin-bottom: 40px; color: #1d9e74;">
						<asp:Literal id="LtlTips" runat="server" />
					</h4>

					<hr />

					<div class="form-group m-b-0">
						<div class="col-xs-12 text-center">
							<button type="button" class="btn btn-primary m-l-10" onclick="window.parent.layer.closeAll()">确 定</button>
						</div>
					</div>
				</div>

			</form>
		</body>

		</html>