<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentTags" %>
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

  <style type="text/css">
  .tag_popularity_1 { FONT-SIZE: 12px; font-weight:normal !important; COLOR: #104d6c; }
  .tag_popularity_2 { FONT-WEIGHT: bold; COLOR: #104d6c; }
  .tag_popularity_3 { FONT-WEIGHT: bold; COLOR: #ff0f6f; font-size:14px !important; }
  .tag_popularity_4 { FONT-WEIGHT: bold; font-size:16px !important; COLOR: #ff0f6f !important }
  </style>

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" AllowPaging="true" OnPageIndexChanged="MyDataGrid_Page" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <PagerStyle Mode="NumericPages" PageButtonCount="10" HorizontalAlign="Right" />
    <Columns>
      <asp:TemplateColumn HeaderText="标签">
        <ItemTemplate>
          <asp:Literal ID="ltlTagName" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="使用次数">
        <ItemTemplate>
          <asp:Literal ID="ltlCount" runat="server"></asp:Literal>
        </ItemTemplate>
        <ItemStyle Width="80" HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate> &nbsp;
          <asp:Literal ID="ltlEditUrl" runat="server"></asp:Literal>
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

  <table class="table noborder table-hover">
    <tr>
      <td>
        <asp:LinkButton ID="pageFirst" OnClick="NavigationButtonClick" CommandName="FIRST" Runat="server">首页</asp:LinkButton>
        |
        <asp:LinkButton ID="pagePrevious" OnClick="NavigationButtonClick" CommandName="PREVIOUS" Runat="server">前页</asp:LinkButton>
        |
        <asp:LinkButton ID="pageNext" OnClick="NavigationButtonClick" CommandName="NEXT" Runat="server">后页</asp:LinkButton>
        |
        <asp:LinkButton ID="pageLast" OnClick="NavigationButtonClick" CommandName="LAST" Runat="server">尾页</asp:LinkButton></td>
      <td class="align-right"> 分页
        <asp:Label Enabled=False Runat=server ID="currentPage" />
      </td>
    </tr>
  </table>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddTag" Text="添加标签" runat="server" />
  </ul>

</form>
</body>
</html>
