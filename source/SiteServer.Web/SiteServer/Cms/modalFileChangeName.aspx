<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalFileChangeName" Trace="false"%>
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

  <script language="javascript">
  document.ready(function(){
    document.getElementById('<%=FileName.ClientID%>').focus();
  });
  </script>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="100"><bairong:help HelpText="文件名" Text="文件名：" runat="server" ></bairong:help></td>
      <td><asp:Literal ID="ltlFileName" runat="server"></asp:Literal></td>
    </tr>
    <tr>
      <td width="100"><bairong:help HelpText="新文件名" Text="新文件名：" runat="server" ></bairong:help></td>
      <td><asp:TextBox Columns="25" MaxLength="50" id="FileName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="FileName" ErrorMessage="文件名称不能为空" foreColor="red" Display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator ID="FileNameValidator" runat="server" ControlToValidate="FileName"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
  </table>

</form>
</body>
</html>
