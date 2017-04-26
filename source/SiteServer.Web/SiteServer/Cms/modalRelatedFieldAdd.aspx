<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalRelatedFieldAdd" Trace="false"%>
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
<bairong:alerts text="前缀及后缀为联动字段显示时在下拉列表之前及之后显示的文字，可以为空" runat="server"></bairong:alerts>

  <table class="table table-noborder table-hover">
    <tr>
      <td width="38%"><bairong:help HelpText="联动字段的名称" Text="联动字段名称：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="RelatedFieldName" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="RelatedFieldName" errorMessage=" *" foreColor="red" display="Dynamic"
						runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="RelatedFieldName"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="38%"><bairong:help HelpText="设置联动字段几级联动" Text="几级联动：" runat="server" ></bairong:help></td>
      <td>
      	<asp:DropDownList ID="TotalLevel" AutoPostBack="true" OnSelectedIndexChanged="TotalLevel_SelectedIndexChanged" runat="server">
        	<asp:ListItem Text="一级" Value="1"></asp:ListItem>
            <asp:ListItem Text="二级" Value="2" Selected="true"></asp:ListItem>
            <asp:ListItem Text="三级" Value="3"></asp:ListItem>
            <asp:ListItem Text="四级" Value="4"></asp:ListItem>
            <asp:ListItem Text="五级" Value="5"></asp:ListItem>
        </asp:DropDownList>
      </td>
    </tr>
    <tr>
      <td width="38%"><bairong:help HelpText="一级前缀字符串" Text="一级前缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Prefix1" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Prefix1"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="38%"><bairong:help HelpText="一级后缀字符串" Text="一级后缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Suffix1" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Suffix1"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <asp:PlaceHolder ID="phFix2" runat="server">
    <tr>
      <td width="38%"><bairong:help HelpText="二级前缀字符串" Text="二级前缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Prefix2" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Prefix2"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="38%"><bairong:help HelpText="二级后缀字符串" Text="二级后缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Suffix2" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Suffix2"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phFix3" runat="server">
    <tr>
      <td width="38%"><bairong:help HelpText="三级前缀字符串" Text="三级前缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Prefix3" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Prefix3"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="38%"><bairong:help HelpText="三级后缀字符串" Text="三级后缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Suffix3" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Suffix3"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phFix4" runat="server">
    <tr>
      <td width="38%"><bairong:help HelpText="四级前缀字符串" Text="四级前缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Prefix4" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Prefix4"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="38%"><bairong:help HelpText="四级后缀字符串" Text="四级后缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Suffix4" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Suffix4"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phFix5" runat="server">
    <tr>
      <td width="38%"><bairong:help HelpText="五级前缀字符串" Text="五级前缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Prefix5" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Prefix5"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    <tr>
      <td width="38%"><bairong:help HelpText="五级后缀字符串" Text="五级后缀字符串：" runat="server" ></bairong:help></td>
      <td width="62%"><asp:TextBox Columns="25" MaxLength="50" id="Suffix5" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="Suffix5"
						ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" /></td>
    </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
