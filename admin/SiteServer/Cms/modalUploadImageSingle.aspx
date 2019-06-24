<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadImageSingle" Trace="false" %>
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

				<div class="form-group form-row">
					<label class="col-3 col-form-label text-right">选择上传的图片</label>
					<div class="col-8">
						<input type="file" id="HifUpload" class="form-control" runat="server" />
					</div>
					<div class="col-1">
						<asp:RequiredFieldValidator ControlToValidate="HifUpload" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
						/>
					</div>
				</div>

				<asp:Literal id="LtlScript" runat="server"></asp:Literal>

				<hr />

				<div class="text-right mr-1">
					<asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
					<button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
				</div>

			</form>
		</body>

		</html>
		<!--#include file="../inc/foot.html"-->