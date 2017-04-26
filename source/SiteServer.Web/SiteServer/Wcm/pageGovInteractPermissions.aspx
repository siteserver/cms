<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovInteractPermissions" %>
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
  <bairong:alerts text="超级管理员默认拥有互动交流的所有权限。" runat="server" />

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" runat="server">
  	<Columns>
        <asp:TemplateColumn HeaderText="部门">
			<ItemTemplate>
				&nbsp;<asp:Literal ID="ltlDepartmentName" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle HorizontalAlign="left" Width="200" />
		</asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="管理员账号">
			<ItemTemplate>
				<asp:Literal ID="ltlUserName" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle HorizontalAlign="left" Width="120" />
		</asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="姓名">
			<ItemTemplate>
				<asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle HorizontalAlign="left" Width="120" />
		</asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="权限">
			<ItemTemplate>
				<asp:Literal ID="ltlPermissions" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle HorizontalAlign="left" />
		</asp:TemplateColumn>
        <asp:TemplateColumn>
			<ItemTemplate>
				<asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle Width="100" cssClass="center" />
		</asp:TemplateColumn>
	</Columns>
  </asp:dataGrid>

</form>
</body>
</html>
