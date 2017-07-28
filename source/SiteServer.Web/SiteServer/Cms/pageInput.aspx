<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageInput" %>
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

  <asp:dataGrid id="DgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="InputId" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>     
      <asp:TemplateColumn
        HeaderText="提交表单名称">
        <ItemTemplate>
          <asp:Literal ID="ltlTitle" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="需要审核">
        <ItemTemplate>
          <asp:Literal ID="ltlIsCheck" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="需要回复">
        <ItemTemplate>
          <asp:Literal ID="ltlIsReply" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="60" cssClass="center" />
      </asp:TemplateColumn>
      <asp:BoundColumn
        HeaderText="添加日期"
        DataField="AddDate"
        DataFormatString="{0:yyyy-MM-dd}"
        ReadOnly="true">
        <ItemStyle Width="70" cssClass="center" />
      </asp:BoundColumn>
      <asp:TemplateColumn HeaderText="上升">
        <ItemTemplate>
          <asp:Literal ID="ltlUpLink" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="下降">
        <ItemTemplate>
          <asp:Literal ID="ltlDownLink" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn >
        <ItemTemplate>
          <asp:Literal ID="ltlStyleUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="70" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlPreviewUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
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
    <asp:Button class="btn btn-success" id="BtnAddInput" Text="添加提交表单" runat="server" />
    <asp:Button class="btn" id="BtnImport" Text="导入提交表单" runat="server" />
  </ul>

</form>
</body>
</html>
