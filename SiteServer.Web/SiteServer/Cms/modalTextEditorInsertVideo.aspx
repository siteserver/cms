<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTextEditorInsertVideo" %>
  <%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
    <!DOCTYPE html>
    <html class="modalPage">

    <head>
      <meta charset="utf-8">
      <!--#include file="../inc/head.html"-->
    </head>

    <body>
      <form runat="server">
        <ctrl:alerts text="如果不设置宽度和高度，页面将显示视频默认的尺寸" runat="server" />
        <input type="hidden" id="fileNames" name="fileNames" value="" />
        <ctrl:code type="ajaxUpload" runat="server" />

        <div class="raw">
          <ul class="nav nav-tabs tabs m-b-10">
            <li id="tab1" class="active tab">
              <a href="javascript:;" onclick="$('#column1').show();$('#column2').hide();$('.nav-tabs li').removeClass('active');$('#tab1').addClass('active');">上传视频</a>
            </li>
            <li id="tab2" class="tab">
              <a href="javascript:;" onclick="$('#column1').hide();$('#column2').show();$('.nav-tabs li').removeClass('active');$('#tab2').addClass('active');">输入地址</a>
            </li>
          </ul>
        </div>

        <div class="form-group form-row" id="column1">
          <label class="col-3 text-right col-form-label">请选择视频文件</label>
          <div class="col-8">
            <div id="fileSelect">
              <a id="upload_video" href="javascript:;" class="btn btn-success">上 传</a>
              <div id="video_upload_txt" style="clear:both; font-size:12px; color:#FF3737;"></div>
            </div>
          </div>
          <div class="col-1"></div>
        </div>

        <div class="form-group form-row" id="column2" style="display: none">
          <label class="col-3 text-right col-form-label">请输入视频地址</label>
          <div class="col-8">
            <asp:TextBox ID="TbPlayUrl" class="form-control" runat="server"></asp:TextBox>
          </div>
          <div class="col-1">
            <asp:RequiredFieldValidator ControlToValidate="TbPlayUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server"
            />
          </div>
        </div>

        <div class="form-group form-row">
          <div class="col-12 text-center ">
            <asp:CheckBox class="checkbox checkbox-primary" id="CbIsImageUrl" onClick="$('#CbIsImageUrl')[0].checked ? $('#columnImageUrl').show() : $('#columnImageUrl').hide();"
              Checked="true" runat="server" Text="设置封面" />
            <asp:CheckBox class="checkbox checkbox-primary" id="CbIsAutoPlay" Checked="true" runat="server" Text="自动播放" />
            <asp:CheckBox class="checkbox checkbox-primary" id="CbIsWidth" onClick="$('#CbIsWidth')[0].checked ? $('#columnWidth').show() : $('#columnWidth').hide();"
              Checked="true" runat="server" Text="设置宽度" />
            <asp:CheckBox class="checkbox checkbox-primary" id="CbIsHeight" onClick="$('#CbIsHeight')[0].checked ? $('#columnHeight').show() : $('#columnHeight').hide();"
              Checked="true" runat="server" Text="设置高度" />
          </div>
        </div>

        <div id="columnImageUrl" class="form-group form-row">
          <label class="col-3 text-right col-form-label">
            封面图片
          </label>
          <div class="col-6">
            <asp:TextBox ID="TbImageUrl" class="form-control" runat="server"></asp:TextBox>
            <div id="image_upload_txt" style="clear:both; font-size:12px; color:#FF3737;"></div>
          </div>
          <div class="col-2">
            <a id="upload_image" href="javascript:;" class="btn btn-success">上 传</a>
          </div>
        </div>

        <div class="form-group form-row">
          <label class="col-3 text-right col-form-label">
            视频播放器
          </label>
          <div class="col-8">
            <asp:DropDownList ID="DdlPlayBy" class="form-control" runat="server"></asp:DropDownList>
          </div>
          <div class="col-1">
          </div>
        </div>

        <div id="columnWidth" class="form-group form-row">
          <label class="col-3 text-right col-form-label">
            宽度
          </label>
          <div class="col-8">
            <asp:TextBox ID="TbWidth" class="form-control" runat="server"></asp:TextBox>
          </div>
          <div class="col-1">
            <asp:RegularExpressionValidator ControlToValidate="TbWidth" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
              foreColor="red" runat="server" />
          </div>
        </div>

        <div id="columnHeight" class="form-group form-row">
          <label class="col-3 text-right col-form-label">
            高度
          </label>
          <div class="col-8">
            <asp:TextBox ID="TbHeight" class="form-control" runat="server"></asp:TextBox>
          </div>
          <div class="col-1">
            <asp:RegularExpressionValidator ControlToValidate="TbHeight" ValidationExpression="\d+" Display="Dynamic" ErrorMessage=" *"
              foreColor="red" runat="server" />
          </div>
        </div>

        <hr />

        <div class="text-right mr-1">
          <asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
          <button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
        </div>

      </form>
    </body>

    </html>
    <!--#include file="../inc/foot.html"-->
    <script type="text/javascript" language="javascript">
      $(document).ready(function () {
        $('#CbIsImageUrl')[0].checked ? $('#columnImageUrl').show() : $('#columnImageUrl').hide();
        $('#CbIsWidth')[0].checked ? $('#columnWidth').show() : $('#columnWidth').hide();
        $('#CbIsHeight')[0].checked ? $('#columnHeight').show() : $('#columnHeight').hide();

        new AjaxUpload('upload_video', {
          action: '<%=UploadUrl%>',
          name: "videodata",
          data: {},
          onSubmit: function (video, ext) {
            var reg = /^(<%=VideoTypeCollection%>)$/i;
            if (ext && reg.test(ext)) {
              $('#video_upload_txt').text('上传中... ');
            } else {
              $('#video_upload_txt').text('系统不允许上传指定的格式');
              return false;
            }
          },
          onComplete: function (video, response) {
            $('#video_upload_txt').text('');
            if (response) {
              response = eval("(" + response + ")");
              if (response.success == 'true') {
                $('#TbPlayUrl').val(response.url);
                $('#column1').hide();
                $('#column2').show();
                $('.nav-tabs li').removeClass('active');
                $('#tab2').addClass('active');
              } else {
                $('#video_upload_txt').text(response.message);
              }
            }
          }
        });
        new AjaxUpload('upload_image', {
          action: '<%=UploadUrl%>',
          name: "imgdata",
          data: {},
          onSubmit: function (img, ext) {
            var reg = /^(<%=ImageTypeCollection%>)$/i;
            if (ext && reg.test(ext)) {
              $('#image_upload_txt').text('上传中... ');
            } else {
              $('#image_upload_txt').text('系统不允许上传指定的格式');
              return false;
            }
          },
          onComplete: function (file, response) {
            $('#image_upload_txt').text('');
            if (response) {
              response =
                eval("(" + response + ")");
              if (response.success == 'true') {
                $('#TbImageUrl').val(response.url);
              } else {
                $('#image_upload_txt').text(response.message);
              }
            }
          }
        });
      });
    </script>