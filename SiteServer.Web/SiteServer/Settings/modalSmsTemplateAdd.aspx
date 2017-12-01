<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalSmsTemplateAdd" %>
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

<table class="table noborder table-hover">
  <tr>
    <td>模板ID：</td>
    <td>
      <asp:TextBox Columns="25" MaxLength="50" id="TbTplId" runat="server" />
      <asp:RequiredFieldValidator ControlToValidate="TbTplId" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
      <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTplId" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
    </td>
  </tr>
</table>

</form>
</body>
</html>
