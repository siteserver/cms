<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.PageUserCheck" %>
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

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>账号</td>
      <td>姓名</td>
      <td>用户组</td>
      <td>注册时间</td>
      <td width="50">&nbsp;</td>
      <td width="20">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
          <tr>
            <td><asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlDisplayName" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlUserGroupName" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlCreateDate" runat="server"></asp:Literal></td>
            <td class="center">
              <asp:HyperLink ID="hlEditLink" Text="编辑" runat="server"></asp:HyperLink>
            </td>
            <td class="center"><asp:Literal ID="ltlSelect" runat="server"></asp:Literal></td>
          </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="spContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="Check" Text="审核通过" runat="server" />
    <asp:Button class="btn" id="Delete" Text="删 除" runat="server" />
  </ul>

</form>
</body>
</html>
