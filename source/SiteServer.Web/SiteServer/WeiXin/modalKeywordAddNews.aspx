<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.WeiXin.ModalKeywordAddNews" Trace="false"%>
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
      <td width="120">关键词：</td>
      <td colspan="3">
        <asp:TextBox id="TbKeywords" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="TbKeywords" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <span class="gray">多个关键词请用空格格开：例如: 微信 腾讯</span>
      </td>
    </tr>
    <tr>
      <td>匹配规则：</td>
      <td width="240">
        <asp:DropDownList id="DdlMatchType" runat="server" />
      </td>
      <td width="80">是否启用：</td>
      <td class="checkbox">
        <asp:CheckBox id="CbIsEnabled" text="启用关键字"  runat="server" />
      </td>
    </tr>
    <asp:PlaceHolder id="PhSelect" visible="false" runat="server">
    <tr>
      <td>从微官网选择：</td>
      <td class="checkbox" colspan="3">
        <asp:CheckBox id="CbIsSelect" checked="true" text="选择微官网内容" runat="server" />
      </td>
    </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
