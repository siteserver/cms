<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTableStyleValidateAdd" Trace="false" %>
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
      <td width="130"><bairong:help HelpText="设置是否对此字段启用表单验证" Text="是否启用表单验证：" runat="server" ></bairong:help></td>
      <td><asp:RadioButtonList id="IsValidate" RepeatDirection="Horizontal" class="noborder" OnSelectedIndexChanged="Validate_SelectedIndexChanged" AutoPostBack="true" runat="server">
          <asp:ListItem Text="启用" />
          <asp:ListItem Text="不启用" Selected="True" />
        </asp:RadioButtonList></td>
    </tr>
    <asp:PlaceHolder ID="phValidate" runat="server">
      <tr>
        <td><bairong:help HelpText="是否在表单界面中必须填写此项。" Text="是否为必填项：" runat="server" ></bairong:help></td>
        <td><asp:RadioButtonList id="IsRequired" RepeatDirection="Horizontal" class="noborder" runat="server">
            <asp:ListItem Text="是" />
            <asp:ListItem Text="否" Selected="True" />
          </asp:RadioButtonList></td>
      </tr>
      <asp:PlaceHolder ID="phNum" runat="server">
        <tr>
          <td><bairong:help HelpText="设置可以添加的最小字符数" Text="最小字符数：" runat="server" ></bairong:help></td>
          <td><asp:TextBox class="input-mini" MaxLength="50" Text="0" id="MinNum" runat="server" />
            个字符
            <asp:RegularExpressionValidator
					ControlToValidate="MinNum"
					ValidationExpression="\d+"
					Display="Dynamic"
					errorMessage=" *" foreColor="red" 
					runat="server"/>
            （0代表不限制） </td>
        </tr>
        <tr>
          <td><bairong:help HelpText="设置可以添加的最大字符数" Text="最大字符数：" runat="server" ></bairong:help></td>
          <td><asp:TextBox class="input-mini" MaxLength="50" Text="0" id="MaxNum" runat="server" />
            个字符
            <asp:RegularExpressionValidator
					ControlToValidate="MaxNum"
					ValidationExpression="\d+"
					Display="Dynamic"
					errorMessage=" *" foreColor="red" 
					runat="server"/>
            （0代表不限制）</td>
        </tr>
      </asp:PlaceHolder>
      <tr>
        <td><bairong:help HelpText="设置表单高级验证" Text="高级验证：" runat="server" ></bairong:help></td>
        <td><asp:DropDownList ID="ValidateType" OnSelectedIndexChanged="Validate_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList></td>
      </tr>
      <asp:PlaceHolder ID="phRegExp" runat="server">
        <tr>
          <td><bairong:help HelpText="设置验证正则表达式" Text="验证正则表达式：" runat="server" ></bairong:help></td>
          <td><asp:TextBox Columns="60" TextMode="MultiLine" id="RegExp" runat="server" /></td>
        </tr>
      </asp:PlaceHolder>
      <tr>
        <td><bairong:help HelpText="设置验证失败提示信息" Text="验证失败提示信息：" runat="server" ></bairong:help></td>
        <td><asp:TextBox Columns="60" TextMode="MultiLine" id="ErrorMessage" runat="server" />
          <br />
          (不设置系统将使用默认提示)</td>
      </tr>
    </asp:PlaceHolder>
  </table>

</form>
</body>
</html>
