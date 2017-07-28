<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTagStyleMenu" %>
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
  <bairong:alerts text="下拉菜单标签为&amp;lt;stl:menu styleName=&quot;样式名称&quot;&gt;&amp;lt;/stl:menu&gt;，用于显示下拉菜单。" runat="server"></bairong:alerts>

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="MenuDisplayID" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn
        HeaderText="样式名称">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"MenuDisplayName")%> </ItemTemplate>
        <ItemStyle Width="180" cssClass="center" />
      </asp:TemplateColumn>
      <asp:BoundColumn
        HeaderText="添加日期"
        DataField="AddDate"
        DataFormatString="{0:yyyy-MM-dd}"
        ReadOnly="true">
        <ItemStyle Width="70" cssClass="center" />
      </asp:BoundColumn>
      <asp:TemplateColumn>
        <ItemTemplate> <a <%#(IsDefault((string)DataBinder.Eval(Container.DataItem,"IsDefault"))) ? "style='display:none'" : ""%> href="pageTagStyleMenu.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&MenuDisplayID=<%# DataBinder.Eval(Container.DataItem,"MenuDisplayID")%>&SetDefault=True">设为默认</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate> <a href="pageTagStyleMenuAdd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&MenuDisplayID=<%# DataBinder.Eval(Container.DataItem,"MenuDisplayID")%>">编辑</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate> <a <%#(IsDefault((string)DataBinder.Eval(Container.DataItem,"IsDefault"))) ? "style='display:none'" : ""%> href="pageTagStyleMenu.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&Delete=True&MenuDisplayID=<%# DataBinder.Eval(Container.DataItem,"MenuDisplayID")%>" onClick="javascript:return confirm('此操作将删除此菜单显示，确认吗？');">删除</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <input type="button" class="btn btn-success" onClick="location.href='pageTagStyleMenuAdd.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';return false;" value="添加样式" />
  </ul>

</form>
</body>
</html>
