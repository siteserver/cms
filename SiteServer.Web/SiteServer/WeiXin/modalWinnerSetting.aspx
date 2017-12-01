<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalWinnerSetting" Trace="false"%>
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
  
  <link href="css/emotion.css" rel="stylesheet">

  <table class="table table-noborder">
    <tr>
      <td width="120">中奖状态：</td>
      <td>
        <asp:DropDownList id="DdlStatus" class="input-medium" runat="server" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
