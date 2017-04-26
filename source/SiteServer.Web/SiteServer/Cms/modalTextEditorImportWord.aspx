<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTextEditorImportWord" %>
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
<bairong:alerts text="请点击按钮上传Word文件" runat="server"></bairong:alerts>

	<bairong:code type="ajaxUpload" runat="server" />
	

	<input type="hidden" id="fileNames" name="fileNames" value="" />
	<div class="control-group">
		<label class="control-label">请选择Word文件</label>
		<div class="controls" id="fileSelect">
		  <a id="upload_file" href="javascript:void(0);" class="btn btn-success">选 择</a>
		  <span id="img_upload_txt" style="clear:both; font-size:12px; color:#FF3737;"></span>
		</div>
	</div>
	<div class="center">
		<asp:CheckBox CssClass="checkbox inline" id="CbIsClearFormat" Checked="true" runat="server" Text="清除格式"/>
		<asp:CheckBox CssClass="checkbox inline" id="CbIsFirstLineIndent" Checked="true" runat="server" Text="首行缩进"/>
		<asp:CheckBox CssClass="checkbox inline" id="CbIsClearFontSize" Checked="true" runat="server" Text="清除字号"/>
		<asp:CheckBox CssClass="checkbox inline" id="CbIsClearFontFamily" Checked="true" runat="server" Text="清除字体"/>
		<asp:CheckBox CssClass="checkbox inline" id="CbIsClearImages" runat="server" Text="清除图片"/>
	</div>

	<script type="text/javascript" language="javascript">
	$(document).ready(function(){
		new AjaxUpload('upload_file', {
		 action: "<%=UploadUrl%>",
		 name: "filedata",
		 data: {},
		 onSubmit: function(file, ext) {
			 var reg = /^(doc|docx)$/i;
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
					 $('#fileSelect').append('<h5>' + response.fileName + '<h5>');
					 $('#fileNames').val($('#fileNames').val() + '|' + response.fileName);
				 } else {
					 $('#img_upload_txt').text(response.message);
				 }
			 }
		 }
		});
	});
	</script>

</form>
</body>
</html>
