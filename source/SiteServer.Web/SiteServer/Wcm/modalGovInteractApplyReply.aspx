<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovInteractApplyReply" Trace="false"%>
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
<bairong:alerts text="办理办件后信息将变为待审核状态" runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="120">答复内容：</td>
      <td>
      	<asp:TextBox ID="tbReply" runat="server" TextMode="MultiLine" style="width:90%;height:120px;"></asp:TextBox>
          <asp:RequiredFieldValidator
          ControlToValidate="tbReply"
          errorMessage=" *" foreColor="red" 
          Display="Dynamic"
          runat="server"/>
      </td>
    </tr>
    <tr>
      <td>答复部门：</td>
      <td><asp:Literal ID="ltlDepartmentName" runat="server"></asp:Literal></td>
    </tr>
    <tr>
      <td>答复人：</td>
      <td><asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
    </tr>
    <tr>
      <td>附件上传：</td>
      <td><input id="htmlFileUrl" runat="server" type="file" style="width:330px;" /></td>
    </tr>
  </table>

</form>
</body>
</html>
