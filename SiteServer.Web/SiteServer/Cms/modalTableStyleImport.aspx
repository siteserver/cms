<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStyleImport" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<!--#include file="../inc/openWindow.html"-->

			<form enctype="multipart/form-data" method="post" runat="server">
				<ctrl:alerts runat="server" />

				<div class="form-horizontal">

					<div class="form-group">
						<label class="col-xs-3 text-right control-label">表样式文件</label>
						<div class="col-xs-8">
							<input type="file" id="HifMyFile" class="form-control" runat="server" />
						</div>
						<div class="col-xs-1 help-block">
							<asp:RequiredFieldValidator ControlToValidate="HifMyFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
							/>
						</div>
					</div>

					<hr />

					<div class="form-group m-b-0">
						<div class="col-xs-11 text-right">
							<asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
							<button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
						</div>
						<div class="col-xs-1"></div>
					</div>

				</div>

			</form>
		</body>

		</html>