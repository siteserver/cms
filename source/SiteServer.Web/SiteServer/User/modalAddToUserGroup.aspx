<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.User.ModalAddToUserGroup" Trace="false"%>

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
      <td width="100">用户组名称：</td>
      <td><asp:DropDownList ID="UserGroupIDDropDownList" runat="server"/></td>
    </tr>
  </table>

</form>
</body>
</html>
