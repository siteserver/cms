<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationUploadImage" %>
<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>

<head>
  <meta charset="utf-8">
  <!--#include file="../inc/head.html"-->
</head>

<body>
  <form class="m-l-15 m-r-15" runat="server">

    <div class="card-box" style="padding: 10px; margin-bottom: 10px;">
      <ul class="nav nav-pills nav-justified">
        <li class="nav-item active">
          <a class="nav-link" href="javascript:;">图片上传设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationUploadVideo.aspx?siteId=<%=SiteId%>">视频上传设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationUploadFile.aspx?siteId=<%=SiteId%>">附件上传设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationWatermark.aspx?siteId=<%=SiteId%>">图片水印设置</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">图片上传文件夹
          <asp:RequiredFieldValidator ControlToValidate="TbImageUploadDirectoryName" errorMessage=" *" foreColor="red"
            display="Dynamic" runat="server" />
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbImageUploadDirectoryName"
            ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
        </label>
        <asp:TextBox class="form-control" MaxLength="50" id="TbImageUploadDirectoryName" runat="server" />
      </div>

      <div class="form-group">
        <label class="col-form-label">图片上传保存方式</label>
        <asp:DropDownList class="form-control" ID="DdlImageUploadDateFormatString" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          本设置只影响新上传的图片, 设置更改之前的图片仍存放在原来位置
        </small>
      </div>

      <div class="form-group">
        <label class="col-form-label">是否按时间重命名上传的图片</label>
        <asp:DropDownList ID="DdlIsImageUploadChangeFileName" class="form-control" runat="server"></asp:DropDownList>
        <small class="form-text text-muted">
          本设置只影响新上传的图片, 设置更改之前的图片名仍保持不变
        </small>
      </div>

      <div class="form-group">
        <label class="col-form-label">上传图片类型
          <asp:RegularExpressionValidator runat="server" ControlToValidate="TbImageUploadTypeCollection"
            ValidationExpression="[^']+" errorMessage=" *" foreColor="red" display="Dynamic" />
        </label>
        <asp:TextBox TextMode="MultiLine" class="form-control" Height="100" id="TbImageUploadTypeCollection" runat="server" />
        <small class="form-text text-muted">
          类型之间用“,”分割
        </small>
      </div>

      <div class="form-group">
        <label class="col-form-label">上传图片最大大小
          <asp:RequiredFieldValidator ControlToValidate="TbImageUploadTypeMaxSize" errorMessage=" *" foreColor="red"
            display="Dynamic" runat="server" />
          <asp:RegularExpressionValidator ControlToValidate="TbImageUploadTypeMaxSize" ValidationExpression="\d+"
            Display="Dynamic" ErrorMessage="上传图片最大大小必须为整数" foreColor="red" runat="server" />
        </label>
        <div class="row">
          <div class="col">
            <asp:TextBox class="form-control" Columns="10" MaxLength="50" id="TbImageUploadTypeMaxSize" runat="server" />
          </div>
          <div class="col">
            <asp:DropDownList id="DdlImageUploadTypeUnit" class="form-control" runat="server">
              <asp:ListItem Value="KB" Text="KB" Selected="true"></asp:ListItem>
              <asp:ListItem Value="MB" Text="MB"></asp:ListItem>
            </asp:DropDownList>
          </div>
        </div>
      </div>

      <div class="form-group">
        <label class="col-form-label">缩略图（小）最大宽度
          <asp:RequiredFieldValidator ControlToValidate="TbPhotoSmallWidth" errorMessage=" *" foreColor="red" display="Dynamic"
            runat="server" />
          <asp:RegularExpressionValidator ControlToValidate="TbPhotoSmallWidth" ValidationExpression="\d+" Display="Dynamic"
            ErrorMessage="缩略图（小）最大宽度必须为整数" foreColor="red" runat="server" />
        </label>
        <asp:TextBox class="form-control" Columns="10" MaxLength="50" id="TbPhotoSmallWidth" runat="server" />
        <small class="form-text text-muted">像素</small>
      </div>

      <div class="form-group">
        <label class="col-form-label">缩略图（中）最大宽度
          <asp:RequiredFieldValidator ControlToValidate="TbPhotoMiddleWidth" errorMessage=" *" foreColor="red" display="Dynamic"
            runat="server" />
          <asp:RegularExpressionValidator ControlToValidate="TbPhotoMiddleWidth" ValidationExpression="\d+" Display="Dynamic"
            ErrorMessage="缩略图（中）最大宽度必须为整数" foreColor="red" runat="server" />
        </label>
        <asp:TextBox class="form-control" Columns="10" MaxLength="50" id="TbPhotoMiddleWidth" runat="server" />
        <small class="form-text text-muted">像素</small>
      </div>

      <hr />

      <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->