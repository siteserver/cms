<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentCrossSiteTrans" Trace="false"%>
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
      <td width="100">选择站点：</td>
      <td>
        <asp:DropDownList ID="PublishmentSystemIDDropDownList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="PublishmentSystemID_SelectedIndexChanged"></asp:DropDownList>
      </td>
    </tr>
    <tr>
      <td>转发到：</td>
      <td>
        <asp:ListBox ID="NodeIDListBox" style="height:200px;" SelectionMode="Multiple" runat="server"></asp:ListBox>
        <asp:RequiredFieldValidator
          ControlToValidate="NodeIDListBox"
          ErrorMessage=" *" foreColor="red"
          Display="Dynamic"
          runat="server" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
