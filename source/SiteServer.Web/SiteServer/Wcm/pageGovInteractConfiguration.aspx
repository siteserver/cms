<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovInteractConfiguration" %>
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
  <bairong:alerts text="如果未设置主题分类根栏目，系统将默认生成，主题分类根栏目只能选择首页或者一级栏目" runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">互动交流设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="200">主题分类根栏目：</td>
          <td><asp:DropDownList ID="ddlGovInteractNodeID" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
          <td width="200">办件是否新窗口打开：</td>
          <td><asp:RadioButtonList ID="rblGovInteractApplyIsOpenWindow" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList></td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
