<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationCreateTrigger" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts text="在此设置各栏目生成页面的规则，同时可以设置当栏目下内容改变后需要的生成栏目" runat="server"></bairong:alerts>

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>栏目名</td>
      <td>内容变动时需要生成的栏目</td>
      <td width="80">&nbsp;</td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <asp:Literal id="ltlHtml" runat="server" />
      </itemtemplate>
    </asp:Repeater>
  </table>

</form>
</body>
</html>
