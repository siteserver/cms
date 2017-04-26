<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PageSiteTemplate" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="Name" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn HeaderText="站点模板名称">
        <ItemTemplate><asp:Literal ID="ltlTemplateName" runat="server"></asp:Literal></ItemTemplate>
        <ItemStyle HorizontalAlign="left"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="站点类型">
        <ItemTemplate><asp:Literal ID="ltlTemplateType" runat="server"></asp:Literal></ItemTemplate>
        <ItemStyle cssClass="center"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="站点模板文件夹">
        <ItemTemplate><asp:Literal ID="ltlDirectoryName" runat="server"></asp:Literal></ItemTemplate>
        <ItemStyle HorizontalAlign="left"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="站点模板介绍">
        <ItemTemplate><asp:Literal ID="ltlDescription" runat="server"></asp:Literal></ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="创建日期">
        <ItemTemplate><asp:Literal ID="ltlCreationDate" runat="server"></asp:Literal></ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDownloadUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="180" cssClass="center"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlCreateUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center"/>
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="Import" Text="导入站点模板" runat="server" />
  </ul>

</form>
</body>
</html>
