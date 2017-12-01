<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.ModalRelatedFieldItemEdit" Trace="false"%>
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
      <td><nobr>
        <bairong:help HelpText="字段项名称" Text="字段项名：" runat="server" ></bairong:help>
        </nobr></td>
      <td>
        <asp:TextBox Width="160" id="ItemName" runat="server"/>
        <asp:RequiredFieldValidator
          ControlToValidate="ItemName"
          errorMessage=" *" foreColor="red" 
          Display="Dynamic"
          runat="server"/></td>
    </tr>
    <tr>
      <td><nobr>
        <bairong:help HelpText="字段项值" Text="字段项值：" runat="server" ></bairong:help>
        </nobr></td>
      <td>
        <asp:TextBox Width="160" id="ItemValue" runat="server"/>
        <asp:RequiredFieldValidator
          ControlToValidate="ItemValue"
          errorMessage=" *" foreColor="red" 
          Display="Dynamic"
          runat="server"/></td>
    </tr>
  </table>

</form>
</body>
</html>
