function add_form(containerID)
{
	var $count = $('#' + containerID + '_Count');
	var count = parseInt($count.val());
	count = count + 1;
	var $el = $('<div>' + $('#' + containerID + '_1').html().replace(/_1/g, '_' + count) + '</div>');
	$el.insertBefore($count);
	$('#' + containerID + '_Count').val(count);
}

function submit_resume()
{
	if (checkFormValueById('frmResume'))
	{
		document.charset='utf-8';
		var frmResume = document.getElementById('frmResume');
		frmResume.action = resumeActionUrl;
		frmResume.target = 'iframeResume';
		frmResume.submit();
	}
}

$(document).ready(function(){
	$('#JobContentID').val($.request.queryString["jobID"]);
	new AjaxUpload('uploadFile', {
	 action: resumeAjaxUploadUrl,
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
