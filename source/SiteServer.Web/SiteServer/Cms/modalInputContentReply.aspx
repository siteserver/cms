<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalInputContentReply" %>
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
    <asp:Repeater ID="rptContents" runat="server">
      <itemtemplate>
        <tr>
          <td width="80">
            <asp:Literal id="ltlDataKey" runat="server" />：
          </td>
          <td><asp:Literal id="ltlDataValue" runat="server" /></td>
        </tr>
      </itemtemplate>
    </asp:Repeater>
    <tr>
      <td colspan="2"><bairong:UEditor id="breReply" runat="server"></bairong:UEditor></td>
    </tr>
  </table>

</form>
</body>
</html>
