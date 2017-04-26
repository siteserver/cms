<%@ Page Language="C#" Trace="false" EnableViewState="false" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicContentTree" %>
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
    <tr class="info">
      <td style="padding-left:60px;">
        <a href="javascript:;" onclick="location.reload();" title="点击刷新栏目树">信息管理</a></td>
    </tr>
  </table>

  <table class="table noborder table-condensed table-hover">
    <tr treeItemLevel="2">
      <td nowrap>
        <img align="absmiddle" style="cursor:pointer" onClick="displayChildren(this);" isAjax="false" isOpen="true" src="../assets/icons/tree/minus.gif" /><img align="absmiddle" border="0" src="../assets/icons/tree/category.gif" />
        &nbsp;<asp:Literal id="LtlCategoryChannel" runat="server" />
      </td>
    </tr>
    <asp:Repeater ID="RptCategoryChannel" runat="server">
      <itemtemplate>
        <asp:Literal id="ltlHtml" runat="server" />
      </itemtemplate>
    </asp:Repeater>
  </table>

  <table class="table noborder table-condensed table-hover">
    <tr treeItemLevel="0">
      <td nowrap><img align="absmiddle" style="cursor:pointer" onClick="displayChildren_Department(this);" isAjax="true" isOpen="false" id="0" src="../assets/icons/tree/plus.gif" /><img align="absmiddle" src="../assets/icons/tree/category.gif" />&nbsp;机构分类</td>
    </tr>
  </table>

  <asp:Repeater ID="RptCategoryClass" runat="server">
    <itemtemplate>
      <table class="table noborder table-condensed table-hover">
        <tr treeItemLevel="0">
          <td nowrap><asp:Literal ID="ltlPlusIcon" runat="server"></asp:Literal><img align="absmiddle" src="../assets/icons/tree/category.gif" />&nbsp;<asp:Literal id="ltlClassName" runat="server" />分类</td>
        </tr>
      </table>
    </itemtemplate>
  </asp:Repeater>

</form>
</body>
</html>
