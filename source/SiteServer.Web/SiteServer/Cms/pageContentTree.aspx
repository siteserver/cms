<%@ Page Language="C#" Trace="false" EnableViewState="false" Inherits="SiteServer.BackgroundPages.BasePageCms" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form runat="server">
	<table class="table noborder table-condensed table-hover">
		<tr class="info thead">
		  <td onclick="location.reload();">
		  	栏目列表
		  </td>
		</tr>
	  <bairong:NodeTree runat="server"></bairong:NodeTree>
	</table>
</form>
</body>
</html>
