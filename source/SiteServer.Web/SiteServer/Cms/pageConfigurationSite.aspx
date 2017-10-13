<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationSite" %>
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

				<div class="popover popover-static">
					<h3 class="popover-title">站点配置管理</h3>
					<div class="popover-content">
						<table class="table noborder table-hover">
							<asp:PlaceHolder id="PhUrlSettings" runat="server">
								<tr>
									<td width="200">Web部署方式：</td>
									<td>
										<asp:DropDownList id="DdlIsSeparatedWeb" AutoPostBack="true" OnSelectedIndexChanged="DdlIsSeparatedWeb_SelectedIndexChanged"
										  runat="server"></asp:DropDownList>
										<br />
										<span>设置网站页面部署方式</span>
									</td>
								</tr>

								<asp:PlaceHolder ID="PhSeparatedWeb" runat="server">
									<tr>
										<td width="200">独立部署Web访问地址：</td>
										<td>
											<asp:TextBox id="TbSeparatedWebUrl" class="form-control" runat="server"></asp:TextBox>
											<asp:RequiredFieldValidator ControlToValidate="TbSeparatedWebUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
											/>
											<asp:RegularExpressionValidator runat="server" ControlToValidate="TbSeparatedWebUrl" ValidationExpression="[^']+" ErrorMessage=" *"
											  ForeColor="red" Display="Dynamic" />
										</td>
									</tr>
								</asp:PlaceHolder>
							</asp:PlaceHolder>

							<tr>
								<td width="200">网页编码：</td>
								<td>
									<asp:DropDownList id="DdlCharset" runat="server"></asp:DropDownList>
									<br>
									<span>模板编码将同步修改</span>
								</td>
							</tr>
							<tr>
								<td>后台信息每页显示数目：</td>
								<td>
									<asp:TextBox Columns="25" Text="18" MaxLength="50" id="TbPageSize" class="input-mini" runat="server" />
									<asp:RequiredFieldValidator ControlToValidate="TbPageSize" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
									/> <span>条</span>
								</td>
							</tr>
							<tr>
								<td>是否启用双击生成页面：</td>
								<td>
									<asp:RadioButtonList ID="RblIsCreateDoubleClick" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server"></asp:RadioButtonList>
									<span>此功能通常用于制作调试期间，网站正式上线后不建议启用</span>
								</td>
							</tr>
						</table>
						<hr />
						<table class="table noborder">
							<tr>
								<td class="center">
									<asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
								</td>
							</tr>
						</table>
					</div>
				</div>

			</form>
		</body>

		</html>