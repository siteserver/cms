function translateNodeAdd(name, value){
  $('#translateContainer').append("<span id='translate_" + value + "' class='label label-primary'>" + name + "&nbsp;<i class='fa fa-times' style='cursor:pointer' onClick=\"translateNodeRemove('" + value + "')\"></i>&nbsp;</span>&nbsp;&nbsp;");
  $('#translateCollection').val(value + ',' + $('#translateCollection').val());
  $('#translateType').show();
}
function translateNodeRemove(value){
  $('#translate_' + value).remove();
  var val = '';
  var values = $('#translateCollection').val().split(",");
  for (i=0;i<values.length ;i++ )
  {
    if (values[i] && value != values[i]){val = values[i] + ',';}
  }
  $('#translateCollection').val(val);
  if (val == ''){
    $('#translateType').hide();
  }
}
$(document).keypress(function(e){
  if(e.ctrlKey && e.which == 13 || e.which == 10) {
    e.preventDefault();
    $("#BtnSubmit").click();
  } else if (e.shiftKey && e.which==13 || e.which == 10) {
    e.preventDefault();
    $("#BtnSubmit").click();
  }
});

var isPreviewSaving = false;
function previewSave() {
    if (!$('#TbTitle').val()) return;
    if (isPreviewSaving) return;

    isPreviewSaving = true;
    var options = {
        beforeSubmit: function () {
            return true;
        },
        url: location.href + '&isPreview=True',
        type: 'POST',
        success: function (data) {
            isPreviewSaving = false;
            var obj = eval("(" + data + ")");
            if (obj.previewId) {
              window.open(previewUrl + '&isPreview=true&previewId=' + obj.previewId);
            }
        }
    };

    if (UE) {
        $.each(UE.instants, function (index, editor) {
            editor.sync();
        });
    }
    $('#myForm').ajaxSubmit(options);
}
