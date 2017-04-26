<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalRelatedFieldItemAdd" Trace="false"%>
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
<bairong:alerts text="每一行为一个选项，如果显示项与值不同可以用“|”隔开，左边为显示项，右边为值" runat="server"></bairong:alerts>

	<table class="table table-noborder table-hover">
	  <tr>
	    <td colspan="2" class="center"><asp:TextBox style="width:250px;height:200px" TextMode="MultiLine" id="ItemNames" runat="server"/>
	      <asp:RequiredFieldValidator ControlToValidate="ItemNames" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"/>
	    </td>
	  </tr>
	</table>

</form>
</body>
</html>
