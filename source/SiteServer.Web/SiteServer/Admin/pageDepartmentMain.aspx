<%@ Page Language="C#" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
</head>
<frameset id="frame" framespacing="0" border="false" cols="180,*" frameborder="0" scrolling="yes">
	<frame name="tree" scrolling="auto" marginwidth="0" marginheight="0" src="pageDepartmentTree.aspx?module=<%=Request.QueryString["module"]%>&amp;publishmentSystemID=<%=Request.QueryString["publishmentSystemID"]%>" >
	<frame name="department" scrolling="auto" marginwidth="0" marginheight="0" src="pageAdministrator.aspx?module=<%=Request.QueryString["module"]%>&amp;publishmentSystemID=<%=Request.QueryString["publishmentSystemID"]%>">
</frameset>
<noframes>
<body>
<p>This page uses frames, but your browser doesn't support them.</p>
</body>
</noframes>
</html>
