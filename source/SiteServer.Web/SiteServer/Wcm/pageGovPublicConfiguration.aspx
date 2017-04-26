<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicConfiguration" %>
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
  <bairong:alerts text="如果未设置主题分类根栏目，系统将默认生成，主题分类根栏目只能选择首页或者一级栏目。" runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">信息公开设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="240">主题分类根栏目：</td>
          <td><asp:DropDownList ID="ddlGovPublicNodeID" runat="server"></asp:DropDownList></td>
        </tr>
        <tr>
          <td>选择机构分类后自动更改发布机构：</td>
          <td><asp:RadioButtonList id="rblIsPublisherRelatedDepartmentID"
        RepeatDirection="Horizontal" class="noborder"
        runat="server">
              <asp:ListItem Text="是" Value="True" Selected="True" />
              <asp:ListItem Text="否" Value="False" />
            </asp:RadioButtonList></td>
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
