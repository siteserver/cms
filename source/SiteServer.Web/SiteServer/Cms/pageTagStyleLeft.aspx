<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTagStyleLeft" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
  	<Columns>
        <asp:TemplateColumn HeaderText="名称">
			<ItemTemplate>
				<asp:Literal ID="ltlName" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle HorizontalAlign="left" />
		</asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="进入">
			<ItemTemplate>
				<asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle cssClass="center" />
		</asp:TemplateColumn>
	</Columns>
  </asp:dataGrid>

</form>
</body>
</html>
