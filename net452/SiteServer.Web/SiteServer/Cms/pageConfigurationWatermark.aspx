<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationWaterMark" %>
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
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationUploadImage.aspx?siteId=<%=SiteId%>">图片上传设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationUploadVideo.aspx?siteId=<%=SiteId%>">视频上传设置</a>
        </li>
        <li class="nav-item">
          <a class="nav-link" href="pageConfigurationUploadFile.aspx?siteId=<%=SiteId%>">附件上传设置</a>
        </li>
        <li class="nav-item active">
          <a class="nav-link" href="javascript:;">图片水印设置</a>
        </li>
      </ul>
    </div>

    <ctrl:alerts runat="server" />

    <div class="card-box">
      <div class="form-group">
        <label class="col-form-label">是否启用水印功能</label>
        <asp:DropDownList ID="DdlIsWaterMark" runat="server" RepeatDirection="Horizontal" class="form-control"
          AutoPostBack="true" OnSelectedIndexChanged="DdlIsWaterMark_SelectedIndexChanged"></asp:DropDownList>
      </div>

      <asp:PlaceHolder id="PhWaterMarkPosition" runat="server">
        <div class="form-group">
          <label class="col-form-label">添加水印位置</label>
          <div class="row m-l-10">
            <asp:Literal ID="LtlWaterMarkPosition" runat="server"></asp:Literal>
          </div>
          <small class="form-text text-muted">
            请在此选择水印添加的位置(共 9 个位置可选)
          </small>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhWaterMarkTransparency" runat="server">
        <div class="form-group">
          <label class="col-form-label">水印不透明度</label>
          <asp:DropDownList class="form-control" ID="DdlWaterMarkTransparency" runat="server"></asp:DropDownList>
          <small class="form-text text-muted">
            取值范围10%--100% (100%为不透明)
          </small>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhWaterMarkMin" runat="server">
        <div class="form-group">
          <label class="col-form-label">图片最小尺寸
            <asp:RequiredFieldValidator ControlToValidate="TbWaterMarkMinWidth" ErrorMessage=" *" ForeColor="red"
              Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="TbWaterMarkMinWidth" ValidationExpression="\d+" Display="Dynamic"
              ErrorMessage="此项必须为数字" foreColor="red" runat="server" />
            <asp:RequiredFieldValidator ControlToValidate="TbWaterMarkMinHeight" ErrorMessage=" *" ForeColor="red"
              Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="TbWaterMarkMinHeight" ValidationExpression="\d+" Display="Dynamic"
              ErrorMessage="此项必须为数字" foreColor="red" runat="server" />
          </label>
          <div class="form-inline row">
            <label class="mr-sm-5 ml-sm-5">宽</label>
            <asp:TextBox ID="TbWaterMarkMinWidth" class="form-control" MaxLength="50" runat="server"></asp:TextBox>
            <label class="mr-sm-5 ml-sm-5">高</label>
            <asp:TextBox ID="TbWaterMarkMinHeight" class="form-control" MaxLength="50" runat="server"></asp:TextBox>
          </div>
          <small class="form-text text-muted">
            需要添加水印的图片的最小尺寸，单位为像素，（0代表不限制）
          </small>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhIsImageWaterMark" runat="server">
        <div class="form-group">
          <label class="col-form-label">水印类型</label>
          <asp:DropDownList ID="DdlIsImageWaterMark" runat="server" RepeatDirection="Horizontal" class="form-control"
            AutoPostBack="true" OnSelectedIndexChanged="DdlIsWaterMark_SelectedIndexChanged"></asp:DropDownList>
          <small class="form-text text-muted">
            选择使用的水印类型
          </small>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhWaterMarkFormatString" runat="server">
        <div class="form-group">
          <label class="col-form-label">文字型水印的内容
            <asp:RequiredFieldValidator ControlToValidate="TbWaterMarkFormatString" ErrorMessage=" *" ForeColor="red"
              Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="TbWaterMarkFormatString"
              ValidationExpression="[^']+" ErrorMessage="此项不能为空" foreColor="red" Display="Dynamic" />
          </label>
          <asp:TextBox class="form-control" Columns="25" MaxLength="50" ID="TbWaterMarkFormatString" runat="server" />
          <small class="form-text text-muted">
            可以使用替换变量: {0}表示当前日期 {1}表示当前时间 例如:&quot;上传于{0} {1}&quot;
          </small>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhWaterMarkFontName" runat="server">
        <div class="form-group">
          <label class="col-form-label">文字水印的字体</label>
          <asp:DropDownList class="form-control" ID="DdlWaterMarkFontName" runat="server"></asp:DropDownList>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhWaterMarkFontSize" runat="server">
        <div class="form-group">
          <label class="col-form-label">文字水印的大小
            <asp:RequiredFieldValidator ControlToValidate="TbWaterMarkFontSize" ErrorMessage="此项不能为空" foreColor="red"
              Display="Dynamic" runat="server" />
            <asp:RegularExpressionValidator ControlToValidate="TbWaterMarkFontSize" ValidationExpression="\d+" Display="Dynamic"
              ErrorMessage="此项必须为数字" foreColor="red" runat="server" />
          </label>
          <asp:TextBox class="form-control" Columns="25" MaxLength="50" ID="TbWaterMarkFontSize" runat="server" />
          <small class="form-text text-muted">
            像素
          </small>
        </div>
      </asp:PlaceHolder>

      <asp:PlaceHolder id="PhWaterMarkImagePath" runat="server">
        <div class="form-group">
          <label class="col-form-label">图片型水印文件
            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="TbWaterMarkImagePath"
              ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
          </label>

          <div class="row pl-2">
            <asp:TextBox class="form-control col-sm-10" ID="TbWaterMarkImagePath" runat="server" />
            <div class="col-sm-2">
              &nbsp;
              <asp:Button ID="BtnImageUrlSelect" runat="server" class="btn btn-success" Text="选择"></asp:Button>
              &nbsp;
              <asp:Button ID="BtnImageUrlUpload" runat="server" class="btn btn-success" Text="上传"></asp:Button>
            </div>
          </div>
        </div>
      </asp:PlaceHolder>

      <hr />

      <asp:Button class="btn btn-primary" text="确 定" onclick="Submit_OnClick" runat="server" />

    </div>

  </form>
</body>

</html>
<!--#include file="../inc/foot.html"-->