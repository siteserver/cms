<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationSite" %>
	<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html>

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<!--#include file="../inc/openWindow.html"-->

			<form class="container" runat="server">
				<bairong:alerts runat="server" />

				<div class="raw">
					<div class="card-box">
						<h4 class="m-t-0 header-title">
							<b>站点配置</b>
						</h4>
						<p class="text-muted font-13 m-b-25">
							在此修改站点相关设置
						</p>

						<ul class="nav nav-pills m-b-30">
							<li class="active">
								<a href="javascript:;">站点配置</a>
							</li>
							<li class="">
								<a href="pageConfigurationContent.aspx?publishmentSystemId=<%=PublishmentSystemId%>">内容配置</a>
							</li>
							<li class="">
								<a href="pageConfigurationComment.aspx?publishmentSystemId=<%=PublishmentSystemId%>">评论设置</a>
							</li>
							<li class="">
								<a href="pageConfigurationSiteAttributes.aspx?publishmentSystemId=<%=PublishmentSystemId%>">站点属性</a>
							</li>

						</ul>

						<div class="form-horizontal">

							<asp:PlaceHolder id="PhUrlSettings" runat="server">
								<div class="form-group">
									<label class="col-sm-3 control-label">Web部署方式</label>
									<div class="col-sm-3">
										<asp:DropDownList id="DdlIsSeparatedWeb" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="DdlIsSeparatedWeb_SelectedIndexChanged"
										  runat="server"></asp:DropDownList>
									</div>
									<div class="col-sm-6 help-block">
										设置网站页面部署方式
									</div>
								</div>

								<asp:PlaceHolder ID="PhSeparatedWeb" runat="server">
									<div class="form-group">
										<label class="col-sm-3 control-label">独立部署Web访问地址</label>
										<div class="col-sm-3">
											<asp:TextBox id="TbSeparatedWebUrl" class="form-control" runat="server"></asp:TextBox>
										</div>
										<div class="col-sm-6 help-block">
											<asp:RequiredFieldValidator ControlToValidate="TbSeparatedWebUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
											/>
											<asp:RegularExpressionValidator runat="server" ControlToValidate="TbSeparatedWebUrl" ValidationExpression="[^']+" ErrorMessage=" *"
											  ForeColor="red" Display="Dynamic" />
										</div>
									</div>
								</asp:PlaceHolder>
							</asp:PlaceHolder>

							<div class="form-group">
								<label class="col-sm-3 control-label">网页编码</label>
								<div class="col-sm-3">
									<asp:DropDownList id="DdlCharset" class="form-control" runat="server"></asp:DropDownList>
								</div>
								<div class="col-sm-6 help-block">
									模板编码将同步修改
								</div>
							</div>

							<div class="form-group">
								<label class="col-sm-3 control-label">后台信息每页显示数目</label>
								<div class="col-sm-3">
									<asp:TextBox Columns="25" Text="18" MaxLength="50" id="TbPageSize" class="form-control" runat="server" />
								</div>
								<div class="col-sm-6 help-block">
									<asp:RequiredFieldValidator ControlToValidate="TbPageSize" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
									/>
									<span>条</span>
								</div>
							</div>

							<div class="form-group">
								<label class="col-sm-3 control-label">是否启用双击生成页面</label>
								<div class="col-sm-3">
									<asp:DropDownList ID="DdlIsCreateDoubleClick" RepeatDirection="Horizontal" class="form-control" runat="server"></asp:DropDownList>
								</div>
								<div class="col-sm-6 help-block">
									此功能通常用于制作调试期间，网站正式上线后不建议启用
								</div>
							</div>

							<hr />

							<div class="form-group m-b-0">
								<div class="col-sm-offset-3 col-sm-9">
									<asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
								</div>
							</div>

						</div>

					</div>
				</div>

			</form>
		</body>

		</html>