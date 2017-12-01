<%@ Page Language="C#" Trace="false" Inherits="SiteServer.BackgroundPages.Cms.ModalUploadWord" %>
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
<asp:Button id="btnSubmit" useSubmitBehavior="false" OnClick="Submit_OnClick" runat="server" style="display:none" />
<bairong:alerts runat="server"></bairong:alerts>

	<script type="text/javascript" src="../assets/swfUpload/swfupload.js"></script>
	<script type="text/javascript" src="../assets/swfUpload/handlers.js"></script>
	<script type="text/javascript">
	function add_form(){
		var $count = $('#File_Count');
		var count = parseInt($count.val());
		count = count + 1;
		var $el = $("<div id='File_" + count + "'>" + $('#File_0').html().replace(/_0/g, '_' + count) + "</div>");
		$el.insertBefore($count);	
		$('#File_Count').val(count);
	}

	function remove_form(divID){
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
	$(document).ready(function(){
		swfu = new SWFUpload({
			// Backend Settings
			upload_url: "<%=GetUploadWordMultipleUrl()%>",

			// File Upload Settings
			file_size_limit : "20 MB",
			file_types : "*.doc;*.docx",
			file_types_description : "Word Files",
			file_upload_limit : 0,    // Zero means unlimited

			// Event Handler Settings - these functions as defined in Handlers.js
			//  The handlers are not part of SWFUpload but are part of my website and control how
			//  my website reacts to the SWFUpload events.
			swfupload_preload_handler : preLoad,
			swfupload_load_failed_handler : loadFailed,
			file_queue_error_handler : fileQueueError,
			file_dialog_complete_handler : fileDialogComplete,
			upload_error_handler : uploadError,
			upload_success_handler : uploadSuccess,
			upload_complete_handler : uploadComplete,

			// Button settings
			button_image_url : "../assets/swfUpload/button.png",
			button_placeholder_id : "swfUploadPlaceholder",
			button_width: 114,
			button_height: 22,
			button_text : '» 批量导入Word',
			button_text_top_padding: 1,
			button_text_left_padding: 10,

			// Flash Settings
			flash_url : "../assets/swfUpload/swfupload.swf",	// Relative to this file
			flash9_url : "../assets/swfUpload/swfupload_FP9.swf",	// Relative to this file

			// Debug Settings
			debug: false
		});
		
		$('#cbIsFirstLineTitle').click(function(e) {
			if(!$("#cbIsFirstLineTitle").attr("checked")){
				$("#cbIsFirstLineRemove").removeAttr("checked");
			};
		});
	});
	</script>

  <input id="File_Count" type="hidden" name="File_Count" value="0" />
  <table class="table noborder table-hover">
    <tr>
      <td colspan="2" align="right" style="text-align:right"><span id="swfUploadPlaceholder"></span></td>
    </tr>
    <tr>
      <td align="right">选项：</td>
      <td><asp:CheckBox CssClass="checkbox inline" ID="cbIsFirstLineTitle" Checked="true" runat="server" Text="将第一行作为标题"/>
      <asp:CheckBox CssClass="checkbox inline" ID="cbIsFirstLineRemove" Checked="true" runat="server" Text="内容正文删除标题"/>
        <asp:CheckBox CssClass="checkbox inline" id="cbIsClearFormat" Checked="true" runat="server" Text="清除格式"/>
        <br />
    <asp:CheckBox CssClass="checkbox inline" id="cbIsFirstLineIndent" Checked="true" runat="server" Text="首行缩进"/>
    <asp:CheckBox CssClass="checkbox inline" id="cbIsClearFontSize" Checked="true" runat="server" Text="清除字号"/>
    <asp:CheckBox CssClass="checkbox inline" id="cbIsClearFontFamily" Checked="true" runat="server" Text="清除字体"/>
    <asp:CheckBox CssClass="checkbox inline" id="cbIsClearImages" runat="server" Text="清除图片"/></td>
    </tr>
    <tr>
      <td align="right">状态：</td>
      <td><asp:RadioButtonList class="radiobuttonlist" ID="rblContentLevel" RepeatDirection="Horizontal" runat="server"/></td>
    </tr>
  </table>

	<div id="File_0" style="display:none">
	  <input type="hidden" id="fileName_0" name="fileName_0" value="" />
	  <table cellSpacing="3" width="100%" style="border-bottom:#c5daf0 1px dashed" cellpadding="3">
	    <tr>
	      <td><div id="divFileName_0"></div></td>
	      <td width="60" class="center"><a href="javascript:void(0);" onClick="remove_form('#File_0');">删除</a></td>
	    </tr>
	  </table>
	</div>

</form>
</body>
</html>
