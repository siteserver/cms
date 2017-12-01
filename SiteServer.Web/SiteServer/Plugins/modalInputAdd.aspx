<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Plugins.ModalInputAdd" Trace="false"%>
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
        <td width="180">提交表单名称：</td>
        <td>
          <asp:TextBox Columns="25" MaxLength="50" id="TbInputName" runat="server" />
          <asp:RequiredFieldValidator ControlToValidate="TbInputName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
          />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbInputName" ValidationExpression="[^',]+" errorMessage=" *"
            foreColor="red" display="Dynamic" />
        </td>
      </tr>
      <tr>
        <td>是否需要审核：</td>
        <td>
          <asp:RadioButtonList ID="RblIsChecked" RepeatDirection="Horizontal" class="noborder" runat="server">
            <asp:ListItem Text="需要审核" Value="False"></asp:ListItem>
            <asp:ListItem Text="不需要审核" Value="True" Selected="true"></asp:ListItem>
          </asp:RadioButtonList>
        </td>
      </tr>
      <tr>
        <td>是否需要回复：</td>
        <td>
          <asp:RadioButtonList ID="RblIsReply" RepeatDirection="Horizontal" class="noborder" runat="server">
            <asp:ListItem Text="需要回复" Value="True"></asp:ListItem>
            <asp:ListItem Text="不需要回复" Value="False" Selected="true"></asp:ListItem>
          </asp:RadioButtonList>
        </td>
      </tr>
      <tr>
        <td>允许匿名提交：</td>
        <td>
          <asp:RadioButtonList ID="RblIsAnomynous" RepeatDirection="Horizontal" class="noborder" runat="server">
            <asp:ListItem Text="允许匿名提交" Value="True" Selected="true"></asp:ListItem>
            <asp:ListItem Text="不允许" Value="False"></asp:ListItem>
          </asp:RadioButtonList>
        </td>
      </tr>
      <asp:PlaceHolder id="PhAdministratorSmsNotify" runat="server">
        <tr>
          <td>向管理员发送短信通知：</td>
          <td>
            <asp:RadioButtonList ID="RblIsAdministratorSmsNotify" AutoPostBack="true" OnSelectedIndexChanged="RblIsAdministratorSmsNotify_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server">
              <asp:ListItem Text="发送短信通知" Value="True" Selected="true"></asp:ListItem>
              <asp:ListItem Text="不发送" Value="False"></asp:ListItem>
            </asp:RadioButtonList>
            <span class="gray">启用短信发送后将自动为每一条新增提交发送短信给指定手机</span>
          </td>
        </tr>
        <asp:PlaceHolder id="PhIsAdministratorSmsNotify" runat="server">
          <tr>
              <td>发送通知短信模板Id：</td>
              <td>
                  <asp:TextBox ID="TbAdministratorSmsNotifyTplId" runat="server"></asp:TextBox>
                  <asp:RequiredFieldValidator ControlToValidate="TbAdministratorSmsNotifyTplId" runat="server" ErrorMessage="*" foreColor="Red"></asp:RequiredFieldValidator>
                  <br />
                  <span class="gray">需进入短信供应商模板管理界面，添加通知类短信模板并获取模板Id</span>
              </td>
          </tr>
          <tr>
              <td>短信模板包含变量：</td>
              <td>
                  <asp:CheckBoxList ID="CblAdministratorSmsNotifyKeys" class="checkboxlist" repeatDirection="Horizontal" repeatColumns="2" runat="server"></asp:CheckBoxList>
                  <span class="gray">请勾选短信模板文字中包含的变量，请确保变量名大小写一致</span>
              </td>
          </tr>
          <tr>
            <td>管理员接受短信通知手机号：</td>
            <td>
              <asp:TextBox ID="TbAdministratorSmsNotifyMobile" runat="server" />
              <asp:RequiredFieldValidator ControlToValidate="TbAdministratorSmsNotifyMobile" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
              />
              <asp:RegularExpressionValidator runat="server" ControlToValidate="TbAdministratorSmsNotifyMobile" ValidationExpression="[^']+" ErrorMessage=" *"
                ForeColor="red" Display="Dynamic" />
            </td>
          </tr>
        </asp:PlaceHolder>
      </asp:PlaceHolder>
    </table>

  </form>
</body>

</html>