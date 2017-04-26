<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTrackerCurrentVisitors" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" AllowPaging="true" DataKeyField="TrackingID" OnPageIndexChanged="MyDataGrid_Page" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <PagerStyle Mode="NumericPages" PageButtonCount="10" HorizontalAlign="Right" />
      <Columns>
      <asp:BoundColumn
        HeaderText="访问时间"
        DataField="AccessDateTime"
        DataFormatString="{0:yyyy-MM-dd HH:mm:ss.fff}"
        ReadOnly="true">
        <ItemStyle Width="70" Wrap="true" HorizontalAlign="left" />
      </asp:BoundColumn>
      <asp:BoundColumn
        HeaderText="IP地址"
        DataField="IPAddress"
        ReadOnly="true">
        <ItemStyle Width="70" Wrap="true" HorizontalAlign="left" />
      </asp:BoundColumn>
      <asp:BoundColumn
        HeaderText="访问页面"
        DataField="PageUrl"
        ReadOnly="true">
        <ItemStyle Wrap="true" HorizontalAlign="left" />
      </asp:BoundColumn>
      <asp:BoundColumn
        HeaderText="来路"
        DataField="Referrer"
        ReadOnly="true">
        <ItemStyle Wrap="true" HorizontalAlign="left" />
      </asp:BoundColumn>
    </Columns>
  </asp:dataGrid>

  <table class="table noborder">
    <tr>
      <td><asp:LinkButton ID="pageFirst" OnClick="NavigationButtonClick" CommandName="FIRST" Runat="server">首页</asp:LinkButton>
        |
        <asp:LinkButton ID="pagePrevious" OnClick="NavigationButtonClick" CommandName="PREVIOUS" Runat="server">前页</asp:LinkButton>
        |
        <asp:LinkButton ID="pageNext" OnClick="NavigationButtonClick" CommandName="NEXT" Runat="server">后页</asp:LinkButton>
        |
        <asp:LinkButton ID="pageLast" OnClick="NavigationButtonClick" CommandName="LAST" Runat="server">尾页</asp:LinkButton></td>
      <td class="align-right"> 分页
        <asp:Label Enabled=False Runat=server ID="currentPage" /></td>
    </tr>
  </table>

</form>
</body>
</html>
