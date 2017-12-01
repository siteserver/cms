<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.WeiXin.PageStoreCategory" %>
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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>属性名称</td>
      <td width="30">上升</td>
      <td width="30">下降</td>
      <td width="50">&nbsp;</td>
      <td width="20"><input onclick="_checkFormAll(this.checked)" type="checkbox" /></td>
    </tr>
    <asp:Repeater ID="RptContents" runat="server">
      <itemtemplate>
          <tr>
            <asp:Literal id="LtlHtml" runat="server" />
          </tr>
      </itemtemplate>
    </asp:Repeater>
  </table>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="BtnAdd" Text="新 增" runat="server" />
    <asp:Button class="btn" id="BtnDelete" Text="删 除" runat="server" />
  </ul>

</form>
</body>
</html>
