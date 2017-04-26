<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTextEditorInsertAudio" %>
<%@ Register TagPrefix="bairong" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<!--#include file="../inc/header.aspx"-->
</head>

<body>
<!--#include file="../inc/openWindow.html"-->
<form class="form-horizontal form-inline" runat="server">
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server" text="请选择插入音频的方式"></bairong:alerts>

  <bairong:code type="ajaxUpload" runat="server" />
  <script type="text/javascript" language="javascript">
  $(document).ready(function(){
    new AjaxUpload('uploadFile', {
     action: '<%=UploadUrl%>',
     name: "filedata",
     data: {},
     onSubmit: function(file, ext) {
       var reg = /^(<%=TypeCollection%>)$/i;
       if (ext && reg.test(ext)) {
         $('#img_upload_txt').text('上传中... ');
       } else {
         $('#img_upload_txt').text('系统不允许上传指定的格式');
         return false;
       }
     },
     onComplete: function(file, response) {
      $('#img_upload_txt').text('');
       if (response) {
         response = eval("(" + response + ")");
         if (response.success == 'true') {
           $('#TbPlayUrl').val(response.playUrl);
           $('#myTab a[href="#2"]').tab('show');
         } else {
           $('#img_upload_txt').text(response.message);
         }
       }
     }
    });
  });
  </script>

  <ul class="nav nav-pills" id="myTab">
    <li class="active"> <a href="#1" data-toggle="tab">上传音频</a> </li>
    <li><a href="#2" data-toggle="tab">输入地址</a></li>
  </ul>
  
  <div class="tab-content">
    <div class="control-group tab-pane active" id="1">
      <label class="control-label">请选择音频文件</label>
      <div class="controls" id="fileSelect">
        <div id="uploadFile" class="btn btn-success">选 择</div>
        <span id="img_upload_txt" style="clear:both; font-size:12px; color:#FF3737;"></span> </div>
    </div>
    <div class="control-group tab-pane" id="2">
      <label class="control-label">请输入音频地址</label>
      <div class="controls">
        <asp:TextBox ID="TbPlayUrl" runat="server" Width="220"></asp:TextBox>
        <asp:RequiredFieldValidator ControlToValidate="TbPlayUrl" ErrorMessage=" *" ForeColor="red" Display="Dynamic" runat="server" />
      </div>
    </div>
  </div>
  <div class="form-inline center">
    <label class="checkbox inline">
      <asp:CheckBox id="CbIsAutoPlay" Checked="true" runat="server" Text="自动播放"/>
    </label>
  </div>

</form>
</body>
</html>
