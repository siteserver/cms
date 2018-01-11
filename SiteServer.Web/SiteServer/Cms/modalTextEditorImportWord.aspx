<%@ Page Language="C#" Inherits="SiteServer.BackgroundPages.Cms.ModalTextEditorImportWord" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<form runat="server">
				<ctrl:alerts text="请点击按钮上传Word文件" runat="server" />
				<input type="hidden" id="fileNames" name="fileNames" value="" />
				<ctrl:code type="ajaxUpload" runat="server" />

				<div class="form-group form-row">
					<div class="col-12">
						<div id="fileSelect" class="ml-4">
							<a id="upload_file" href="javascript:;" class="btn btn-success">上 传</a>
							<span id="img_upload_txt" style="clear:both; font-size:12px; color:#FF3737;"></span>
						</div>
					</div>
				</div>

				<div class="form-group form-row">
					<div class="col-12 text-center">
						<div class="checkbox checkbox-primary">
							<asp:CheckBox id="CbIsClearFormat" Checked="true" runat="server" Text="清除格式" />
							<asp:CheckBox id="CbIsFirstLineIndent" Checked="true" runat="server" Text="首行缩进" />
							<asp:CheckBox id="CbIsClearFontSize" Checked="true" runat="server" Text="清除字号" />
							<asp:CheckBox id="CbIsClearFontFamily" Checked="true" runat="server" Text="清除字体" />
							<asp:CheckBox id="CbIsClearImages" runat="server" Text="清除图片" />
						</div>
					</div>
				</div>

				<script type="text/javascript" language="javascript">
					$(document).ready(function () {
						new AjaxUpload('upload_file', {
							action: "<%=UploadUrl%>",
							name: "filedata",
							data: {},
							onSubmit: function (file, ext) {
								var reg = /^(doc|docx)$/i;
								if (ext && reg.test(ext)) {
									$('#img_upload_txt').text('上传中... ');
								} else {
									$('#img_upload_txt').text('系统不允许上传指定的格式');
									return false;
								}
							},
							onComplete: function (file, response) {
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

				<hr />

				<div class="text-right mr-1">
					<asp:Button class="btn btn-primary m-l-5" text="确 定" runat="server" onClick="Submit_OnClick" />
					<button type="button" class="btn btn-default m-l-5" onclick="window.parent.layer.closeAll()">取 消</button>
				</div>

			</form>
		</body>

		</html>
		<!--#include file="../inc/foot.html"-->