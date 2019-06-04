<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageRelatedFieldItem" %>
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

				<div class="card-box">

					<div class="panel panel-default">
						<div class="panel-body p-0">
							<div class="table-responsive">
								<table id="contents" class="table tablesaw table-hover m-0">
									<thead>
										<tr>
											<th>字段项名 </th>
											<th>字段项值 </th>
											<th width="50"></th>
											<th width="50"></th>
											<th width="60"></th>
											<th width="60"></th>
										</tr>
									</thead>
									<tbody>
										<asp:Repeater ID="RptContents" runat="server">
											<itemtemplate>
												<tr>
													<td>
														<asp:Literal ID="ltlItemName" runat="server"></asp:Literal>
													</td>
													<td>
														<asp:Literal ID="ltlItemValue" runat="server"></asp:Literal>
													</td>
													<td class="text-center">
														<asp:HyperLink ID="hlUp" runat="server">
															<i class="ion-arrow-up-a" style="font-size: 18px"></i>
														</asp:HyperLink>
													</td>
													<td class="text-center">
														<asp:HyperLink ID="hlDown" runat="server">
															<i class="ion-arrow-down-a" style="font-size: 18px"></i>
														</asp:HyperLink>
													</td>
													<td class="text-center">
														<asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
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

					<asp:Button class="btn btn-primary" id="BtnAdd" Text="添加字段项" runat="server" />
					<asp:Button class="btn m-l-5" id="BtnReturn" Text="返 回" runat="server" />

				</div>

			</form>
		</body>

		</html>
		<!--#include file="../inc/foot.html"-->