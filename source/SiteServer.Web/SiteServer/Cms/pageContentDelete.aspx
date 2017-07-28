<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageContentDelete" %>
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
    <h3 class="popover-title">删除内容</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="160">需要删除的内容：</td>
          <td><asp:Literal ID="ltlContents" runat="server"></asp:Literal></td>
        </tr>
        <tr id="RetainRow" runat="server">
          <td width="120">是否保留文件：</td>
          <td>
            <asp:RadioButtonList ID="RetainFiles" runat="server" RepeatDirection="Horizontal" class="noborder">
              <asp:ListItem Text="保留生成的文件" Value="true"></asp:ListItem>
              <asp:ListItem Text="删除生成的文件" Value="false" Selected="true"></asp:ListItem>
            </asp:RadioButtonList>
            <span>选择保留文件则此操作将仅在数据库中删除内容。</span>
          </td>
        </tr>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-danger" id="Submit" text="确 定" OnClick="Submit_OnClick" runat="server"/>
            <asp:Button class="btn" id="Return" text="返 回" CausesValidation="false" OnClick="Return_OnClick" runat="server"/>
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
