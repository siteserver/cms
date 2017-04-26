<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTrackerBrowser" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="Browser" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn HeaderText="浏览器">
        <ItemTemplate> <%# (string)DataBinder.Eval(Container.DataItem,"Browser")%> </ItemTemplate>
        <ItemStyle Width="120" HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="比例">
        <ItemTemplate>
          <div class="progress progress-success progress-striped">
            <div class="bar" style="width: <%# GetAccessNumBarWidth(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"AccessNum")))%>%;"></div>
          </div>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:BoundColumn
        HeaderText="总访问量"
        DataField="AccessNum"
        ReadOnly="true">
        <ItemStyle Width="80" cssClass="center" />
      </asp:BoundColumn>
      <asp:TemplateColumn HeaderText="总访问人数">
        <ItemTemplate> <%# GetUniqueAccessNum((string)DataBinder.Eval(Container.DataItem,"Browser"))%> </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="当天访问量">
        <ItemTemplate> <%# GetTodayAccessNum((string)DataBinder.Eval(Container.DataItem,"Browser"))%> </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="当天访问人数">
        <ItemTemplate> <%# GetTodayUniqueAccessNum((string)DataBinder.Eval(Container.DataItem,"Browser"))%> </ItemTemplate>
        <ItemStyle Width="90" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

</form>
</body>
</html>
