<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalStlTagAdd" Trace="false"%>
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
      <td width="140">自定义模板标签名：</td>
      <td>
        <asp:TextBox Columns="30" id="TagName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="TagName" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" />
      </td>
    </tr>
    <tr>
      <td>自定义模板标签说明：</td>
      <td><asp:TextBox Columns="60" id="Description" runat="server" /></td>
    </tr>
    <tr>
      <td colspan="2">自定义标签：</td>
    </tr>
    <tr>
      <td class="center" colspan="2"><asp:TextBox ID="Content" runat="server" TextMode="MultiLine" Width="520" Height="250"></asp:TextBox>
        <asp:RequiredFieldValidator ControlToValidate="Content" ErrorMessage=" *" foreColor="red" Display="Dynamic" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
