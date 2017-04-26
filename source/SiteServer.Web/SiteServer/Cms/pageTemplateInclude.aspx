<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateInclude" %>
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
  <bairong:alerts text="包含文件在站点目录 include 中，模板中使用 &amp;lt;stl:include file=&quot;/include/包含文件.html&quot;&gt;&amp;lt;/stl:include&gt; 引用。" runat="server" />

  <asp:dataGrid id="DgFiles" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn HeaderText="文件名称">
        <ItemTemplate> <a href="pageTemplateIncludeAdd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&FileName=<%# Container.DataItem %>"><%# Container.DataItem %></a></ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="文件编码">
        <ItemTemplate> <%# GetCharset((string)Container.DataItem) %> </ItemTemplate>
        <ItemStyle Width="160" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate> <a href="pageTemplateInclude.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&Delete=True&FileName=<%# Container.DataItem %>" onClick="javascript:return confirm('此操作将删除包含文件“<%# Container.DataItem %>”，确认吗？');">删除</a> </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <input type=button class="btn btn-success" onClick="location.href='pageTemplateIncludeAdd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';" value="添 加" />
  </ul>

</form>
</body>
</html>
