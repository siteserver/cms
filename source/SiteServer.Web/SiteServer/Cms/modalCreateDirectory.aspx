<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalCreateDirectory" Trace="false"%>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body onLoad="document.getElementById('<%=DirectoryName.ClientID%>').focus();">
<!--#include file="../inc/openWindow.html"-->
<form class="form-inline" runat="server">
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="100">文件夹名称：</td>
      <td>
      	<asp:TextBox Columns="18" MaxLength="50" id="DirectoryName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="DirectoryName" ErrorMessage="文件夹名称不能为空" foreColor="red" Display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator ID="DirectoryNameValidator" runat="server" ControlToValidate="DirectoryName" ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" />
      </td>
    </tr>
  </table>

</form>
</body>
</html>
