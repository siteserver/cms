<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.PageRole" %>
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

  <asp:dataGrid id="dgContents" runat="server" showHeader="true"
      ShowFooter="false"
      AutoGenerateColumns="false"
      AllowPaging="true"
      DataKeyField="RoleName"
      OnPageIndexChanged="dgContents_Page"
      HeaderStyle-CssClass="info thead"
      CssClass="table table-bordered table-hover"
      gridlines="none">
    <PagerStyle Mode="NumericPages" PageButtonCount="10" />
    <Columns>
      <asp:BoundColumn
        HeaderText="角色名称"
        DataField="RoleName" >
        <ItemStyle Width="130" HorizontalAlign="left" />
      </asp:BoundColumn>
      <asp:BoundColumn
        HeaderText="备注"
        DataField="Description" >
      </asp:BoundColumn>
      <asp:TemplateColumn>
        <ItemTemplate> <a href='pageRoleAdd.aspx?RoleName=<%# DataBinder.Eval(Container.DataItem,"RoleName")%>'>修改</a> </ItemTemplate>
        <ItemStyle Width="80" cssClass="center" />
      </asp:TemplateColumn>
      <asp:TemplateColumn>
        <ItemTemplate> <a href='pageRole.aspx?Delete=True&RoleName=<%# DataBinder.Eval(Container.DataItem,"RoleName")%>' onClick="javascript:return confirm('此操作将会删除角色“<%# DataBinder.Eval(Container.DataItem,"RoleName")%>”，确认吗？');">删除</a> </ItemTemplate>
        <ItemStyle Width="50" cssClass="center" />
      </asp:TemplateColumn>
    </Columns>
  </asp:dataGrid>

  <table class="table table-noborder">
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
