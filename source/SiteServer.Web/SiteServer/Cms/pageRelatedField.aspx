<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageRelatedField" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="RelatedFieldID" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
  	<Columns>
        <asp:TemplateColumn
			HeaderText="联动字段名称">
			<ItemTemplate>
				&nbsp;<asp:Literal ID="ltlRelatedFieldName" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle HorizontalAlign="left" />
		</asp:TemplateColumn>
        <asp:TemplateColumn
			HeaderText="级数">
			<ItemTemplate>
				&nbsp;<asp:Literal ID="ltlTotalLevel" runat="server"></asp:Literal>
			</ItemTemplate>
			<ItemStyle Width="100" cssClass="center" />
		</asp:TemplateColumn>
        <asp:TemplateColumn >
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

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button ID="AddButton" Text="添加联动字段" Cssclass="btn btn-success" runat="server"></asp:Button>
    <asp:Button ID="ImportButton" Text="导 入" Cssclass="btn" runat="server"></asp:Button>
  </ul>

</form>
</body>
</html>
