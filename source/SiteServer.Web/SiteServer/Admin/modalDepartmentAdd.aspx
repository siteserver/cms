<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.ModalDepartmentAdd" %>
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
	  <td width="130">部门名称：</td>
	  <td>
	  	<asp:TextBox Columns="25" MaxLength="50" id="DepartmentName" runat="server" />
		<asp:RequiredFieldValidator ControlToValidate="DepartmentName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
		<asp:RegularExpressionValidator runat="server" ControlToValidate="DepartmentName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
	  </td>
	</tr>
    <tr>
	  <td>部门编号：</td>
	  <td>
	  	<asp:TextBox Columns="25" MaxLength="50" id="Code" runat="server" />
		<asp:RequiredFieldValidator ControlToValidate="Code" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
		<asp:RegularExpressionValidator runat="server" ControlToValidate="Code" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
	  </td>
	</tr>
    <asp:PlaceHolder ID="phParentID" runat="server">
	<tr>
	  <td>上级部门：</td>
	  <td>
	  	<asp:DropDownList ID="ParentID" runat="server"> </asp:DropDownList>
	  </td>
	</tr>
    </asp:PlaceHolder>
	<tr>
	  <td>部门简介：</td>
	  <td>
	  	<asp:TextBox Columns="60" Rows="4" TextMode="MultiLine" id="Summary" runat="server" />
	  </td>
	</tr>
  </table>

</form>
</body>
</html>
