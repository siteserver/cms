<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationInnerLink" validateRequest="false" %>
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
  <asp:Literal id="ltlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">站内链接设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="220">是否启用站内链接功能：</td>
          <td><asp:RadioButtonList ID="IsInnerLink" runat="server" RepeatDirection="Horizontal" class="noborder" AutoPostBack="true" OnSelectedIndexChanged="IsInnerLink_SelectedIndexChanged"></asp:RadioButtonList></td>
        </tr>
        <asp:PlaceHolder ID="phInnerLink" runat="server">
          <tr>
            <td>是否设置栏目名称为关键字：</td>
            <td><asp:DropDownList ID="IsInnerLinkByChannelName" runat="server" RepeatDirection="Horizontal"></asp:DropDownList></td>
          </tr>
          <tr>
            <td>站内链接显示代码：</td>
            <td><asp:TextBox Columns="56" MaxLength="200" id="InnerLinkFormatString" runat="server" />
              <asp:RequiredFieldValidator ControlToValidate="InnerLinkFormatString" ErrorMessage="此项不能为空" foreColor="red" Display="Dynamic" runat="server" />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="InnerLinkFormatString"
                                      ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
              <br>
              {0}代表链接地址，{1}代表站内链接关键字 </td>
          </tr>
          <tr>
            <td>站内链接数量限制（单个关键字）：</td>
            <td>
              <asp:TextBox Columns="10" MaxLength="50" id="InnerLinkMaxNum" runat="server" />
              <asp:RequiredFieldValidator ControlToValidate="InnerLinkMaxNum" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
              <asp:RegularExpressionValidator
                ControlToValidate="InnerLinkMaxNum"
                ValidationExpression="\d+"
                Display="Dynamic"
                errorMessage=" *" foreColor="red" 
                runat="server"/>
              <asp:CompareValidator 
                ControlToValidate="InnerLinkMaxNum" 
                Operator="GreaterThan" 
                ValueToCompare="0" 
                Display="Dynamic"
                errorMessage=" *" foreColor="red" 
                runat="server"/>
              </td>
          </tr>
        </asp:PlaceHolder>
      </table>
  
      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
          </td>
        </tr>
      </table>
  
    </div>
  </div>

</form>
</body>
</html>
