<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.BasePageCms" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
</head>
<frameset framespacing="0" border="false" cols="40%,60%" frameborder="0" scrolling="yes">
	<frame scrolling="auto" marginwidth="0" marginheight="0" src="pageGovPublicCategoryClass.aspx?PublishmentSystemID=<%=PublishmentSystemId%>" >
    <frame name="category" scrolling="auto" marginwidth="0" marginheight="0" src="../pageBlank.html">
</frameset>
</html>
