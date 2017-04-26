<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalRestrictionAdd" Trace="false"%>
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
<bairong:alerts text="xxx.xxx.xxx.xxx = 精确匹配<br>xxx.xxx.xxx.xxx-xxx.xxx.xxx.xxx = 范围<br>xxx.xxx.xxx.* = 任何匹配" runat="server"></bairong:alerts>

  <table class="table noborder table-hover">
    <tr>
      <td width="100">IP访问规则：</td>
      <td>
        <asp:TextBox Columns="50" MaxLength="50" id="Restriction" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="Restriction" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Restriction" ValidationExpression="[\d{0,3}\.\*-]+" errorMessage=" *" foreColor="red" display="Dynamic" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
