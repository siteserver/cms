<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Wcm.PageGovPublicApplyConfiguration" %>
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
    <h3 class="popover-title">依申请公开设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="140">办理时限：</td>
          <td colspan="3">
            <asp:TextBox id="tbGovPublicApplyDateLimit" class="input-mini" Columns="4" MaxLength="4" Style="text-align:right" Text="0" runat="server"/>
            日
            <asp:RequiredFieldValidator
              ControlToValidate="tbGovPublicApplyDateLimit"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator
              ControlToValidate="tbGovPublicApplyDateLimit"
              ValidationExpression="\d+"
              ErrorMessage="办理时限只能是数字"
              foreColor="red"
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <tr>
          <td>预警：</td>
          <td width="70">办理时限</td>
          <td width="150">
            <asp:RadioButtonList ID="rblGovPublicApplyAlertDateIsAfter" RepeatDirection="Horizontal" class="radiobuttonlist" runat="server">
              <asp:ListItem Text="前" Value="False" Selected="true"></asp:ListItem>
              <asp:ListItem Text="后" Value="True"></asp:ListItem>
            </asp:RadioButtonList>
          </td>
          <td>
            <asp:TextBox ID="tbGovPublicApplyAlertDate" class="input-mini" Columns="4" MaxLength="4" style="text-align:right" Text="0" runat="server"/>        
            日
            <asp:RequiredFieldValidator
              ControlToValidate="tbGovPublicApplyAlertDate"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>  
            <asp:RegularExpressionValidator
              ControlToValidate="tbGovPublicApplyAlertDate"
              ValidationExpression="\d+"
              ErrorMessage="预警只能是数字"
              foreColor="red"
              Display="Dynamic"
              runat="server"/>
            </td>
        </tr>
        <tr>
          <td>黄牌：</td>
          <td colspan="3">
            预警后
            <asp:TextBox id="tbGovPublicApplyYellowAlertDate" class="input-mini" Columns="4" MaxLength="4" Style="text-align:right" Text="0" runat="server"/>
            日
            <asp:RequiredFieldValidator
              ControlToValidate="tbGovPublicApplyYellowAlertDate"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator
              ControlToValidate="tbGovPublicApplyYellowAlertDate"
              ValidationExpression="\d+"
              ErrorMessage="黄牌只能是数字"
              foreColor="red"
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <tr>
          <td>红牌：</td>
          <td colspan="3">
            黄牌后
            <asp:TextBox id="tbGovPublicApplyRedAlertDate" class="input-mini" Columns="4" MaxLength="4" Style="text-align:right" Text="0" runat="server"/>
            日
            <asp:RequiredFieldValidator
              ControlToValidate="tbGovPublicApplyRedAlertDate"
              errorMessage=" *" foreColor="red" 
              Display="Dynamic"
              runat="server"/>
            <asp:RegularExpressionValidator
              ControlToValidate="tbGovPublicApplyRedAlertDate"
              ValidationExpression="\d+"
              ErrorMessage="红牌只能是数字"
              foreColor="red"
              Display="Dynamic"
              runat="server"/>
          </td>
        </tr>
        <tr>
          <td>申请是否可删除：</td>
          <td colspan="3">
            <asp:RadioButtonList ID="rblGovPublicApplyIsDeleteAllowed" RepeatDirection="Horizontal" class="noborder" runat="server">
              <asp:ListItem Text="是" Value="True" Selected="true"></asp:ListItem>
              <asp:ListItem Text="否" Value="False"></asp:ListItem>
            </asp:RadioButtonList>
          </td>
        </tr>
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
