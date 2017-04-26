<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalGatherDatabaseSet" Trace="false"%>
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
      <td width="110"><bairong:help HelpText="选择栏目，采集到的内容将添加到此栏目中" Text="采集到栏目：" runat="server" ></bairong:help></td>
      <td><asp:DropDownList ID="NodeIDDropDownList" runat="server"></asp:DropDownList></td>
    </tr>
    <tr>
      <td><bairong:help HelpText="需要采集的内容数，0代表采集所有内容" Text="采集内容数：" runat="server" ></bairong:help></td>
      <td><asp:TextBox class="input-mini" MaxLength="4" id="GatherNum" Style="text-align:right" Text="10" runat="server"/>
        <span class="gray">（0代表不限定）</span>
        <asp:RequiredFieldValidator
					ControlToValidate="GatherNum"
					errorMessage=" *" foreColor="red" 
					Display="Dynamic"
					runat="server"/>
        <asp:RegularExpressionValidator
					ControlToValidate="GatherNum"
					ValidationExpression="\d+"
					ErrorMessage="采集数只能是数字"
          foreColor="red"
					Display="Dynamic"
					runat="server"/></td>
    </tr>
  </table>

</form>
</body>
</html>
