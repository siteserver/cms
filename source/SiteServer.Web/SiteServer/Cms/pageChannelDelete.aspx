<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageChannelDelete" %>
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

  <div class="popover popover-static">
    <h3 class="popover-title"><asp:Literal id="ltlPageTitle" runat="server"></asp:Literal></h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="150" class="center">是否保留文件：</td>
          <td><asp:RadioButtonList ID="RetainFiles" runat="server" RepeatDirection="Horizontal" class="noborder">
              <asp:ListItem Text="保留生成的文件" Value="true"></asp:ListItem>
              <asp:ListItem Text="删除生成的文件" Value="false" Selected="true"></asp:ListItem>
            </asp:RadioButtonList></td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-danger" id="Delete" text="删 除" OnClick="Delete_OnClick" runat="server"/>
            <input class="btn" type="button" onClick="location.href='<%=ReturnUrl%>';return false;" value="返 回" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
