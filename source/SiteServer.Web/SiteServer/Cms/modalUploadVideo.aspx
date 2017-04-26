<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadVideo" Trace="false" %>
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
			<td width="120">选择上传的视频：</td>
			<td>
				<input type=file  id="hifUpload" size="45" runat="server"/> 
				<asp:RequiredFieldValidator ControlToValidate="hifUpload" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
			</td>
		</tr>
	</table>

	<script type="text/javascript" language="javascript">
		<asp:Literal id="ltlScript" runat="server"></asp:Literal>
	</script>

</form>
</body>
</html>
