<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationUploadImage" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <!--#include file="../inc/openWindow.html"-->

      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>图片上传设置</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此设置站点的图片上传功能选项
            </p>

            <ul class="nav nav-pills m-b-30">
              <li class="active">
                <a href="javascript:;">图片上传设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationUploadVideo.aspx?publishmentSystemId=<%=PublishmentSystemId%>">视频上传设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationUploadFile.aspx?publishmentSystemId=<%=PublishmentSystemId%>">附件上传设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationWatermark.aspx?publishmentSystemId=<%=PublishmentSystemId%>">图片水印设置</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">图片上传文件夹</label>
                <div class="col-sm-3">
                  <asp:TextBox class="form-control" MaxLength="50" id="TbImageUploadDirectoryName" runat="server" />
                </div>
                <div class="col-sm-6">
                  <asp:RequiredFieldValidator ControlToValidate="TbImageUploadDirectoryName" errorMessage=" *" foreColor="red" display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbImageUploadDirectoryName" ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" display="Dynamic" />
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">图片上传保存方式</label>
                <div class="col-sm-3">
                  <asp:DropDownList class="form-control" ID="DdlImageUploadDateFormatString" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <span class="help-block">
                    本设置只影响新上传的图片, 设置更改之前的图片仍存放在原来位置
                  </span>
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">是否按时间重命名上传的图片</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="DdlIsImageUploadChangeFileName" class="form-control" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <span class="help-block">
                    本设置只影响新上传的图片, 设置更改之前的图片名仍保持不变
                  </span>
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">上传图片类型</label>
                <div class="col-sm-3">
                  <asp:TextBox TextMode="MultiLine" class="form-control" Height="100" id="TbImageUploadTypeCollection" runat="server" />
                </div>
                <div class="col-sm-6">
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="TbImageUploadTypeCollection" ValidationExpression="[^']+"
                    errorMessage=" *" foreColor="red" display="Dynamic" />
                  <span class="help-block">
                    类型之间用“,”分割
                  </span>
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">上传图片最大大小</label>
                <div class="col-sm-2">
                  <asp:TextBox class="form-control" Columns="10" MaxLength="50" id="TbImageUploadTypeMaxSize" runat="server" />
                </div>
                <div class="col-sm-1">
                  <asp:DropDownList id="DdlImageUploadTypeUnit" class="form-control" runat="server">
                    <asp:ListItem Value="KB" Text="KB" Selected="true"></asp:ListItem>
                    <asp:ListItem Value="MB" Text="MB"></asp:ListItem>
                  </asp:DropDownList>
                </div>
                <div class="col-sm-6">
                  <asp:RequiredFieldValidator ControlToValidate="TbImageUploadTypeMaxSize" errorMessage=" *" foreColor="red" display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator ControlToValidate="TbImageUploadTypeMaxSize" ValidationExpression="\d+" Display="Dynamic"
                    ErrorMessage="上传图片最大大小必须为整数" foreColor="red" runat="server" />
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">缩略图（小）最大宽度</label>
                <div class="col-sm-2">
                  <asp:TextBox class="form-control" Columns="10" MaxLength="50" id="TbPhotoSmallWidth" runat="server" />
                </div>
                <div class="col-sm-1">
                  <span class="help-block">像素</span>
                </div>
                <div class="col-sm-6">
                  <asp:RequiredFieldValidator ControlToValidate="TbPhotoSmallWidth" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator ControlToValidate="TbPhotoSmallWidth" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="缩略图（小）最大宽度必须为整数"
                    foreColor="red" runat="server" />
                </div>
              </div>
              <div class="form-group">
                <label class="col-sm-3 control-label">缩略图（中）最大宽度</label>
                <div class="col-sm-2">
                  <asp:TextBox class="form-control" Columns="10" MaxLength="50" id="TbPhotoMiddleWidth" runat="server" />
                </div>
                <div class="col-sm-1">
                  <span class="help-block">像素</span>
                </div>
                <div class="col-sm-6">
                  <asp:RequiredFieldValidator ControlToValidate="TbPhotoMiddleWidth" errorMessage=" *" foreColor="red" display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator ControlToValidate="TbPhotoMiddleWidth" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="缩略图（中）最大宽度必须为整数"
                    foreColor="red" runat="server" />
                </div>
              </div>

              <hr />

              <div class="form-group m-b-0">
                <div class="col-sm-offset-3 col-sm-9">
                  <asp:Button class="btn btn-primary" id="Submit" text="确 定" onclick="Submit_OnClick" runat="server" />
                </div>
              </div>

            </div>

          </div>
        </div>

      </form>
    </body>

    </html>