<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalKeywordPreview" Trace="false"%>
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
<asp:Button id="BtnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td class="center"><h5>请输入微信号，此图文消息将发送至该微信号预览：</h5></td>
    </tr>
    <tr>
      <td class="center">
        <asp:TextBox id="TbWeiXin" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="TbWeiXin" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
