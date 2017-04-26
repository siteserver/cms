<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalContentStarSet" Trace="false"%>
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
<bairong:alerts text="在此可以设置指定内容的总评分人数以及平均评分值，0代表将取消设置" runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="140">总评分人数：</td>
      <td><asp:TextBox class="input-mini" MaxLength="50" Text="0" id="TotalCount" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="TotalCount" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator ControlToValidate="TotalCount" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="总评分人数必须为数字" foreColor="red" runat="server"/></td>
    </tr>
    <tr>
      <td width="140">平均评分值：</td>
      <td><asp:TextBox class="input-mini" MaxLength="50" Text="0" id="PointAverage" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="PointAverage" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator ControlToValidate="PointAverage" ValidationExpression="[\d\.]+" Display="Dynamic" ErrorMessage="平均评分值必须为数字,可以带小数点" foreColor="red" runat="server"/></td>
    </tr>
  </table>

</form>
</body>
</html>
