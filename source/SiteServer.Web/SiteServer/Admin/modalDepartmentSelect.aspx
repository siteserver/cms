<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Admin.ModalDepartmentSelect" Trace="false"%>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table cellpadding="3" width="95%" class="center">
    <tr>
      <td height="30"><table cellspacing="1" cellpadding="1" class="center" border="0" id="MyDataGrid" style="width:100%;">
          <tr class="summary-title" class="center" style="height:22px;">
            <td align="Left">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;分类名</td>
          </tr>
          <tr treeItemLevel="2">
            <td nowrap><img align="absmiddle" style="cursor:pointer" onClick="displayChildren(this);" isAjax="false" isOpen="true" src="../assets/icons/tree/minus.gif" /><img align="absmiddle" border="0" src="../assets/icons/tree/category.gif" />&nbsp;部门列表</td>
          </tr>
          <asp:Repeater ID="rptCategory" runat="server">
            <itemtemplate>
              <asp:Literal id="ltlHtml" runat="server" />
            </itemtemplate>
          </asp:Repeater>
        </table></td>
    </tr>
  </table>

</form>
</body>
</HTML>
