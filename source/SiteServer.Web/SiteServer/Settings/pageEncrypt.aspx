<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Settings.PageEncrypt" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
  <h3 class="popover-title">加密字符串</h3>
  <div class="popover-content">

    <table class="table noborder table-hover">
      <tr>
        <td width="140">需要加密的字符串：</td>
        <td>
          <asp:TextBox TextMode="MultiLine" width="98%" rows="5" id="RawString" runat="server" />
          <asp:RequiredFieldValidator id="RequiredFieldValidator" ControlToValidate="RawString" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
      </tr>
      <tr>
        <td>加密后的字符串：</td>
        <td><asp:Literal ID="ltlString" runat="server" /></td>
      </tr>
    </table>

    <hr />
    <table class="table noborder">
      <tr>
        <td class="center">
          <asp:Button class="btn btn-primary" id="Submit" text="加 密" onclick="Submit_OnClick" runat="server" />
        </td>
      </tr>
    </table>

    </div>
  </div>

</form>
</body>
</html>
