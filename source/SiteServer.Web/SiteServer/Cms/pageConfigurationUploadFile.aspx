<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationUploadFile" %>
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
    <h3 class="popover-title">附件上传设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="220">附件上传文件夹：</td>
          <td>
            <asp:TextBox Columns="25" MaxLength="50" id="tbFileUploadDirectoryName" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="tbFileUploadDirectoryName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="tbFileUploadDirectoryName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        <tr>
          <td>附件上传保存方式：</td>
          <td>
            <asp:RadioButtonList ID="rblFileUploadDateFormatString" class="noborder" runat="server"></asp:RadioButtonList>
            <span>本设置只影响新上传的附件, 设置更改之前的附件仍存放在原来位置</span>
          </td>
        </tr>
        <tr>
          <td>是否按时间重命名上传的附件：</td>
          <td>
            <asp:RadioButtonList ID="rblIsFileUploadChangeFileName" class="noborder" runat="server"></asp:RadioButtonList>
            <span>本设置只影响新上传的附件, 设置更改之前的附件名仍保持不变</span>
          </td>
        </tr>
        <tr>
          <td>允许上传的文件类型：</td>
          <td>
            <asp:TextBox TextMode="MultiLine" Width="260px" Height="100" id="tbFileUploadTypeCollection" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="tbFileUploadTypeCollection"
            ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
            <br>
            <span>类型之间用“,”分割</span>
          </td>
        </tr>
        <tr>
          <td>上传附件最大大小：</td>
          <td>
            <asp:TextBox class="input-mini" Columns="10" MaxLength="50" id="tbFileUploadTypeMaxSize" runat="server" />
            <asp:DropDownList id="ddlFileUploadTypeUnit" class="input-small" runat="server">
              <asp:ListItem Value="KB" Text="KB" Selected="true"></asp:ListItem>
              <asp:ListItem Value="MB" Text="MB"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ControlToValidate="tbFileUploadTypeMaxSize" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator
              ControlToValidate="tbFileUploadTypeMaxSize"
              ValidationExpression="\d+"
              Display="Dynamic"
              ErrorMessage="上传附件最大大小必须为整数"
              foreColor="red"
              runat="server"/>
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
