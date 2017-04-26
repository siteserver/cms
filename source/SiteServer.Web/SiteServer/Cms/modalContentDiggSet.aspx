<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentDiggSet" Trace="false"%>
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
<bairong:alerts text="在此可以设置指定内容的赞同/不赞同数目" runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="140"><bairong:help HelpText="赞同数" Text="赞同数：" runat="server" ></bairong:help></td>
      <td><asp:TextBox class="input-mini" MaxLength="50" id="GoodNum" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="GoodNum" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator ControlToValidate="GoodNum" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="赞同数必须为数字" foreColor="red" runat="server"/></td>
    </tr>
    <tr>
      <td width="140"><bairong:help HelpText="不赞同数" Text="不赞同数：" runat="server" ></bairong:help></td>
      <td><asp:TextBox class="input-mini" MaxLength="50" Text="0" id="BadNum" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="BadNum" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator ControlToValidate="BadNum" ValidationExpression="[\d\.]+" Display="Dynamic" ErrorMessage="不赞同数必须为数字,可以带小数点" foreColor="red" runat="server"/></td>
    </tr>
  </table>

</form>
</body>
</html>
