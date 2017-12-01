<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStyleImport" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" enctype="multipart/form-data" method="post" runat="server">
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

	<table class="table table-noborder table-hover">
	  <tr>
	    <td><bairong:help HelpText="选择需要上传的表样式文件" Text="表样式文件：" runat="server"></bairong:help></td>
	    <td><input type=file  id=myFile size="35" runat="server"/>
	      <asp:RequiredFieldValidator ControlToValidate="myFile" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
	  </tr>
	</table>

</form>
</body>
</html>
