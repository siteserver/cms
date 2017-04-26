<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageVoteIPAddress" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" DataKeyField="VoteIPAddressID" AllowPaging="true" OnPageIndexChanged="MyDataGrid_Page" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" runat="server">
    <PagerStyle Mode="NumericPages" PageButtonCount="10" HorizontalAlign="Right" />
      <Columns>
      <asp:TemplateColumn
        HeaderText="IP地址">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"IPAddress")%> </ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn
        HeaderText="用户">
        <ItemTemplate> <%#DataBinder.Eval(Container.DataItem,"UserName")%> </ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:BoundColumn
        HeaderText="投票时间"
        DataField="AddDate"
        DataFormatString="{0:yyyy-MM-dd HH:mm}"
        ReadOnly="true">
        <ItemStyle HorizontalAlign="left" />
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

  <ul class="breadcrumb breadcrumb-button">
    <input type=button class="btn" onClick="location.href='pageVote.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';" value="返 回" />
  </ul>

</form>
</body>
</html>
