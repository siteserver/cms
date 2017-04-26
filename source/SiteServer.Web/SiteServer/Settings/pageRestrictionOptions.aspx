<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageRestrictionOptions" %>
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

  <div class="popover popover-static">
  <h3 class="popover-title">访问限制选项</h3>
  <div class="popover-content">

    <table class="table noborder table-hover">
      <tr>
        <td width="120">访问限制选项：</td>
        <td>
          <asp:RadioButtonList ID="RestrictionType" RepeatDirection="Vertical" runat="server"></asp:RadioButtonList>
        </td>
      </tr>
    </table>

    <hr />
    <table class="table noborder">
      <tr>
        <td class="center">
          <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
        </td>
      </tr>
    </table>

    </div>
  </div>

</form>
</body>
</html>
