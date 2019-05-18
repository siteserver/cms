<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageRelatedField" %>
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
				<li class="nav-item">
					<a class="nav-link" href="pageTableStyleContent.aspx?siteId=<%=SiteId%>">内容字段管理</a>
				</li>
				<li class="nav-item">
					<a class="nav-link" href="pageTableStyleChannel.aspx?siteId=<%=SiteId%>">栏目字段管理</a>
				</li>
				<li class="nav-item">
					<a class="nav-link" href="pageTableStyleSite.aspx?siteId=<%=SiteId%>">站点字段管理</a>
				</li>
				<li class="nav-item active">
					<a class="nav-link" href="javascript:;">联动字段设置</a>
				</li>
			</ul>
		</div>

		<ctrl:alerts runat="server" />

		<div class="card-box">
			<div class="panel panel-default">
				<div class="panel-body p-0">
					<div class="table-responsive">
						<table class="table tablesaw table-hover m-0">
							<thead>
								<tr>
									<th>联动字段名称 </th>
									<th class="text-center" width="100">级数 </th>
									<th width="100"></th>
									<th width="60"></th>
									<th width="60"></th>
									<th width="60"></th>
								</tr>
							</thead>
							<tbody>
								<asp:Repeater ID="RptContents" runat="server">
									<itemtemplate>
										<tr>
											<td>
												<asp:Literal ID="ltlRelatedFieldName" runat="server"></asp:Literal>
											</td>
											<td class="text-center">
												<asp:Literal ID="ltlTotalLevel" runat="server"></asp:Literal>
											</td>
											<td class="text-center">
												<asp:Literal ID="ltlItemsUrl" runat="server"></asp:Literal>
											</td>
											<td class="text-center">
												<asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
											</td>
											<td class="text-center">
												<asp:Literal ID="ltlExportUrl" runat="server"></asp:Literal>
											</td>
											<td class="text-center">
												<asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
											</td>
										</tr>
									</itemtemplate>
								</asp:Repeater>
							</tbody>
						</table>
					</div>
				</div>
			</div>

			<hr />

			<asp:Button ID="BtnAdd" Text="添加联动字段" Cssclass="btn btn-primary m-r-5" runat="server"></asp:Button>
			<asp:Button ID="BtnImport" Text="导 入" Cssclass="btn m-r-5" runat="server"></asp:Button>

		</div>

	</form>
</body>

</html>
<!--#include file="../inc/foot.html"-->