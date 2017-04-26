<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovInteractChannel" %>
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
  <bairong:alerts text="互动交流提交标签为&amp;lt;stl:govInteractApply channelIndex=&quot;栏目索引&quot;&gt;&amp;lt;/stl:govInteractApply&gt;，互动交流查询标签为&amp;lt;stl:govInteractQuery channelIndex=&quot;栏目索引&quot;&gt;&amp;lt;/stl:govInteractQuery&gt;。" runat="server" />

  <table class="table table-bordered table-hover">
    <tr class="info thead">
      <td>名称</td>
      <td>简介</td>
      <td width="30">上升</td>
      <td width="30">下降</td>
      <td width="100"></td>
      <td width="50">&nbsp;</td>
      <td width="20"></td>
    </tr>
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <asp:Literal id="ltlHtml" runat="server" />
      </itemtemplate>
    </asp:Repeater>
  </table>

  <ul class="breadcrumb breadcrumb-button">
    <asp:Button class="btn btn-success" id="AddChannel" Text="添 加" runat="server" />
    <asp:Button class="btn" id="Delete" Text="删 除" runat="server" />
  </ul>

</form>
</body>
</html>
