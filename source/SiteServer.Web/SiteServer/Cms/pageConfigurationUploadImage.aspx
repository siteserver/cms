<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationUploadImage" %>
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
  <asp:Literal id="LtlBreadCrumb" runat="server" />
  <bairong:alerts runat="server" />

  <div class="popover popover-static">
    <h3 class="popover-title">图片上传设置</h3>
    <div class="popover-content">
    
      <table class="table noborder table-hover">
        <tr>
          <td width="220">图片上传文件夹：</td>
          <td>
            <asp:TextBox Columns="25" MaxLength="50" id="TbImageUploadDirectoryName" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TbImageUploadDirectoryName" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbImageUploadDirectoryName" ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
          </td>
        </tr>
        <tr>
          <td>图片上传保存方式：</td>
          <td>
            <asp:RadioButtonList ID="RblImageUploadDateFormatString" class="noborder" runat="server"></asp:RadioButtonList>
            <span>本设置只影响新上传的图片, 设置更改之前的图片仍存放在原来位置</span>
          </td>
        </tr>
        <tr>
          <td>是否按时间重命名上传的图片：</td>
          <td>
            <asp:RadioButtonList ID="RblIsImageUploadChangeFileName" class="noborder" runat="server"></asp:RadioButtonList>
            <span>本设置只影响新上传的图片, 设置更改之前的图片名仍保持不变</span>
          </td>
        </tr>
        <tr>
          <td>上传图片类型：</td>
          <td>
            <asp:TextBox TextMode="MultiLine" Width="260px" Height="100" id="TbImageUploadTypeCollection" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbImageUploadTypeCollection"
            ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
            <br>
            <span>类型之间用“,”分割</span>
          </td>
        </tr>
        <tr>
          <td>上传图片最大大小：</td>
          <td>
            <asp:TextBox class="input-mini" Columns="10" MaxLength="50" id="TbImageUploadTypeMaxSize" runat="server" />
            <asp:DropDownList id="DdlImageUploadTypeUnit" class="input-small" runat="server">
              <asp:ListItem Value="KB" Text="KB" Selected="true"></asp:ListItem>
              <asp:ListItem Value="MB" Text="MB"></asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ControlToValidate="TbImageUploadTypeMaxSize" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator
              ControlToValidate="TbImageUploadTypeMaxSize"
              ValidationExpression="\d+"
              Display="Dynamic"
              ErrorMessage="上传图片最大大小必须为整数"
              foreColor="red"
              runat="server"/>
            </td>
        </tr>
        <tr>
          <td>图片模型上传图片设置：</td>
          <td>
            缩略图（小）最大宽度：<asp:TextBox class="input-mini" Columns="10" MaxLength="50" id="TbPhotoSmallWidth" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TbPhotoSmallWidth" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator
              ControlToValidate="TbPhotoSmallWidth"
              ValidationExpression="\d+"
              Display="Dynamic"
              ErrorMessage="缩略图（小）最大宽度必须为整数"
              foreColor="red"
              runat="server"/> px
            <div style="height: 5px"></div>
            缩略图（中）最大宽度：<asp:TextBox class="input-mini" Columns="10" MaxLength="50" id="TbPhotoMiddleWidth" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TbPhotoMiddleWidth" errorMessage=" *" foreColor="red" display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator
              ControlToValidate="TbPhotoMiddleWidth"
              ValidationExpression="\d+"
              Display="Dynamic"
              ErrorMessage="缩略图（中）最大宽度必须为整数"
              foreColor="red"
              runat="server"/> px
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
