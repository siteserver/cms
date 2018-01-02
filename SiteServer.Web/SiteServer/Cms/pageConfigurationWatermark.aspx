<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.PageConfigurationWaterMark" %>
  <%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html>

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form class="container" runat="server">
        <bairong:alerts runat="server" />

        <div class="raw">
          <div class="card-box">
            <h4 class="m-t-0 header-title">
              <b>图片水印设置</b>
            </h4>
            <p class="text-muted font-13 m-b-25">
              在此设置图片水印功能
            </p>

            <ul class="nav nav-pills m-b-30">
              <li>
                <a href="pageConfigurationUploadImage.aspx?publishmentSystemId=<%=PublishmentSystemId%>">图片上传设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationUploadVideo.aspx?publishmentSystemId=<%=PublishmentSystemId%>">视频上传设置</a>
              </li>
              <li class="">
                <a href="pageConfigurationUploadFile.aspx?publishmentSystemId=<%=PublishmentSystemId%>">附件上传设置</a>
              </li>
              <li class="active">
                <a href="javascript:;">图片水印设置</a>
              </li>
            </ul>

            <div class="form-horizontal">

              <div class="form-group">
                <label class="col-sm-3 control-label">是否启用水印功能</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="IsWaterMark" runat="server" RepeatDirection="Horizontal" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="IsWaterMark_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-sm-6">

                </div>
              </div>
              <div class="form-group" id="WaterMarkPositionRow" runat="server">
                <label class="col-sm-3 control-label">添加水印位置</label>
                <div class="col-sm-6">
                  <asp:Literal ID="WaterMarkPosition" runat="server"></asp:Literal>
                  <br />
                  请在此选择水印添加的位置(共 9 个位置可选)
                </div>
              </div>
              <div class="form-group" id="WaterMarkTransparencyRow" runat="server">
                <label class="col-sm-3 control-label">水印不透明度</label>
                <div class="col-sm-3">
                  <asp:DropDownList class="form-control" ID="WaterMarkTransparency" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                    取值范围10%--100% (100%为不透明)
                </div>
              </div>
              <div class="form-group" id="WaterMarkMinRow" runat="server">
                <label class="col-sm-3 control-label">图片最小尺寸</label>
                <div class="col-sm-9">
                  宽：
                  <asp:TextBox ID="WaterMarkMinWidth" class="form-control" style="width: 100px; display: inline-block;" Columns="10" MaxLength="50" runat="server"></asp:TextBox>
                  高：
                  <asp:TextBox ID="WaterMarkMinHeight" class="form-control" style="width: 100px; display: inline-block;" Columns="10" MaxLength="50" runat="server"></asp:TextBox>
                  <br />
                  <asp:RequiredFieldValidator ControlToValidate="WaterMarkMinWidth" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator ControlToValidate="WaterMarkMinWidth" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字"
                    foreColor="red" runat="server" />
                  <asp:RequiredFieldValidator ControlToValidate="WaterMarkMinHeight" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
                  />
                  <asp:RegularExpressionValidator ControlToValidate="WaterMarkMinHeight" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字"
                    foreColor="red" runat="server" /> 需要添加水印的图片的最小尺寸，（0代表不限制）
                </div>
              </div>
              <div class="form-group" id="IsImageWaterMarkRow" runat="server">
                <label class="col-sm-3 control-label">水印类型</label>
                <div class="col-sm-3">
                  <asp:DropDownList ID="IsImageWaterMark" runat="server" RepeatDirection="Horizontal" class="form-control" AutoPostBack="true"
                    OnSelectedIndexChanged="IsWaterMark_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-sm-6 help-block">
                  选择使用的水印类型
                </div>
              </div>
              <div class="form-group" id="WaterMarkFormatStringRow" runat="server">
                <label class="col-sm-3 control-label">文字型水印的内容</label>
                <div class="col-sm-3">
                  <asp:TextBox class="form-control" Columns="25" MaxLength="50" ID="WaterMarkFormatString" runat="server" />

                </div>
                <div class="col-sm-6 help-block">
                  <asp:RequiredFieldValidator ControlToValidate="WaterMarkFormatString" ErrorMessage=" *" ForeColor="red" Display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator runat="server" ControlToValidate="WaterMarkFormatString" ValidationExpression="[^']+" ErrorMessage="此项不能为空"
                    foreColor="red" Display="Dynamic" /> 可以使用替换变量: {0}表示当前日期 {1}表示当前时间 例如:&quot;上传于{0} {1}&quot;
                </div>
              </div>
              <div class="form-group" id="WaterMarkFontNameRow" runat="server">
                <label class="col-sm-3 control-label">文字水印的字体</label>
                <div class="col-sm-3">
                  <asp:DropDownList class="form-control" ID="WaterMarkFontName" runat="server"></asp:DropDownList>
                </div>
                <div class="col-sm-6">

                </div>
              </div>
              <div class="form-group" id="WaterMarkFontSizeRow" runat="server">
                <label class="col-sm-3 control-label">文字水印的大小</label>
                <div class="col-sm-2">
                  <asp:TextBox class="form-control" Columns="25" MaxLength="50" ID="WaterMarkFontSize" runat="server" />
                </div>
                <div class="col-sm-1 help-block">
                  像素
                </div>
                <div class="col-sm-6 help-block">
                  <asp:RequiredFieldValidator ControlToValidate="WaterMarkFontSize" ErrorMessage="此项不能为空" foreColor="red" Display="Dynamic"
                    runat="server" />
                  <asp:RegularExpressionValidator ControlToValidate="WaterMarkFontSize" ValidationExpression="\d+" Display="Dynamic" ErrorMessage="此项必须为数字"
                    foreColor="red" runat="server" />
                </div>
              </div>

              <div class="form-group" id="WaterMarkImagePathRow" runat="server">
                <label class="col-sm-3 control-label">图片型水印文件</label>
                <div class="col-sm-3">
                  <asp:TextBox class="form-control" Columns="55" MaxLength="100" ID="WaterMarkImagePath" runat="server" />
                </div>
                <div class="col-sm-6 help-block">
                  <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ControlToValidate="WaterMarkImagePath" ErrorMessage=" *" ForeColor="red"
                    Display="Dynamic" runat="server" /> &nbsp;
                  <asp:Button ID="ImageUrlSelect" runat="server" class="btn btn-success" Text="选择"></asp:Button>
                  &nbsp;
                  <asp:Button ID="ImageUrlUpload" runat="server" class="btn btn-success" Text="上传"></asp:Button>
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