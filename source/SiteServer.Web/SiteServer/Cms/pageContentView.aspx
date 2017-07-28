<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageContentView" %>
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
  <bairong:alerts runat="server" />

  <asp:Literal ID="ltlScripts" runat="server"></asp:Literal>

  <table class="table table-bordered table-striped">
    <tr>
      <td width="150">栏目名：</td>
      <td colspan="2"><asp:Literal ID="ltlNodeName" runat="server"/></td>
    </tr>
    <asp:Repeater ID="MyRepeater" runat="server">
      <itemtemplate>
        <asp:Literal id="ltlHtml" runat="server" />
      </itemtemplate>
    </asp:Repeater>
    <tr id="RowTags" runat="server">
      <td>内容标签：</td>
      <td colspan="2"><asp:Literal id="ltlTags" runat="server"/></td>
    </tr>
    <tr id="RowContentGroup" runat="server">
      <td>所属内容组：</td>
      <td colspan="2"><asp:Literal ID="ltlContentGroup" runat="server"/></td>
    </tr>
    <tr>
      <td>最后修改日期：</td>
      <td colspan="2"><asp:Literal id="ltlLastEditDate" runat="server" /></td>
    </tr>
    <tr>
      <td>添加人：</td>
      <td colspan="2"><asp:Literal id="ltlAddUserName" runat="server" /></td>
    </tr>
    <tr>
      <td>最后修改人：</td>
      <td colspan="2"><asp:Literal id="ltlLastEditUserName" runat="server" /></td>
    </tr>
    <tr>
      <td>状态：</td>
      <td colspan="2"><asp:Literal ID="ltlContentLevel" runat="server"/></td>
    </tr>
  </table>

  <hr />
  <table class="table noborder">
    <tr>
      <td class="center">
        <asp:Button class="btn btn-primary" id="Submit" text="审 核" runat="server"/>
        <input class="btn btn-info" type="button" onClick="submitPreview();" value="预 览" />
        <input class="btn" type="button" onClick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
