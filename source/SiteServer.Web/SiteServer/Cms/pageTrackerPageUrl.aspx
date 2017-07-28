<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTrackerPageUrl" %>
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

  <asp:dataGrid id="dgContents" showHeader="true" AutoGenerateColumns="false" AllowPaging="true" DataKeyField="PageUrl" OnPageIndexChanged="MyDataGrid_Page" HeaderStyle-CssClass="info thead" CssClass="table table-bordered table-hover" gridlines="none" runat="server">
    <PagerStyle Mode="NumericPages" PageButtonCount="10" HorizontalAlign="Right" />
      <Columns>
      <asp:TemplateColumn HeaderText="受访页面">
        <ItemTemplate> <a HREF="<%# (string)DataBinder.Eval(Container.DataItem,"PageUrl")%>" TARGET="_BLANK"><%# (string)DataBinder.Eval(Container.DataItem,"PageUrl")%></a> </ItemTemplate>
        <ItemStyle HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="比例">
        <ItemTemplate>
          <div class="progress progress-success progress-striped">
            <div class="bar" style="width: <%# GetAccessNumBarWidth(Convert.ToInt32(DataBinder.Eval(Container.DataItem,"AccessNum")))%>%"></div>
          </div>
        </ItemTemplate>
        <ItemStyle Width="300" HorizontalAlign="left" />
      </asp:TemplateColumn>
      <asp:BoundColumn
        HeaderText="总访问量"
        DataField="AccessNum"
        ReadOnly="true">
        <ItemStyle Width="80" cssClass="center" />
      </asp:BoundColumn>
      <asp:TemplateColumn HeaderText="总访问人数">
        <ItemTemplate> <%# GetUniqueAccessNum((string)DataBinder.Eval(Container.DataItem,"PageUrl"))%> </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="当天访问量">
        <ItemTemplate> <%# GetTodayAccessNum((string)DataBinder.Eval(Container.DataItem,"PageUrl"))%> </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn HeaderText="当天访问人数">
        <ItemTemplate> <%# GetTodayUniqueAccessNum((string)DataBinder.Eval(Container.DataItem,"PageUrl"))%> </ItemTemplate>
        <ItemStyle Width="90" cssClass="center" />
      </asp:TemplateColumn>
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
