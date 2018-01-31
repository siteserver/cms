<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.BasePageCms" %>
  <!DOCTYPE html>
  <html>

  <head>
    <meta charset="utf-8">
  </head>
  <frameset framespacing="0" border="false" cols="180,*" frameborder="0" scrolling="yes">
    <frame scrolling="auto" marginwidth="0" marginheight="0" src="pageTemplateLeft.aspx?SiteID=<%=SiteId%>"
    />
    <frame id="management" name="management" scrolling="auto" marginwidth="0" marginheight="0" src="pageTemplate.aspx?SiteID=<%=SiteId%>"
    />
  </frameset>
  <noframes></noframes>

  </html>