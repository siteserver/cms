<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalKeywordEdit" Trace="false"%>
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

  <table class="table table-noborder">
    <tr>
      <td width="120">关键词：</td>
      <td><asp:TextBox id="TbKeyword" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="TbKeyword" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
      </td>
    </tr>
    <tr>
      <td>匹配规则：</td>
      <td>
        <asp:DropDownList id="DdlMatchType" runat="server" />
      </td>
    </tr>
    <tr>
      <td>是否启用：</td>
      <td class="checkbox">
        <asp:CheckBox id="CbIsEnabled" text="启用关键字"  runat="server" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
