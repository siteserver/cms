<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalRelatedFieldItemAdd" Trace="false"%>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<form runat="server">
				<ctrl:alerts text="每一行为一个选项，如果显示项与值不同可以用“|”隔开，左边为显示项，右边为值" runat="server" />

				<div class="form-group form-row">
					<label class="col-1 text-right col-form-label"></label>
					<div class="col-10">
						<asp:TextBox class="form-control" Rows="8" TextMode="MultiLine" id="TbItemNames" runat="server" />
					</div>
					<div class="col-1">
						<asp:RequiredFieldValidator ControlToValidate="TbItemNames" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
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