<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadWord" %>
	<%@ Register TagPrefix="ctrl" Namespace="SiteServer.BackgroundPages.Controls" Assembly="SiteServer.BackgroundPages" %>
		<!DOCTYPE html>
		<html class="modalPage">

		<head>
			<meta charset="utf-8">
			<!--#include file="../inc/head.html"-->
		</head>

		<body>
			<!--#include file="../inc/openWindow.html"-->

			<form runat="server">
				<ctrl:alerts runat="server" />

				<input id="File_Count" type="hidden" name="File_Count" value="0" />
				<script type="text/javascript" src="../assets/swfUpload/swfupload.js"></script>
				<script type="text/javascript" src="../assets/swfUpload/handlers.js"></script>
				<script type="text/javascript">
					function add_form() {
						var $count = $('#File_Count');
						var count = parseInt($count.val());
						count = count + 1;
						var $el = $("<div id='File_" + count + "'>" + $('#File_0').html().replace(/_0/g, '_' + count) + "</div>");
						$el.insertBefore($count);
						$('#File_Count').val(count);
					}

					function remove_form(divID) {
						$(divID).remove();
					}

					function uploadSuccess(file, response) {
						try {
							if (response) {
								response = eval("(" + response + ")");

								if (response.success == 'true') {
									add_form();
									var $count = $('#File_Count');
									var index = parseInt($count.val());
									$("#fileName_" + index).val(response.fileName);
									$("#divFileName_" + index).html(response.fileName);
								} else {
									alert(response.message);
								}
							}
						} catch (ex) {
							this.debug(ex);
						}
					}

					var swfu;
					$(document).ready(function () {
						swfu = new SWFUpload({
							// Backend Settings
							upload_url: "<%=GetUploadWordMultipleUrl()%>",

							// File Upload Settings
							file_size_limit: "20 MB",
							file_types: "*.doc;*.docx",
							file_types_description: "Word Files",
							file_upload_limit: 0, // Zero means unlimited

							// Event Handler Settings - these functions as defined in Handlers.js
							//  The handlers are not part of SWFUpload but are part of my website and control how
							//  my website reacts to the SWFUpload events.
							swfupload_preload_handler: preLoad,
							swfupload_load_failed_handler: loadFailed,
							file_queue_error_handler: fileQueueError,
							file_dialog_complete_handler: fileDialogComplete,
							upload_error_handler: uploadError,
							upload_success_handler: uploadSuccess,
							upload_complete_handler: uploadComplete,

							// Button settings
							button_image_url: "../assets/swfUpload/button.png",
							button_placeholder_id: "swfUploadPlaceholder",
							button_width: 114,
							button_height: 22,
							button_text: '» 批量导入Word',
							button_text_top_padding: 1,
							button_text_left_padding: 10,

							// Flash Settings
							flash_url: "../assets/swfUpload/swfupload.swf", // Relative to this file
							flash9_url: "../assets/swfUpload/swfupload_FP9.swf", // Relative to this file

							// Debug Settings
							debug: false
						});

						$('#CbIsFirstLineTitle').click(function (e) {
							if (!$("#CbIsFirstLineTitle").attr("checked")) {
								$("#CbIsFirstLineRemove").removeAttr("checked");
							};
						});
					});
				</script>

				<div class="form-horizontal">

					<div class="form-group">
						<label class="col-xs-2 text-right control-label"></label>
						<div class="col-xs-9">
							<span id="swfUploadPlaceholder"></span>
						</div>
						<div class="col-xs-1 help-block"></div>
					</div>

					<div class="form-group">
						<label class="col-xs-2 text-right control-label">设置</label>
						<div class="col-xs-9">
							<div class="checkbox checkbox-primary">
								<asp:CheckBox ID="CbIsFirstLineTitle" Checked="true" runat="server" Text="将第一行作为标题" />
								<asp:CheckBox ID="CbIsFirstLineRemove" Checked="true" runat="server" Text="内容正文删除标题" />
								<asp:CheckBox id="CbIsClearFormat" Checked="true" runat="server" Text="清除格式" />
								<br />
								<asp:CheckBox id="CbIsFirstLineIndent" Checked="true" runat="server" Text="首行缩进" />
								<asp:CheckBox id="CbIsClearFontSize" Checked="true" runat="server" Text="清除字号" />
								<asp:CheckBox id="CbIsClearFontFamily" Checked="true" runat="server" Text="清除字体" />
								<asp:CheckBox id="CbIsClearImages" runat="server" Text="清除图片" />
							</div>
						</div>
						<div class="col-xs-1 help-block"></div>
					</div>

					<div class="form-group">
						<label class="col-xs-2 text-right control-label">状态</label>
						<div class="col-xs-9">
							<asp:DropDownList ID="DdlContentLevel" class="form-control" runat="server" />
						</div>
						<div class="col-xs-1 help-block"></div>
					</div>

					<div class="form-group" id="File_0" style="display:none">
						<label class="col-xs-2 text-right control-label">
							<input type="hidden" id="fileName_0" name="fileName_0" value="" />
						</label>
						<div class="col-xs-9">
							<div id="divFileName_0"></div>
						</div>
						<div class="col-xs-1 help-block">
							<a href="javascript:void(0);" onClick="remove_form('#File_0');">删除</a>
						</div>
					</div>

					<hr />

					<div class="form-group m-b-0">
						<div class="col-xs-11 text-right">
							<asp:Button class="btn btn-primary m-l-10" text="确 定" runat="server" onClick="Submit_OnClick" />
							<button type="button" class="btn btn-default m-l-10" onclick="window.parent.layer.closeAll()">取 消</button>
						</div>
						<div class="col-xs-1"></div>
					</div>

				</div>

			</form>
		</body>

		</html>