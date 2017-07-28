<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTask" %>
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
  <bairong:alerts text="启用定时任务需要在服务器中安装SiteServer Service服务组件" runat="server" />

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn HeaderText="所属站点">
        <ItemTemplate>
          <asp:Literal ID="ltlSite" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="160" HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="任务名称">
        <ItemTemplate>
          <asp:Literal ID="ltlTaskName" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="执行周期">
        <ItemTemplate>
          <asp:Literal ID="ltlFrequencyType" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="最近一次执行时间">
        <ItemTemplate>
          <asp:Literal ID="ltlLastExecuteDate" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="120" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="是否启用">
        <ItemTemplate>
          <asp:Literal ID="ltlIsEnabled" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlEditHtml" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="40" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlEnabledHtml" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="40" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDeleteHtml" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="40" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddTask" Text="添加任务" runat="server" />
  </ul>

</form>
</body>
</html>
