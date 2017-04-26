function selectApplyerType(val){
	if (val == "False"){
		$('#dataContainer1').show();
		$('#dataContainer2').hide();
	}else{
		$('#dataContainer1').hide();
		$('#dataContainer2').show();
	}
}

function submit_apply()
{
	if (checkFormValueById('frmApply'))
	{
		$('#frmApply').showLoading();
		var frmApply = document.getElementById('frmApply');
		frmApply.action = govPublicActionUrl;
		frmApply.target = 'iframeApply';
		frmApply.submit();
	}
}

$(document).ready(function(){
	$('#JobContentID').val($.request.queryString["jobID"]);
	new AjaxUpload('uploadFile', {
	 action: govPublicAjaxUploadUrl,
	 name: "ImageUrl",
	 data: {},
	 onSubmit: function(file, ext) {
		 var reg = /^(jpg|jpeg|gif)$/i;
		 if (ext && reg.test(ext)) {
			 $('#img_upload_txt').text('上传中... ');
		 } else {
			 $('#img_upload_txt').text('只允许上传JPG,GIF图片');
			 return false;
		 }
	 },
	 onComplete: function(file, response) {
		$('#img_upload_txt').text(' ');
		 if (response) {
			 response = eval("(" + response + ")");
			 if (response.success == 'true') {
				 $("#imgPhoto").attr('src', response.url);
				 $("#ImageUrl").val(response.value);
			 } else {
				 $('#img_upload_txt').text(response.message);
			 }
		 }
	 }
	});
});
