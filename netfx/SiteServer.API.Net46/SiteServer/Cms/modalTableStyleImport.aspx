<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStyleImport" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<form enctype="multipart/form-data" method="post" runat="server">
				<ctrl:alerts runat="server" />

				<div class="form-group form-row">
					<label class="col-3 text-right col-form-label">表样式文件</label>
					<div class="col-8">
						<input type="file" id="HifMyFile" class="form-control" runat="server" />
					</div>
					<div class="col-1 help-block">
						<asp:RequiredFieldValidator ControlToValidate="HifMyFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
						/>
					</div>
				</div>

				<hr />

				<div class="text-right mr-1">
					<asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
					<button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
				</div>

			</form>
		</body>

		</html>
		<!--#include file="../inc/foot.html"-->