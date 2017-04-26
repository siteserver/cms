<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageInnerLink" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="InnerLinkName" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:BoundColumn
        HeaderText="链接关键字"
        DataField="InnerLinkName" >
        <ItemStyle Width="160" cssClass="center" />
      </asp:BoundColumn>
      <asp:TemplateColumn HeaderText="链接地址">
        <ItemTemplate> <a href='<%# GetLinkUrl((string)DataBinder.Eval(Container.DataItem,"LinkUrl"))%>' target="_blank"><%# DataBinder.Eval(Container.DataItem,"LinkUrl")%></a> </ItemTemplate>
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate> <%# GetEditHtml((string)DataBinder.Eval(Container.DataItem,"InnerLinkName"))%> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate> <%# GetDeleteHtml((string)DataBinder.Eval(Container.DataItem,"InnerLinkName"))%> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddInnerLink" Text="添加站内链接" runat="server" />
  </ul>

</form>
</body>
</html>
