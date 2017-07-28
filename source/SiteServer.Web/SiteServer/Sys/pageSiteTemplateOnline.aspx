<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Sys.PageSiteTemplateOnline" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn HeaderText="站点模板名称">
        <ItemTemplate>
          <asp:Literal ID="ltlTemplateName" runat="server"></asp:Literal>
        </ItemTemplate>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="站点类型">
        <ItemTemplate>
          <asp:Literal ID="ltlTemplateType" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="110" cssClass="center"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="站点模板文件夹">
        <ItemTemplate>
          <asp:Literal ID="ltlDirectoryName" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="left"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="大小">
        <ItemTemplate>
          <asp:Literal ID="ltlSize" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" cssClass="center"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="模板作者">
        <ItemTemplate>
          <asp:Literal ID="ltlAuthor" runat="server"></asp:Literal>
        </ItemTemplate>
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="上传时间">
        <ItemTemplate>
          <asp:Literal ID="ltlUploadDate" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="100" cssClass="center"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlPageUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" cssClass="center"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDemoUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" cssClass="center"/>
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDownloadUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" cssClass="center"/>
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <br>

</form>
</body>
</html>
