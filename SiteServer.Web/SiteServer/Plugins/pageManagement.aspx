<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.PageManagement" %>
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

    <div class="well well-small">
      <asp:Literal id="LtlNav" runat="server" />
    </div>

    <asp:dataGrid id="DgPlugins" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover"
      gridlines="none" runat="server">
      <Columns>
        <asp:TemplateColumn HeaderText="插件Id">
          <ItemTemplate>
            <asp:Literal ID="ltlPluginId" runat="server"></asp:Literal>
          </ItemTemplate>
          <ItemStyle HorizontalAlign="left" />
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="插件名称">
          <ItemTemplate>
            <asp:Literal ID="ltlPluginName" runat="server"></asp:Literal>
          </ItemTemplate>
          <ItemStyle HorizontalAlign="left" />
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="插件介绍">
          <ItemTemplate>
            <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
          </ItemTemplate>
          <ItemStyle HorizontalAlign="left" />
        </asp:TemplateColumn>
        <asp:TemplateColumn HeaderText="载入时间">
          <ItemTemplate>
            <asp:Literal ID="ltlInitTime" runat="server"></asp:Literal>
          </ItemTemplate>
          <ItemStyle Width="80" cssClass="center" />
        </asp:TemplateColumn>
        <asp:TemplateColumn>
          <ItemTemplate>
            <asp:Literal ID="ltlCmd" runat="server"></asp:Literal>
          </ItemTemplate>
          <ItemStyle Width="120" cssClass="center" />
        </asp:TemplateColumn>
      </Columns>
    </asp:dataGrid>

  </form>
</body>

</html>