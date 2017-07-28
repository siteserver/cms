<%@ Page Language="C#" validateRequest="false" Inherits="SiteServer.BackgroundPages.Cms.PageTagStyleMailSMS" %>
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
    <h3 class="popover-title">邮件/短信发送设置</h3>
    <div class="popover-content">

      <table class="table noborder table-hover">
        <tr>
          <td width="155"> 是否发送短信： </td>
          <td><asp:RadioButtonList ID="rblIsSMS" AutoPostBack="true" OnSelectedIndexChanged="rblIsSMS_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server">
              <asp:ListItem Text="发送短信" Value="True"></asp:ListItem>
              <asp:ListItem Text="不发送短信" Value="False" Selected="true"></asp:ListItem>
            </asp:RadioButtonList></td>
          <td class="gray">设置提交内容后是否需要发送短信提醒</td>
        </tr>
        <asp:PlaceHolder ID="phSMS" Visible="false" runat="server">
          <tr>
            <td width="155"> 短信接收人： </td>
            <td><asp:RadioButtonList ID="rblSMSReceiver" AutoPostBack="true" OnSelectedIndexChanged="rblSMSReceiver_SelectedIndexChanged" runat="server">
                <asp:ListItem Text="指定手机" Value="True" Selected="true"></asp:ListItem>
                <asp:ListItem Text="表单提交者" Value="False"></asp:ListItem>
                <asp:ListItem Text="表单提交者及指定手机" Value="All"></asp:ListItem>
              </asp:RadioButtonList></td>
            <td class="gray">设置短信提醒的收信人</td>
          </tr>
          <asp:PlaceHolder ID="phSMSTo" runat="server">
            <tr>
              <td> 指定手机号码： </td>
              <td><asp:TextBox Columns="35" MaxLength="50" id="tbSMSTo" runat="server" /></td>
              <td class="gray">多个手机号码用";"分隔</td>
            </tr>
          </asp:PlaceHolder>
          <asp:PlaceHolder ID="phSMSFiledName" runat="server">
            <tr>
              <td> 提交表单手机字段： </td>
              <td><asp:DropDownList ID="ddlSMSFiledName" runat="server"></asp:DropDownList></td>
              <td class="gray">设置提交表单的手机字段，系统将向此字段的手机号码发送短信</td>
            </tr>
          </asp:PlaceHolder>
          <tr>
            <td width="155">自定义短信发送内容：</td>
            <td><asp:RadioButtonList ID="rblIsSMSTemplate" AutoPostBack="true" OnSelectedIndexChanged="rblIsSMSTemplate_SelectedIndexChanged" RepeatDirection="Horizontal" class="noborder" runat="server">
                <asp:ListItem Text="自定义内容" Value="True"></asp:ListItem>
                <asp:ListItem Text="使用系统默认内容" Value="False" Selected="true"></asp:ListItem>
              </asp:RadioButtonList></td>
            <td class="gray">设置是否自定义短信发送内容</td>
          </tr>
          <asp:PlaceHolder ID="phSMSTemplate" Visible="false" runat="server">
            <tr>
              <td width="155">短信发送内容：</td>
              <td colspan="2"><asp:TextBox Width="90%" TextMode="MultiLine" ID="tbSMSContent" runat="server" Rows="10" Wrap="false" Text="" />
                <br>
                <span class="gray">（
                <asp:Literal ID="ltlTips2" runat="server"></asp:Literal>
                ）</span></td>
            </tr>
          </asp:PlaceHolder>
        </asp:PlaceHolder>
      </table>

      <hr />
      <table class="table noborder">
        <tr>
          <td class="center">
            <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
            <input type=button class="btn" onClick="location.href='pageInput.aspx?PublishmentSystemID=<%=PublishmentSystemId%>';" value="返 回" />
          </td>
        </tr>
      </table>

    </div>
  </div>

</form>
</body>
</html>
