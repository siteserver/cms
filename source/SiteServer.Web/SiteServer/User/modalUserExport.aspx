<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.User.ModalUserExport" %>

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
<bairong:alerts text="在此导出用户数据至Excel中" runat="server"></bairong:alerts>

  <asp:PlaceHolder ID="PhExport" runat="server">
    <table class="table table-noborder table-hover">
      <tr>
        <td class="center" valign="top" ><table class="center" width="95%">
            <tr>
              <td>用户类型：</td>
              <td><asp:RadioButtonList ID="RblCheckedState" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList></td>
            </tr>
          </table></td>
      </tr>
    </table>
  </asp:PlaceHolder>

</form>
</body>
</html>
