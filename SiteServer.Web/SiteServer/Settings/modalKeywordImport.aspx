<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.ModalKeywordImport" validateRequest="false" trace="false" %>
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
<bairong:alerts text="敏感词导入规则：以英文逗号(,)分隔敏感词，如果需要替换用竖线(|)分隔，如：敏感词1,敏感词2,敏感词3|替换词3,敏感词4" runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="60"> 等级： </td>
      <td><asp:DropDownList ID="ddlGrade" runat="server"></asp:DropDownList></td>
    </tr>
    <tr>
      <td colspan="2" class="center"><asp:TextBox style="width:98%;height:230px" TextMode="MultiLine" id="tbKeywords" runat="server"/></td>
    </tr>
  </table>

</form>
</body>
</html>
