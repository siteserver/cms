<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCreateChannels" Trace="false"%>
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

  <table class="table table-noborder table-hover">
    <tr>
      <td width="140">是否生成下级栏目：</td>
      <td><asp:RadioButtonList ID="IsIncludeChildren" RepeatDirection="Horizontal" class="noborder" runat="server">
          <asp:ListItem Text="生成下级栏目" Value="True"></asp:ListItem>
          <asp:ListItem Text="仅生成选中栏目" Value="False" Selected="true"></asp:ListItem>
        </asp:RadioButtonList></td>
    </tr>
    <tr>
      <td>是否生成内容页：</td>
      <td><asp:RadioButtonList ID="IsCreateContents" RepeatDirection="Horizontal" class="noborder" runat="server">
          <asp:ListItem Text="生成内容页" Value="True"></asp:ListItem>
          <asp:ListItem Text="不生成内容页" Value="False" Selected="true"></asp:ListItem>
        </asp:RadioButtonList></td>
    </tr>
  </table>

</form>
</body>
</html>
