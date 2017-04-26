<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageUserGroup" %>
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
    <asp:Literal ID="ltlBreadCrumb" runat="server" />
    <bairong:Alerts runat="server" />

    <asp:dataGrid id="MyDataGrid" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" runat="server">
      <Columns>
      <asp:TemplateColumn HeaderText="用户组名称">
        <ItemTemplate>
          <asp:Literal ID="ltlGroupName" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="用户组说明">
        <ItemTemplate>
          <asp:Literal ID="ltlDescription" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="默认用户组">
        <ItemTemplate>
          <asp:Literal ID="ltlIsDefault" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      </Columns>
    </asp:dataGrid>

    <ul class="breadcrumb breadcrumb-button">
        <asp:Button class="btn btn-success" ID="AddButton" Text="添加用户组" runat="server" />
    </ul>

</form>

</body>
</html>
