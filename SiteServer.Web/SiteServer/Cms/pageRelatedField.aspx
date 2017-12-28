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
				<ctrl:alerts runat="server" />

				<div class="raw">
					<div class="card-box">
						<h4 class="m-t-0 header-title">
							<b>联动字段设置</b>
						</h4>
						<p class="text-muted font-13 m-b-25">
							在此设置联动字段
						</p>

						<ul class="nav nav-pills m-b-30">
							<li class="">
								<a href="pageTableStyleContent.aspx?publishmentSystemId=<%=PublishmentSystemId%>">内容字段管理</a>
							</li>
							<li class="">
								<a href="pageTableStyleChannel.aspx?publishmentSystemId=<%=PublishmentSystemId%>">栏目字段管理</a>
							</li>
							<li class="active">
								<a href="javascript:;">联动字段设置</a>
							</li>
						</ul>

						<div class="form-horizontal">

							<asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="RelatedFieldID" HeaderStyle-CssClass="info thead text-center"
							  CssClass="table table-hover m-0" gridlines="none" runat="server">
								<Columns>
									<asp:TemplateColumn HeaderText="联动字段名称">
										<ItemTemplate>
											&nbsp;
											<asp:Literal ID="ltlRelatedFieldName" runat="server"></asp:Literal>
										</ItemTemplate>
										<ItemStyle HorizontalAlign="left" />
									</asp:TemplateColumn>
									<asp:TemplateColumn HeaderText="级数">
										<ItemTemplate>
											&nbsp;
											<asp:Literal ID="ltlTotalLevel" runat="server"></asp:Literal>
										</ItemTemplate>
										<ItemStyle Width="100" cssClass="center" />
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:Literal ID="ltlItemsUrl" runat="server"></asp:Literal>
										</ItemTemplate>
										<ItemStyle Width="100" cssClass="center" />
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
										</ItemTemplate>
										<ItemStyle Width="50" cssClass="center" />
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:Literal ID="ltlExportUrl" runat="server"></asp:Literal>
										</ItemTemplate>
										<ItemStyle Width="50" cssClass="center" />
									</asp:TemplateColumn>
									<asp:TemplateColumn>
										<ItemTemplate>
											<asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
										</ItemTemplate>
										<ItemStyle Width="50" cssClass="center" />
									</asp:TemplateColumn>
								</Columns>
							</asp:dataGrid>

						</div>

						<hr />

						<div class="form-group m-b-0">
							<asp:Button ID="AddButton" Text="添加联动字段" Cssclass="btn m-r-5" runat="server"></asp:Button>
							<asp:Button ID="ImportButton" Text="导 入" Cssclass="btn m-r-5" runat="server"></asp:Button>
						</div>

					</div>
				</div>

			</form>
		</body>

		</html>