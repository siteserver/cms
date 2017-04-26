<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageSeoMetaList" %>
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

  <script>
    var previousID;
    function toggleAppare(id){
      if (previousID && previousID != id){
        $('#' + previousID).hide();
      }
      $('#' + id).toggle();
      previousID = id;
    }
  </script>

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="SeoMetaID" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <Columns>
      <asp:TemplateColumn
        HeaderText="名称">
        <ItemTemplate>
          <asp:Literal id="ltlSeoMetaName" runat="server" />
        </ItemTemplate>
        <ItemStyle Width="140" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="页面标题">
        <ItemTemplate>
          <asp:Literal id="ltlPageTitle" runat="server" />
        </ItemTemplate>
        <ItemStyle Wrap="false" HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="是否默认">
        <ItemTemplate> 
          <asp:Literal id="ltlIsDefault" runat="server" />
        </ItemTemplate>
        <ItemStyle Width="100" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:HyperLink ID="hlViewLink" runat="server" NavigateUrl="javascript:;" Text="查看"></asp:HyperLink>
        </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:HyperLink ID="hlEditLink" runat="server" NavigateUrl="javascript:;" Text="编辑"></asp:HyperLink>
        </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDefaultUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate>
          <asp:Literal ID="ltlDeleteUrl" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddSeoMeta" Text="添加页面元数据" runat="server" />
  </ul>

</form>
</body>
</html>
