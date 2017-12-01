<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentTaxis" Trace="false"%>
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
        <td width="120">排序方向：</td>
        <td>
          <asp:RadioButtonList ID="RblTaxisType" RepeatDirection="Horizontal" class="noborder" runat="server"></asp:RadioButtonList>
        </td>
      </tr>
      <tr>
        <td width="120">移动数目：</td>
        <td>
          <asp:TextBox class="input-mini" Text="1" MaxLength="50" id="TbTaxisNum" runat="server" />
          <asp:RequiredFieldValidator ControlToValidate="TbTaxisNum" errorMessage=" *" foreColor="red" Display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbTaxisNum" ValidationExpression="^([1-9]|[1-9][0-9]{1,})$"
            errorMessage=" *" foreColor="red" Display="Dynamic" />
        </td>
      </tr>
    </table>

  </form>
</body>

</html>