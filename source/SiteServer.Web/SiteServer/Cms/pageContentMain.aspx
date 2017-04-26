<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.BasePageCms" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <script type="text/javascript" src="../assets/jquery/jquery-1.9.1.min.js"></script>
    <script type="text/javascript" src="../assets/main.js"></script>
</head>
<frameset id="frame" framespacing="0" border="false" cols="180,*" frameborder="0" scrolling="yes">
	<frame name="tree" scrolling="auto" marginwidth="0" marginheight="0" src="pageContentTree.aspx?PublishmentSystemID=<%=PublishmentSystemId%>&RightPageURL=pageContent.aspx" >
	<frame name="content" scrolling="auto" marginwidth="0" marginheight="0" src="../pageBlank.html">
</frameset>
<noframes>
<body>
<p>This page uses frames, but your browser doesn't support them.</p>
</body>
</noframes>
</html>

