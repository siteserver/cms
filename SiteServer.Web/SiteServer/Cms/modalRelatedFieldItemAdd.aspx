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

				<div class="form-horizontal">

					<div class="form-group">
						<label class="col-xs-1 text-right control-label"></label>
						<div class="col-xs-10">
							<asp:TextBox class="form-control" Rows="8" TextMode="MultiLine" id="TbItemNames" runat="server" />
						</div>
						<div class="col-xs-1">
							<asp:RequiredFieldValidator ControlToValidate="TbItemNames" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
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