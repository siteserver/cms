<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageTemplateLog" %>
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
      <td width="30">序号</td>
      <td>修订人</td>
      <td>修订时间</td>
      <td>字符数</td>
      <td width="60"></td>
      <td width="20">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
          <tr>
            <td class="center"><asp:Literal ID="ltlIndex" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlAddUserName" runat="server"></asp:Literal></td>
            <td><asp:Literal ID="ltlAddDate" runat="server"></asp:Literal></td>
            <td>
              <asp:Literal ID="ltlContentLength" runat="server"></asp:Literal>
            </td>
            <td class="center">
              <asp:Literal ID="ltlView" runat="server"></asp:Literal></td>
            <td class="center">
              <input type="checkbox" name="IDCollection" value='<%#DataBinder.Eval(Container.DataItem, "ID")%>' />
            </td>
          </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <bairong:sqlPager id="spContents" runat="server" class="table table-pager" />

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success hide" id="btnCompare" Text="修订版本对比" runat="server" />
    <asp:Button class="btn" id="btnDelete" Text="删 除" runat="server" />
  </ul>

</form>
</body>
</html>
