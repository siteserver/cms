<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationSite" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
	<meta charset="utf-8">
	<!--#include file="../inc/head.html"-->
</head>

<body>
	<form class="m-l-15 m-r-15" runat="server">

		<div class="card-box" style="padding: 10px; margin-bottom: 10px;">
			<ul class="nav nav-pills nav-justified">
				<li class="nav-item active">
					<a class="nav-link" href="javascript:;">站点设置</a>
				</li>
				<li class="nav-item">
					<a class="nav-link" href="pageConfigurationContent.aspx?siteId=<%=SiteId%>">内容设置</a>
				</li>
				<li class="nav-item">
					<a class="nav-link" href="pageConfigurationSiteAttributes.aspx?siteId=<%=SiteId%>">站点属性</a>
				</li>
			</ul>
		</div>

		<ctrl:alerts runat="server" />

		<div class="card-box">
			<div class="form-group">
				<label class="col-form-label">网页编码</label>
				<asp:DropDownList id="DdlCharset" class="form-control" runat="server"></asp:DropDownList>
				<small class="form-text text-muted">模板编码将同步修改</small>
			</div>

			<div class="form-group">
				<label class="col-form-label">后台信息每页显示数目(条)
					<asp:RequiredFieldValidator ControlToValidate="TbPageSize" errorMessage=" *" foreColor="red" Display="Dynamic"
					 runat="server" />
				</label>
				<asp:TextBox Text="18" MaxLength="50" id="TbPageSize" class="form-control" runat="server" />
			</div>

			<div class="form-group">
				<label class="col-form-label">是否启用双击生成页面</label>
				<asp:DropDownList ID="DdlIsCreateDoubleClick" RepeatDirection="Horizontal" class="form-control" runat="server"></asp:DropDownList>
				<small class="form-text text-muted">此功能通常用于制作调试期间，网站正式上线后不建议启用</small>
			</div>

			<hr />

			<asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />

		</div>

	</form>
</body>

</html>
<!--#include file="../inc/foot.html"-->