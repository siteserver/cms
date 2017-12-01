<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.BasePageCms" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
</head>
<frameset framespacing="0" border="false" cols="260,*" frameborder="0" scrolling="yes">
	<frame scrolling="auto" marginwidth="0" marginheight="0" src="pageTagStyleLeft.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&tableStyle=<%=Request.QueryString["tableStyle"]%>&type=<%=Request.QueryString["type"]%>" >
    <frame name="management" scrolling="auto" marginwidth="0" marginheight="0" src="../pageBlank.html">
</frameset><noframes></noframes>
</html>
