<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovInteractApplyTranslate" Trace="false"%>
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
<bairong:alerts text="此操作将吧办件转移到对应的分类中" runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="120">转移到：</td>
      <td><asp:DropDownList ID="ddlNodeID" runat="server"></asp:DropDownList></td>
    </tr>
    <tr>
      <td>意见：</td>
      <td><asp:TextBox ID="tbTranslateRemark" runat="server" TextMode="MultiLine" Columns="60" rows="4"></asp:TextBox></td>
    </tr>
    <tr>
      <td>操作部门：</td>
      <td><asp:Literal ID="ltlDepartmentName" runat="server"></asp:Literal></td>
    </tr>
    <tr>
      <td>操作人：</td>
      <td><asp:Literal ID="ltlUserName" runat="server"></asp:Literal></td>
    </tr>
  </table>

</form>
</body>
</html>
