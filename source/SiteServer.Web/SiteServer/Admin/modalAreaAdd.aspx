<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.ModalAreaAdd" %>
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
	<asp:PlaceHolder ID="phParentID" runat="server">
	<tr>
	  <td width="120">上级区域：</td>
	  <td><asp:DropDownList ID="ParentID" runat="server"> </asp:DropDownList></td>
	</tr>
    </asp:PlaceHolder>
    <tr>
	  <td>区域名称：</td>
	  <td>
	  	<asp:TextBox Columns="25" MaxLength="50" id="AreaName" runat="server" />
		<asp:RequiredFieldValidator ControlToValidate="AreaName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
		<asp:RegularExpressionValidator runat="server" ControlToValidate="AreaName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
	  </td>
	</tr>
  </table>

</form>
</body>
</html>
