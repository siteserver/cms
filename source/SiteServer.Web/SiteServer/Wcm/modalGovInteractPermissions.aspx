<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovInteractPermissions" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="80">权限：</td>
      <td><asp:CheckBoxList id="cblPermissions" RepeatDirection="Horizontal" class="noborder" RepeatColumns="3" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
