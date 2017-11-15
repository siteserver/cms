<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalCouponRelated" Trace="false"%>
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
      <td width="120">关联优惠劵：</td>
      <td>
        <asp:CheckBoxList id="CblCoupon" class="checkboxlist" runat="server" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
