<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicCategory" %>
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
      <td>名称</td>
      <td style="width:200px;">代码</td>
      <td class="center" style="width:30px;">上升</td>
      <td class="center" style="width:30px;">下降</td>
      <td class="center" style="width:50px;">&nbsp;</td>
      <td width="20" class="center">
        <input onclick="_checkFormAll(this.checked)" type="checkbox" />
      </td>
    </tr>
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
        <asp:Literal id="ltlHtml" runat="server" />
      </itemtemplate>
    </asp:Repeater>
  </table>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="BtnAddChannel" Text="添 加" runat="server" />
    <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
  </ul>

</form>
</body>
</html>
