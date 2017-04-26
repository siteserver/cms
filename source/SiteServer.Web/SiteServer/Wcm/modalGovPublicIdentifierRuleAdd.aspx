<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.ModalGovPublicIdentifierRuleAdd" Trace="false"%>
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
      <td width="150">规则名称：</td>
      <td>
        <asp:TextBox id="tbRuleName" Columns="25" MaxLength="50" runat="server" />
        <asp:RequiredFieldValidator ControlToValidate="tbRuleName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
        <asp:RegularExpressionValidator runat="server" ControlToValidate="tbRuleName" ValidationExpression="[^',]+" errorMessage=" *" foreColor="red" display="Dynamic" />
      </td>
    </tr>
    <tr>
      <td>规则类型：</td>
      <td>
        <asp:DropDownList ID="ddlIdentifierType" OnSelectedIndexChanged="ddlIdentifierType_SelectedIndexChanged" AutoPostBack="true" runat="server"></asp:DropDownList>
      </td>
    </tr>
    <asp:PlaceHolder ID="phAttributeName" runat="server">
    <tr>
      <td>字段：</td>
      <td>
        <asp:DropDownList ID="ddlAttributeName" runat="server"></asp:DropDownList>
      </td>
    </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phMinLength" runat="server">
    <tr>
      <td>最小位数：</td>
      <td>
        <asp:TextBox id="tbMinLength" Text="0" Columns="25" MaxLength="50" Width="50" runat="server" />          
        <asp:RequiredFieldValidator ControlToValidate="tbMinLength" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />          
        <asp:RegularExpressionValidator
          ControlToValidate="tbMinLength"
          ValidationExpression="\d+"
          Display="Dynamic"
          ErrorMessage="必须为大于等于零的整数"
          foreColor="red"
          runat="server"/>          
        <asp:CompareValidator 
          ControlToValidate="tbMinLength" 
          Operator="GreaterThanEqual"
          ValueToCompare="0" 
          Display="Dynamic"
          ErrorMessage="必须为大于等于零的整数"
          foreColor="red"
          runat="server"/>
        <div class="gray">0表示不设置最小位数，设置后不足位数将通过补零方式填充</div>
      </td>
    </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phFormatString" runat="server">
    <tr>
      <td>日期格式：</td>
      <td><asp:TextBox id="tbFormatString" Text="yyyy" Columns="25" MaxLength="50" Width="100" runat="server" />          
        <asp:RequiredFieldValidator ControlToValidate="tbFormatString" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" /></td>
    </tr>
    </asp:PlaceHolder>
    <asp:PlaceHolder ID="phSequence" runat="server">
    <tr>
      <td>顺序号起始数字：</td>
      <td><asp:TextBox id="tbSequence" Text="0" Columns="25" MaxLength="50" Width="50" runat="server" />          
          <asp:RequiredFieldValidator ControlToValidate="tbSequence" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />          
          <asp:RegularExpressionValidator
            ControlToValidate="tbSequence"
            ValidationExpression="\d+"
            Display="Dynamic"
            ErrorMessage="必须为大于等于零的整数"
            foreColor="red"
            runat="server"/>          
        <asp:CompareValidator 
            ControlToValidate="tbSequence" 
            Operator="GreaterThanEqual"
            ValueToCompare="0" 
            Display="Dynamic"
            ErrorMessage="必须为大于等于零的整数"
            foreColor="red"
            runat="server"/>
        <div class="gray">每发布一篇信息，顺序号将在原有基础上自动增1</div></td>
    </tr>
    <tr>
      <td>不同栏目顺序号归零：</td>
      <td>
        <asp:RadioButtonList id="rblIsSequenceChannelZero" repeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
        <div class="gray">在不同的栏目中添加内容，顺序号需要归零后重新开始</div>
      </td>
    </tr>
    <tr>
      <td>不同部门顺序号归零：</td>
      <td>
        <asp:RadioButtonList id="rblIsSequenceDepartmentZero" repeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
        <div class="gray">当添加内容的所属部门不相同时，顺序号需要按照部门归零后重新开始</div>
      </td>
    </tr>
    <tr>
      <td>不同年份顺序号归零：</td>
      <td>
        <asp:RadioButtonList id="rblIsSequenceYearZero" repeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
        <div class="gray">当添加内容的添加时间为不同年份时，顺序号需要按照年份归零后重新开始</div>
      </td>
    </tr>
    </asp:PlaceHolder>
    <tr>
      <td>分隔符：</td>
      <td><asp:TextBox id="tbSuffix" Text="-" Columns="25" MaxLength="50" Width="50" runat="server" /></td>
    </tr>
  </table>

</form>
</body>
</html>
