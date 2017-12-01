<%@ Page Language="C#" ValidateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalStoreCategoryAdd" Trace="false" %>
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
<asp:Button id="BtnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table noborder table-hover">
	<asp:PlaceHolder ID="PhParentID" runat="server">
	<tr>
	  <td width="120">上级类别：</td>
	  <td><asp:DropDownList ID="ParentID" runat="server"> </asp:DropDownList></td>
	</tr>
    </asp:PlaceHolder>
    <tr>
	  <td>属性名称：</td>
	  <td>
	  	<asp:TextBox Columns="25" MaxLength="50" id="CategoryName" runat="server" />
		<asp:RequiredFieldValidator ControlToValidate="CategoryName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
		<asp:RegularExpressionValidator runat="server" ControlToValidate="CategoryName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
	  </td>
	</tr>
  </table>

</form>
</body>
</html>
